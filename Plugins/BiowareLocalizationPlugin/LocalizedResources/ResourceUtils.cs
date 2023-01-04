
using Frosty.Core;
using FrostySdk.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BiowareLocalizationPlugin.LocalizedResources
{
    /// <summary>
    /// Helper class for methods that must not necessarily be inlcuded in the resource class.
    /// </summary>
    public class ResourceUtils
    {
        private ResourceUtils()
        {
            // prevent instantiation
        }

        /// <summary>
        /// Reads the header information from the given reader and asset entry.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ResourceHeader ReadHeader(NativeReader reader)
        {
                 
            uint magic = reader.ReadUInt();
            if (magic != ResourceHeader.Magic)
                throw new InvalidDataException();

            ResourceHeader header = new ResourceHeader
            {
                Unknown1 = reader.ReadUInt(),
                DataOffset = reader.ReadUInt(),
                Unknown2 = reader.ReadUInt(),
                Unknown3 = reader.ReadUInt(),
                Unknown4 = reader.ReadUInt(),

                NodeCount = reader.ReadUInt(),
                NodeOffset = reader.ReadUInt(),

                StringsCount = reader.ReadUInt(),
                StringsOffset = reader.ReadUInt()
            };

            // this block's first offset is the position after the stringId and positions are parsed, subsequent offsets (and the dataoffset) are 8 bytes * count further than the last
            for(int i = 0; i<2 && reader.Position < header.NodeOffset; i++)
            {
                header.FirstUnknownDataDefSegments.Add(ReadCountAndOffset(reader));
            }

            // The remainder until the node offset is reached is filled by ids and positions of declinated articles for dragon age crafting.
            while (reader.Position < header.NodeOffset)
            {
                header.AddDragonAgeDeclinatedCraftingNamePart(ReadCountAndOffset(reader));
            }

            return header;
        }

        private static DataCountAndOffsets ReadCountAndOffset(NativeReader reader)
        {
            DataCountAndOffsets somePointer = new DataCountAndOffsets
            {
                Count = reader.ReadUInt(),
                Offset = reader.ReadUInt()
            };

            return somePointer;
        }

        public static byte[] ReadUnkownSegment(NativeReader reader, DataCountAndOffsets countAndOffset)
        {
            return reader.ReadBytes(((int)countAndOffset.Count) * 8);
        }

        /// <summary>
        /// Creates a list of huffman nodes from the given reader and node count.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="nodeCount">The total number of nodes to read</param>
        /// <param name="supportedCharacters">The list of characters encoded.</param>
        /// <returns>The root node</returns>
        public static HuffmanNode ReadNodes(NativeReader reader, uint nodeCount, out List<char> supportedCharacters)
        {

            HuffmanNode rootNode = null;
            HuffmanNode leftNode = null;
            HuffmanNode rightNode = null;

            List<HuffmanNode> nodes = new List<HuffmanNode>();
            int nodeValue = 0;

            for (int i = 0; i < nodeCount; i++)
            {
                HuffmanNode n = new HuffmanNode() { Value = reader.ReadUInt() };

                int idx = nodes.FindIndex((HuffmanNode a) => { return a.Value == n.Value; });
                if (idx != -1)
                    n = nodes[idx];

                if (leftNode == null)
                {
                    leftNode = n;
                }
                else if (rightNode == null)
                {
                    rightNode = n;
                    if (idx == -1)
                        nodes.Add(rightNode);

                    n = new HuffmanNode
                    {
                        Value = (uint)nodeValue++,
                    };
                    n.SetLeftNode(leftNode);
                    n.SetRightNode(rightNode);

                    rootNode = n;

                    leftNode = null;
                    rightNode = null;
                    idx = -1;

                }

                if (idx == -1)
                    nodes.Add(n);
            }

            supportedCharacters = GetLeafCharacters(nodes);

            return rootNode;
        }

        private static List<char> GetLeafCharacters(List<HuffmanNode> nodes)
        {
            List<char> leafCharacters = new List<char>();
            foreach (HuffmanNode node in nodes)
            {
                if (node.Left == null && node.Right == null
                    // exclude letter 0x00 / value 0xFFFF as that is used as end text marker
                    && (node.Value != uint.MaxValue))
                {
                    leafCharacters.Add(node.Letter);
                }
            }

            leafCharacters.Sort();
            return leafCharacters;
        }

        /// <summary>
        /// For the sub tree starting at the given node, this method returns all the tree elements without the root.
        /// Note that the list representation tries to represent the tree bottom-up, starting with the most left side node at leach depth level.
        /// </summary>
        /// <param name="rootNode">The root of the tree of which to return all nodes as list.</param>
        /// <returns>The list of all nodes in the tree, excluding the root.</returns>
        /// <remarks>This does not work for creating the list to Write! Use GetNodeListToWrite for that instead!</remarks>
        public static List<HuffmanNode> GetNodeList(HuffmanNode rootNode)
        {

            List<HuffmanNode> nodesSansRoot = new List<HuffmanNode>();

            if(rootNode == null)
            {
                App.Logger.Log("Given Root Node was null!");
                return nodesSansRoot;
            }

            bool hasNextLevel = true;
            List<HuffmanNode> nextLevel = new List<HuffmanNode> { rootNode };
            while (hasNextLevel)
            {
                nextLevel = GetNextLevel(nextLevel);
                nodesSansRoot.AddRange(nextLevel);

                hasNextLevel = nextLevel.Any();
            }

            nodesSansRoot.Reverse();

            return nodesSansRoot;
        }

        /// <summary>
        /// Returns a list with all the children of the nodes in the given list. For each node in the given list the right child is added before the left one.
        /// </summary>
        /// <param name="currentLevel">the list of currently selected nodes</param>
        /// <returns>the list of child nodes</returns>
        public static List<HuffmanNode> GetNextLevel(List<HuffmanNode> currentLevel)
        {
            List<HuffmanNode> nextLevel = new List<HuffmanNode>(currentLevel.Count * 2);
            foreach (HuffmanNode n in currentLevel)
            {
                if (n.Right != null)
                    nextLevel.Add(n.Right);

                if (n.Left != null)
                    nextLevel.Add(n.Left);
            }
            return nextLevel;
        }

        /// <summary>
        /// Recalculates the nodelist to write to the resource based on the single remembered root node.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>list of nodes in the order to write</returns>
        public static List<HuffmanNode> GetNodeListToWrite(HuffmanNode rootNode)
        {
            List<HuffmanNode> nodesSansRoot = new List<HuffmanNode>();

            if (rootNode == null)
            {
                App.Logger.Log("Given root node was null!");
                return nodesSansRoot;
            }

            // get all branches
            List<HuffmanNode> branches = GetAllBranchNodes( new List<HuffmanNode>() { rootNode });

            // sort branches by their value, so that the write out can happen in the correct order
            branches.Sort();

            // add all the children in the order of their parent's value
            foreach(HuffmanNode branch in branches)
            {
                nodesSansRoot.Add(branch.Left);
                nodesSansRoot.Add(branch.Right);
            }

            return nodesSansRoot;
        }

        private static List<HuffmanNode> GetAllBranchNodes(List<HuffmanNode> currentNodes)
        {
            List<HuffmanNode> branchNodes = new List<HuffmanNode>();
            
            foreach(HuffmanNode currentNode in currentNodes)
            {
                if(currentNode.Left != null && currentNode.Right != null)
                {
                    branchNodes.Add(currentNode);
                    branchNodes.AddRange(
                        GetAllBranchNodes( new List<HuffmanNode>() { currentNode.Left, currentNode.Right }));
                }
            }

            return branchNodes;
        }

        /// <summary>
        /// Returns a dictionary of characters to their encoded bit representation.
        /// </summary>
        /// <param name="encodingNodes">The (limited) list of Huffman code nodes used.</param>
        /// <returns>Dictionary of char - code values</returns>
        public static Dictionary<char, List<bool>> GetCharEncoding(List<HuffmanNode> encodingNodes)
        {

            Dictionary<char, List<bool>> charEncodings = new Dictionary<char, List<bool>>();

            foreach (HuffmanNode node in encodingNodes)
            {
                if (node.Left == null && node.Right == null)
                {
                    char c = node.Letter;
                    List<bool> path = GetCharEncoding(node);

                    charEncodings.Add(c, path);
                }
            }
            return charEncodings;
        }

        /// <summary>
        /// Return the encoding for the given node as path in the tree.
        /// </summary>
        /// <param name="node">The node for which to find the encoding.</param>
        /// <returns>the encoding as list of bools.</returns>
        private static List<bool> GetCharEncoding(HuffmanNode node)
        {
            HuffmanNode parent = node.Parent;
            if (parent == null)
            {
                return new List<bool>();
            }

            List<bool> encoding = GetCharEncoding(parent);

            if (node == parent.Left)
            {
                encoding.Add(false);
            }
            else if (node == parent.Right)
            {
                encoding.Add(true);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Trying to find encoding for node <{0}> failed to to incorrect setup tree!",
                        node.ToString()));
            }

            return encoding;
        }

        /// <summary>
        /// Returns the bit encoded text as list of bools.
        /// The end of the text is marked with the delimiter character 0x0 / huffman node value = uint.MaxValue.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <param name="charEncoding">The character encoding to use for the text.</param>
        /// <returns>The encoded text.</returns>
        public static List<bool> GetEncodedText(String text, IDictionary<char, List<bool>> charEncoding)
        {

            List<bool> encodedText = new List<bool>();

            foreach (char c in text.ToCharArray())
            {
                encodedText.AddRange(charEncoding[c]);
            }

            // add the text delimeter:
            char delimiter = (char)0x0;
            encodedText.AddRange(charEncoding[delimiter]);

            return encodedText;
        }

        /// <summary>
        /// Sets the given texts position to the given start position, returning the posiiton offset for the next textblock.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="textBlock"></param>
        /// <returns>the position after the given text has been written.</returns>
        public static int UpdateTextAndGetNextTextPosition(int startPosition, EncodedTextPosition textBlock)
        {
            int nextPosition = startPosition;
            if (textBlock.Position < 0)
            {
                textBlock.Position = startPosition;
                nextPosition += textBlock.GetLength();
            }

            return nextPosition;
        }

        /// <summary>
        /// Retunrs a byte array representing all texts in this resource.
        /// </summary>
        /// <param name="sortedTexts"></param>
        /// <returns>byte array</returns>
        public static byte[] GetTextRepresentationToWrite(SortedSet<EncodedTextPosition> sortedTexts)
        {

            List<bool> allBits = new List<bool>();
            foreach (EncodedTextPosition textEntry in sortedTexts)
            {
                allBits.AddRange(textEntry.EncodedText.Value);
            }

            int byteSize = allBits.Count / 8 + 1;
            BitArray ba = new BitArray(allBits.ToArray());

            byte[] byteArray = new byte[byteSize];
            ba.CopyTo(byteArray, 0);
            return byteArray;
        }

        /// <summary>
        /// Checks whether the strings to check include only characters that are included in the given supported char list.
        /// </summary>
        /// <param name="stringsToCheck"></param>
        /// <param name="allSupportedCharacters"></param>
        /// <returns>true if all string characters are included in the supported char list</returns>
        public static bool IncludesOnlySupportedCharacters(IEnumerable<string> stringsToCheck, List<char> allSupportedCharacters)
        {
            HashSet<char> allCharsToCheck = new HashSet<char>();
            foreach(string stringToCheck in stringsToCheck)
            {
                allCharsToCheck.UnionWith(stringToCheck.AsEnumerable());
            }

            // the list of supported characters must be sorted in ascending order for this to work
            // the getLeaf chars method herein does this now per default

            foreach(char toCheck in allCharsToCheck)
            {
                foreach(char supported in allSupportedCharacters)
                {
                    if(supported == toCheck)
                    {
                        // found it, no need to search further
                        break;
                    }
                    else if(supported > toCheck)
                    {
                        // already past the point where it should have been found
                        return false;
                    }
                }
                // char not found in supported chars
                return false;
            }

            // all chars found
            return true;
        }

        /// <summary>
        /// Calculates the huffman encoding for the given texts, and returns the root node of the resulting tree.
        /// </summary>
        /// <param name="texts"></param>
        /// <returns>Huffman root node.</returns>
        public static HuffManConstructionNode CalculateHuffmanEncoding(IEnumerable<string> texts)
        {

            // get set of chars and their number of occurences...
            Dictionary<char, int> charNumbers = new Dictionary<char, int>();
            foreach(string text in texts)
            {
                foreach(char c in text)
                {
                    if( charNumbers.TryGetValue(c, out int occurences))
                    {
                        charNumbers[c] = ++occurences;
                    }
                    else
                    {
                        charNumbers[c] = 1;
                    }
                }
            }

            // add the text delimeter:
            char delimiter = (char)0x0;
            charNumbers[delimiter] = texts.Count();

            List<HuffManConstructionNode> nodeList = new List<HuffManConstructionNode>();
            foreach(var entry in charNumbers)
            {
                nodeList.Add( new HuffManConstructionNode()
                    {
                        Value = ~(uint)entry.Key,
                        Occurences = entry.Value
                    });
            }

            uint nodeValue = 0;
            while (nodeList.Count > 1)
            {

                nodeList.Sort();

                HuffManConstructionNode left = nodeList[0];
                HuffManConstructionNode right = nodeList[1];

                nodeList.RemoveRange(0, 2);

                HuffManConstructionNode composite = new HuffManConstructionNode() {
                    Value = nodeValue++,
                };
                composite.SetLeftNode(left);
                composite.SetRightNode(right);

                nodeList.Add(composite);
            }

            return nodeList[0];
        }

        /// <summary>
        /// Returns an object with the encoded texts and their positions as well as id grouping for the primary and secondary placements.
        /// </summary>
        /// <param name="allGroupedTextsById">The texts to encode mapped to their id, grouped by their origin</param>
        /// <param name="characterEncoding">The encoding to use</param>
        /// <returns>TextIds to text and position</returns>
        public static EncodedTextPositionGrouping GetEncodedTextsToWrite(
            List<SortedDictionary<uint, string>> allGroupedTextsById,
            IDictionary<char, List<bool>> characterEncoding)
        {

            Dictionary<string, EncodedText> dictionaryOfEncodedTexts = new Dictionary<string, EncodedText>();
            Dictionary<EncodedText, EncodedTextPosition> uniqueTextPositions = new Dictionary<EncodedText, EncodedTextPosition>();

            IDictionary<uint, string> primaryTextsById = allGroupedTextsById[0];
            Dictionary<uint, EncodedText> encodedPrimaryTexts = GetEncodedTextsById(uniqueTextPositions, dictionaryOfEncodedTexts, primaryTextsById, characterEncoding);

            List<Dictionary<uint, EncodedText>> encodedDeclinatedArticleTexts = new List<Dictionary<uint, EncodedText>>();
            for (int groupId = 1; groupId < allGroupedTextsById.Count; groupId++)
            {

                IDictionary<uint, string> textsById = allGroupedTextsById[groupId];
                Dictionary<uint, EncodedText> encodedTexts = GetEncodedTextsById(uniqueTextPositions, dictionaryOfEncodedTexts, textsById, characterEncoding);

                encodedDeclinatedArticleTexts.Add(encodedTexts);
            }

            // Calculate the actual bit offsets for the texts
            int currentTextPosition = 0;
            foreach (EncodedTextPosition textPosition in uniqueTextPositions.Values)
            {
                currentTextPosition = ResourceUtils.UpdateTextAndGetNextTextPosition(currentTextPosition, textPosition);
            }

            SortedDictionary<uint, EncodedTextPosition> primaryTextsSortedById = MapEncodedTextPositionById(encodedPrimaryTexts, uniqueTextPositions);

            List<SortedDictionary<uint, EncodedTextPosition>> encodedDeclinatedArticleTextsById = new List<SortedDictionary<uint, EncodedTextPosition>>();
            foreach(var idMappedText in encodedDeclinatedArticleTexts)
            {
                SortedDictionary<uint, EncodedTextPosition> encodedTextsById = MapEncodedTextPositionById(idMappedText, uniqueTextPositions);
                encodedDeclinatedArticleTextsById.Add(encodedTextsById);
            }

            return new EncodedTextPositionGrouping(primaryTextsSortedById, encodedDeclinatedArticleTextsById, new SortedSet<EncodedTextPosition>(uniqueTextPositions.Values));
        }

        /// <summary>
        /// From the given textsById map and characterEncoding this method creates the returned map of EncodedText entries.
        /// It also updates the given uniqueTextPositions dictionary with the created items.
        /// </summary>
        /// <param name="uniqueTextPositions">The dictionary of each unique string to write and their position</param>
        /// <param name="dictionaryOfEncodedTexts">The dictionary of already encoded texts</param>
        /// <param name="textsById">the not yet encoded strings for an id</param>
        /// <param name="characterEncoding">the encodeing</param>
        /// <returns>the encoded string for an id</returns>
        private static Dictionary<uint, EncodedText> GetEncodedTextsById(
            IDictionary<EncodedText, EncodedTextPosition> uniqueTextPositions,
            Dictionary<string, EncodedText> dictionaryOfEncodedTexts,
            IDictionary<uint, string> textsById, IDictionary<char, List<bool>> characterEncoding)
        {
            Dictionary<uint, EncodedText> encodedTexts = new Dictionary<uint, EncodedText>();
            foreach (KeyValuePair<uint, string> entry in textsById)
            {
                string text = entry.Value;
                bool encodedTextExists = dictionaryOfEncodedTexts.TryGetValue(text, out EncodedText encodedText);
                if (!encodedTextExists)
                {
                    // Same strings are treated as equal,
                    // so we reduce the set of encodedTextPositions here a bit while keeping a link to the text id via the encodedText itself
                    // The original resource is compressed even further, with different texts being stored as overlapping sequences of the same bits
                    encodedText = new EncodedText(ResourceUtils.GetEncodedText(text, characterEncoding));
                    uniqueTextPositions[encodedText] = new EncodedTextPosition(encodedText);
                }
                encodedTexts[entry.Key] = encodedText;
            }

            return encodedTexts;
        }

        /// <summary>
        /// Combines the two given dictionaries, returning a single mapping from text id to an encoded text with position.
        /// </summary>
        /// <param name="encodedTexts"></param>
        /// <param name="uniqueTextPositions"></param>
        /// <returns></returns>
        private static SortedDictionary<uint, EncodedTextPosition> MapEncodedTextPositionById (
            IDictionary<uint, EncodedText> encodedTexts,
            IDictionary<EncodedText, EncodedTextPosition> uniqueTextPositions )
        {
            SortedDictionary<uint, EncodedTextPosition> textsSortedById = new SortedDictionary<uint, EncodedTextPosition>();
            foreach (KeyValuePair<uint, EncodedText> entry in encodedTexts)
            {
                textsSortedById.Add(entry.Key, uniqueTextPositions[entry.Value]);
            }

            return textsSortedById;
        }

        /// <summary>
        /// Using Frostys NativeWriter / Reader to persist texts in the mod format does break certain non ascii characters (even though unicode utf-8 is used...?).
        /// To circumvent that we write the texts ourselves, guaranteed in ut-8
        /// </summary>
        /// <param name="textEntriesToWrite"></param>
        /// <returns></returns>
        public static byte[] ConvertTextEntriesToBytes(Dictionary<uint,string> textEntriesToWrite)
        {

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8))
                {

                    foreach (KeyValuePair<uint, string> textEntry in textEntriesToWrite)
                    {
                        writer.Write(textEntry.Key);
                        writer.Write(textEntry.Value);
                    }
                    writer.Flush();
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// For each adjective included in the map, this first writes the adjective id, then the number of declinations, and then the adjectives themselves.
        /// </summary>
        /// <param name="adjectiveEntriesToWrite"></param>
        /// <returns></returns>
        public static byte[] ConvertAdjectivesToBytes(Dictionary<uint, List<string>> adjectiveEntriesToWrite)
        {

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8))
                {

                    foreach (KeyValuePair<uint, List<string>> textEntry in adjectiveEntriesToWrite)
                    {

                        List<string> declinationsList = textEntry.Value;

                        writer.Write(textEntry.Key);
                        writer.Write(declinationsList.Count);

                        foreach(string declination in declinationsList)
                        {
                            writer.Write(declination);
                        }
                    }
                    writer.Flush();
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Reads the given byte array as utf-8 string with 7bit size info prependet.
        /// </summary>
        /// <param name="parseable">The bytes to parse</param>
        /// <returns>The parsed text</returns>
        private static string ConvertBytesToString(byte[] parseable)
        {
            using(BinaryReader reader = new BinaryReader( new MemoryStream(parseable), Encoding.UTF8 ))
            {
                return reader.ReadString();
            }
        }

        /// <summary>
        /// Returns the read text, forwarding the readers position to after the text.
        /// As the Frosty NativeReader reads non ASCII chars not entirely correct - we have to wrap this stuff into a binary reader using utf-8 encoding, so that hopefully the correct text is parsed.
        /// </summary>
        /// <param name="reader">The reader currently used to read the mod</param>
        /// <returns>The read text</returns>
        public static string ReadModString(NativeReader reader)
        {
            long position = reader.Position;

            int stringLength = reader.Read7BitEncodedInt();

            int offset = (int)( reader.Position - position );

            reader.Position = position;
            byte[] modTextBytes = reader.ReadBytes(stringLength + offset);

            return ConvertBytesToString(modTextBytes);
        }
    }
}
