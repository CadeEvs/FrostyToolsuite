
using Frosty.Core;
using FrostySdk.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                LanguageAndDeclinationsMarker = reader.ReadUInt(),
                Unknown2 = reader.ReadUInt(),
                Unknown3 = reader.ReadUInt(),

                NodeCount = reader.ReadUInt(),
                NodeOffset = reader.ReadUInt(),

                StringsCount = reader.ReadUInt(),
                StringsOffset = reader.ReadUInt(),

                ItemNameSetupCountsAndOffsets = ReadCountAndOffset(reader),
                AdjectiveDeclinationsCountsAndOffsets = ReadCountAndOffset(reader),
            };

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

        /// <summary>
        /// Reads dictionary entries from the given count and offset.
        /// </summary>
        /// <param name="reader">the reader</param>
        /// <param name="countAndOffset">the data holding count and offset</param>
        /// <returns></returns>
        public static IDictionary<uint, uint> ReadDictionary(NativeReader reader, DataCountAndOffsets countAndOffset)
        {
            IDictionary<uint, uint> dictionary = new Dictionary<uint, uint>();

            for (int i = 0; i < countAndOffset.Count; i++)
            {
                uint key = reader.ReadUInt();
                uint value = reader.ReadUInt();

                dictionary[key] = value;
            }

            return dictionary;
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

            if (rootNode == null)
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
                App.Logger.LogError("Given root node was null!");
                return nodesSansRoot;
            }

            // get all branches
            List<HuffmanNode> branches = GetAllBranchNodes(new List<HuffmanNode>() { rootNode });

            // sort branches by their value, so that the write out can happen in the correct order
            branches.Sort();

            // add all the children in the order of their parent's value
            foreach (HuffmanNode branch in branches)
            {
                nodesSansRoot.Add(branch.Left);
                nodesSansRoot.Add(branch.Right);
            }

            return nodesSansRoot;
        }

        private static List<HuffmanNode> GetAllBranchNodes(List<HuffmanNode> currentNodes)
        {
            List<HuffmanNode> branchNodes = new List<HuffmanNode>();

            foreach (HuffmanNode currentNode in currentNodes)
            {
                if (currentNode.Left != null && currentNode.Right != null)
                {
                    branchNodes.Add(currentNode);
                    branchNodes.AddRange(
                        GetAllBranchNodes(new List<HuffmanNode>() { currentNode.Left, currentNode.Right }));
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
        public static List<bool> GetCharEncoding(HuffmanNode node)
        {
            HuffmanNode parent = node.Parent;
            if (parent == null)
            {
                return new List<bool>();
            }

            if (node.GetType() == typeof(HuffManConstructionNode))
            {
                return ((HuffManConstructionNode)node).GetNodeEncoding();
            }

            List<bool> encoding = GetCharEncoding(parent);

            encoding.Add(GetBoolValueFromParent(node));

            return encoding;
        }

        /// <summary>
        /// Returns the bool value for the huffman encoding based on the parents node. Requires that the parent exists!
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool GetBoolValueFromParent(HuffmanNode node)
        {
            // if we really messed up, then all of these could be null
            HuffmanNode parent = node.Parent;
            HuffmanNode left = parent?.Left;
            HuffmanNode right = parent?.Right;

            if (node == left)
            {
                return false;
            }
            else if (node == right)
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Trying to find encoding for node <{0}> failed due to incorrect tree setup!",
                        node.ToString()));
            }
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
        /// Sets the given texts position to the given start position, returning the position offset for the next textblock.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="textBlock"></param>
        /// <returns>the position after the given text has been written.</returns>
        private static int UpdateTextAndGetNextTextPosition(int startPosition, EncodedTextPosition textBlock)
        {
            int nextPosition = startPosition;
            if (textBlock.Position < 0)
            {
                textBlock.Position = startPosition;
                nextPosition += textBlock.GetLength();
            }

            return nextPosition;
        }

        // Simple naive attempt at finding bit overlaps for texts.
        // this is probably now working out well or at all.
        private static void FindTextPositionWithOverlapp(EncodedTextPosition textBlockToInsert, List<bool> CurrentListOfTextsBits)
        {
            List<bool> encodedTextBits = textBlockToInsert.EncodedText.Value;

            // the list must include at least the char sequence for the delimiter, so it is never empty
            int currentTextBitsCount = CurrentListOfTextsBits.Count;
            ParallelLoopResult offsetResult = Parallel.For(0, currentTextBitsCount, (int offset, ParallelLoopState state) =>
            {
                bool foundworkingOffset = GetBitOffset(CurrentListOfTextsBits, offset, encodedTextBits);
                if (foundworkingOffset)
                {
                    state.Break();
                }
            });

            List<bool> bitsToInsert = encodedTextBits;
            bool foundPosition = offsetResult.LowestBreakIteration.HasValue;
            int foundOffset = currentTextBitsCount;
            if (foundPosition)
            {
                // this means we  found something int he loop
                foundOffset = ((int)offsetResult.LowestBreakIteration.Value);

                int numberOfMissingBitsToAppend = foundOffset + encodedTextBits.Count - currentTextBitsCount;
                if (numberOfMissingBitsToAppend > 0)
                {
                    bitsToInsert = encodedTextBits.GetRange(encodedTextBits.Count - numberOfMissingBitsToAppend, numberOfMissingBitsToAppend);
                }
                else
                {
                    bitsToInsert = new List<bool>();
                }
            }

            textBlockToInsert.Position = foundOffset;
            CurrentListOfTextsBits.AddRange(bitsToInsert);
        }

        private static bool GetBitOffset(List<bool> currentListOfTextsBits, int offset, List<bool> bitsToInsert)
        {
            bool match = false;
            // i don't like this nested loop inside another loop with break calls nested, but i don't see a better way right now at 3:30 am :(
            for (int testBitAt = 0; testBitAt < bitsToInsert.Count; testBitAt++)
            {
                // offset + testbit beyond current list of bits -> ? abort, update and return depending on previous match
                // bit does not match -> go to next offset / break
                // bit matches:
                //      - before end: go check next bit
                //      - at end/all bits match -> update and return from method
                //      - all bits match until end of currentlist.. -> update and return from method

                if (offset + testBitAt >= currentListOfTextsBits.Count)
                {
                    // if matched so far -> update partial, use offset and return, else retry from next offset
                    return match;
                }

                if (bitsToInsert[testBitAt] != currentListOfTextsBits[offset + testBitAt])
                {
                    // start again from next offset.
                    return false;
                }

                // else last one did match!
                match = true;
                if (testBitAt == bitsToInsert.Count - 1)
                {
                    // all the bits match!
                    return match;
                }

                // we we have a partial match - we already handled going past the current list size, so just do nothing and check the next testBit!
            }
            return match;
        }

        private static byte[] getTextSizedOrderedByteArrayWithOverlap(Dictionary<string, EncodedText> dictionaryOfEncodedTexts, Dictionary<EncodedText, EncodedTextPosition> uniqueTextPositions)
        {

            // this is some bullshit variant, just for test
            // it doesnt even work properly...

            IComparer<string> stringLenghtCompare = Comparer<string>.Create(
                (a, b) =>
                {
                    int lc = b.Length.CompareTo(a.Length); // reverse
                    if (lc != 0) return lc;
                    return a.CompareTo(b);
                }); // wtf is this?
            SortedDictionary<string, EncodedText> sortedStringDict = new SortedDictionary<string, EncodedText>(dictionaryOfEncodedTexts, stringLenghtCompare);

            List<bool> encodedTextBits = new List<bool>();
            List<string> alreadyUsedStrings = new List<string>();
            foreach (var entry in sortedStringDict)
            {
                string stringToAdd = entry.Key;
                EncodedText currentText = entry.Value;
                EncodedTextPosition currentTextPositionData = uniqueTextPositions[currentText];

                // parallel attempt
                Parallel.ForEach(alreadyUsedStrings,
                    (string alreadyUsedString, ParallelLoopState state) =>
                    {
                        if (alreadyUsedString.EndsWith(stringToAdd))
                        {
                            EncodedText longerEncodedString = dictionaryOfEncodedTexts[alreadyUsedString];
                            int bitOffset = longerEncodedString.GetLength() - currentText.GetLength(); // this should work, right?!

                            int longerEncodedStringPosition = uniqueTextPositions[longerEncodedString].Position;

                            currentTextPositionData.Position = longerEncodedStringPosition + bitOffset;

                            state.Break();
                        }
                    }
                );

                // not found:
                if (currentTextPositionData.Position < 0)
                {
                    currentTextPositionData.Position = encodedTextBits.Count;
                    encodedTextBits.AddRange(currentTextPositionData.EncodedText.Value);
                }

                alreadyUsedStrings.Add(stringToAdd);
            }
            return GetByteArrayFromBitList(encodedTextBits);
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

            return GetByteArrayFromBitList(allBits);
        }

        /// <summary>
        /// Returns a byte array from the bit list
        /// </summary>
        /// <param name="bitList"></param>
        private static byte[] GetByteArrayFromBitList(List<bool> bitList)
        {

            // Bytesize needs to be multiples of 4 bytes long!
            int byteSize = (bitList.Count + 7) / 8;

            // next 4 bytesize alingment -> + 3 to get to or over the next 4 byte thershold, then null out the last 2 bits / ( dec 3 ) for the actual size.
            byteSize = (byteSize + 3) & ~3;

            BitArray ba = new BitArray(bitList.ToArray());

            byte[] byteArray = new byte[byteSize];
            ba.CopyTo(byteArray, 0);


            return byteArray;
        }

        /// <summary>
        /// Checks whether the strings to check include only characters that are included in the given supported char list.
        /// Assumes that the given list of characters is ordered numerically.
        /// </summary>
        /// <param name="stringsToCheck"></param>
        /// <param name="allSupportedCharacters">The list of chars supported by the encoding, ordered by their numeric value</param>
        /// <param name="firstMiss">if this returns false, then this character is the first one found missing in the list of supported characters</param>
        /// <returns>true if all string characters are included in the supported char list</returns>
        public static bool IncludesOnlySupportedCharacters(IEnumerable<string> stringsToCheck, List<char> allSupportedCharacters, out char firstMiss)
        {
            firstMiss = (char)0;
            bool printVerificationTexts = Config.Get(BiowareLocalizationPluginOptions.PRINT_VERIFICATION_TEXTS, false, ConfigScope.Game);

            HashSet<char> allCharsToCheck = new HashSet<char>();
            foreach (string stringToCheck in stringsToCheck)
            {
                allCharsToCheck.UnionWith(stringToCheck.AsEnumerable());
            }

            // the list of supported characters must be sorted in ascending order for this to work
            // the getLeaf chars method herein does this now per default

            foreach (char toCheck in allCharsToCheck)
            {
                if (!IsCharInOrderedListOfChars(toCheck, allSupportedCharacters, printVerificationTexts))
                {
                    firstMiss = toCheck;
                    return false;
                }
            }

            // all chars found
            return true;
        }

        private static bool IsCharInOrderedListOfChars(char toCheck, List<char> allSupportedCharacters, bool printVerificationText)
        {
            foreach (char supported in allSupportedCharacters)
            {
                if (supported == toCheck)
                {
                    // found it, no need to search further
                    return true;
                }
                if (supported > toCheck)
                {
                    // already past the point where it should have been found
                    if (printVerificationText)
                    {
                        LogMissingCharacterWarning(toCheck, string.Format("before reaching char <{0} / u{1}>", supported, (int)supported), allSupportedCharacters);
                    }
                    return false;
                }
            }
            // char not found in supported chars
            if (printVerificationText)
            {
                LogMissingCharacterWarning(toCheck, "in all the supported characters", allSupportedCharacters);
            }
            return false;
        }

        private static void LogMissingCharacterWarning(char missingChar, String intermediateMessage, List<char> supportedChars)
        {
            App.Logger.LogWarning("Did not find char <{0} / u{1}> {2}", missingChar, (int)missingChar, intermediateMessage);
            App.Logger.LogWarning("List of supported chars: [{0}]", String.Join(", ", supportedChars.Select(c => c.ToString()).ToArray()));
            App.Logger.LogWarning("List of supported chars values: [{0}]", String.Join(", ", supportedChars.Select(c => ((int)c).ToString()).ToArray()));
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
            foreach (string text in texts)
            {
                foreach (char c in text)
                {
                    if (charNumbers.TryGetValue(c, out int occurences))
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
            foreach (var entry in charNumbers)
            {
                char c = entry.Key;
                nodeList.Add(new HuffManConstructionNode()
                {
                    Value = ~(uint)c,
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

                HuffManConstructionNode composite = new HuffManConstructionNode()
                {
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

            byte[] textBytes = UpdatePositionsAndCreateTextBytes(dictionaryOfEncodedTexts, uniqueTextPositions);

            ///* enable this for testing and debugging */
            //ResourceTestUtils.VerifyTextPositions(uniqueTextPositions.Values);

            SortedDictionary<TextID, EncodedTextPosition> primaryTextsSortedById = MapEncodedTextPositionById(encodedPrimaryTexts, uniqueTextPositions);

            List<SortedDictionary<TextID, EncodedTextPosition>> encodedDeclinatedArticleTextsById = new List<SortedDictionary<TextID, EncodedTextPosition>>();
            foreach (var idMappedText in encodedDeclinatedArticleTexts)
            {
                SortedDictionary<TextID, EncodedTextPosition> encodedTextsById = MapEncodedTextPositionById(idMappedText, uniqueTextPositions);
                encodedDeclinatedArticleTextsById.Add(encodedTextsById);
            }

            return new EncodedTextPositionGrouping(primaryTextsSortedById, encodedDeclinatedArticleTextsById, textBytes);
        }

        public static EncodedTextPositionGrouping GetEncodedTextsWhileReusingStringData(
            List<HuffmanNode> huffmanNodeList,
            List<LocalizedStringWithId> primaryExistingTexts,
            DragonAgeDeclinatedAdjectiveTuples dragonAgeDeclinatedAdjectives,
            ModifiedLocalizationResource alterations,
            byte[] originalData
            )
        {

            Dictionary<char, List<bool>> encoding = ResourceUtils.GetCharEncoding(huffmanNodeList);

            // first get the default entries

            SortedDictionary<TextID, EncodedTextPosition> primaryTextIdsAndPositions = new SortedDictionary<TextID, EncodedTextPosition>();
            foreach (var entry in primaryExistingTexts)
            {
                primaryTextIdsAndPositions[new TextID(entry.Id)] = FromLocalizedTextPosition(entry, encoding);
            }

            List<SortedDictionary<TextID, EncodedTextPosition>> declinatedAdjectivesIdsAndPositions = new List<SortedDictionary<TextID, EncodedTextPosition>>();
            for (int i = 0; i < dragonAgeDeclinatedAdjectives.NumberOfDeclinations; i++)
            {
                SortedDictionary<TextID, EncodedTextPosition> adjectiveIdsAndPositionsOfDeclination = new SortedDictionary<TextID, EncodedTextPosition>();
                declinatedAdjectivesIdsAndPositions.Add(adjectiveIdsAndPositionsOfDeclination);
                foreach (var entry in dragonAgeDeclinatedAdjectives.GetAdjectivesOfDeclination(i))
                {
                    adjectiveIdsAndPositionsOfDeclination[new TextID(entry.Id)] = FromLocalizedTextPosition(entry, encoding);
                }
            }

            // now add in the modifications...
            List<SortedDictionary<uint, string>> allEditsGroupedTextsById = new List<SortedDictionary<uint, string>>();
            allEditsGroupedTextsById.Add(new SortedDictionary<uint, string>(alterations.AlteredTexts));

            if (alterations.AlteredDeclinatedCraftingAdjectives.Count > 0)
            {
                // assume that only the specified number of declinations exists...
                for (int i = 0; i < dragonAgeDeclinatedAdjectives.NumberOfDeclinations; i++)
                {
                    allEditsGroupedTextsById.Add(new SortedDictionary<uint, string>());
                }

                // add the actual entries
                foreach (var alteredDeclination in alterations.AlteredDeclinatedCraftingAdjectives)
                {
                    uint id = alteredDeclination.Key;
                    List<string> declinations = alteredDeclination.Value;

                    for (int declinationNumber = 0; declinationNumber < declinations.Count; declinationNumber++)
                    {
                        string declination = declinations[declinationNumber];
                        SortedDictionary<uint, string> declinationsTexts = allEditsGroupedTextsById[declinationNumber + 1];
                        declinationsTexts[id] = declination;
                    }
                }
            }

            EncodedTextPositionGrouping onlyEdits = GetEncodedTextsToWrite(allEditsGroupedTextsById, encoding);

            int offset = originalData.Length * 8;

            foreach (var entry in onlyEdits.PrimaryTextIdsAndPositions)
            {
                TextID id = entry.Key;
                EncodedTextPosition textPosition = entry.Value;
                textPosition.Position = textPosition.Position + offset;

                primaryTextIdsAndPositions[id] = textPosition;
            }

            byte[] stringBytesToWrite = new byte[originalData.Length + onlyEdits.TextBytes.Length];

            Array.Copy(originalData, 0, stringBytesToWrite, 0, originalData.Length);
            Array.Copy(onlyEdits.TextBytes, 0, stringBytesToWrite, originalData.Length, onlyEdits.TextBytes.Length);

            return new EncodedTextPositionGrouping(primaryTextIdsAndPositions, declinatedAdjectivesIdsAndPositions, stringBytesToWrite);
        }

        /// <summary>
        /// Creates a new EncodedTextPosition entry for the given LocalizedString entry
        /// </summary>
        /// <param name="aLocalizedString"></param>
        /// <param name="encoding"></param>
        /// <returns>EncodedTextPosition</returns>
        private static EncodedTextPosition FromLocalizedTextPosition(LocalizedString aLocalizedString, Dictionary<char, List<bool>> encoding)
        {
            EncodedText et = new EncodedText(GetEncodedText(aLocalizedString.Value, encoding));
            return new EncodedTextPosition(et)
            {
                Position = aLocalizedString.DefaultPosition
            };
        }


        /// <summary>
        /// Different variants how to generate the bit array for the given data.
        /// </summary>
        private enum WriteVariant
        {
            /// <summary>
            /// Default - take the bits as they come and append to the end of the bit list. Fastest, but largest generated resource size.
            /// </summary>
            DEFAULT,

            /// <summary>
            /// Try to find the bit overlap by checking each bit. _Extremely_ slow, very taxing on hardware, but smallest created resource size.
            /// </summary>
            BIT_OVERLAP,

            /// <summary>
            /// Try to find a bit overlap by finding previous texts that end with the current text. Very slow, slightly smaller resource size than default.
            /// </summary>
            STRING_OVERLAP,

            /// <summary>
            /// Same as default, but it first writes smaller encoded texts and the largest last. Slightly slower than default, same largest generated resource size.
            /// </summary>
            BIT_LENGHT_ORDERED_DEFAULT
        };

        // none of the variants make a difference w.r.t. disappearing texts, just use the fastest one.
        private static readonly WriteVariant writeVariant = WriteVariant.DEFAULT;
        private static byte[] UpdatePositionsAndCreateTextBytes(Dictionary<string, EncodedText> dictionaryOfEncodedTexts, Dictionary<EncodedText, EncodedTextPosition> uniqueTextPositions)
        {

            byte[] textBytes;
            switch (writeVariant)
            {

                case WriteVariant.DEFAULT:
                    // Calculate the actual bit offsets for the texts
                    int currentTextPosition = 0;
                    IEnumerable<EncodedTextPosition> allEncodedTextPositions = uniqueTextPositions.Values;
                    foreach (EncodedTextPosition textPosition in allEncodedTextPositions)
                    {
                        currentTextPosition = UpdateTextAndGetNextTextPosition(currentTextPosition, textPosition);
                    }
                    // this sorted set can only be created after the positions are set!
                    var uniqueTextsWithPosition = new SortedSet<EncodedTextPosition>(allEncodedTextPositions);
                    textBytes = GetTextRepresentationToWrite(uniqueTextsWithPosition);
                    break;

                case WriteVariant.BIT_OVERLAP:
                    App.Logger.LogWarning("Using experimantal extremely slow bitwise overlap comparison method for writing byte array!");

                    List<EncodedTextPosition> textsSortedByEncodedLength = uniqueTextPositions.Values.ToList();

                    textsSortedByEncodedLength.Sort((a, b) =>
                    {
                        int lengthCompare = b.EncodedText.GetLength().CompareTo(a.EncodedText.GetLength());
                        if (lengthCompare != 0) return lengthCompare;
                        return a.GetHashCode().CompareTo(b.GetHashCode());
                    });

                    List<bool> textBits = new List<bool>();
                    foreach (EncodedTextPosition textPosition in textsSortedByEncodedLength)
                    {
                        FindTextPositionWithOverlapp(textPosition, textBits);
                    }
                    textBytes = GetByteArrayFromBitList(textBits);
                    break;

                case WriteVariant.STRING_OVERLAP:
                    App.Logger.LogWarning("Using experimantal very slow method for String comparison overerlaping byte array!");
                    textBytes = getTextSizedOrderedByteArrayWithOverlap(dictionaryOfEncodedTexts, uniqueTextPositions);
                    break;

                case WriteVariant.BIT_LENGHT_ORDERED_DEFAULT:
                    // Calculate the actual bit offsets for the texts
                    currentTextPosition = 0;

                    List<EncodedTextPosition> allEncodedTextPositionsSortedByLength = uniqueTextPositions.Values.ToList();
                    allEncodedTextPositionsSortedByLength.Sort((a, b) =>
                    {
                        // this looks the same as in the bit comparison case, but is reverse ordered
                        int lengthCompare = a.EncodedText.GetLength().CompareTo(b.EncodedText.GetLength());
                        if (lengthCompare != 0) return lengthCompare;
                        return a.GetHashCode().CompareTo(b.GetHashCode());
                    });

                    foreach (EncodedTextPosition textPosition in allEncodedTextPositionsSortedByLength)
                    {
                        currentTextPosition = UpdateTextAndGetNextTextPosition(currentTextPosition, textPosition);
                    }
                    // this sorted set can only be created after the positions are set!
                    uniqueTextsWithPosition = new SortedSet<EncodedTextPosition>(allEncodedTextPositionsSortedByLength);
                    textBytes = GetTextRepresentationToWrite(uniqueTextsWithPosition);
                    break;

                default:
                    throw new ArgumentException("Invalid textwrite variant selected!");
            }

            if (Config.Get(BiowareLocalizationPluginOptions.PRINT_VERIFICATION_TEXTS, false, ConfigScope.Game))
            {
                App.Logger.Log("Using Variant <{0}> the encoded text size was <{1}> bytes", writeVariant, textBytes.Length);
            }

            return textBytes;
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
                    dictionaryOfEncodedTexts[text] = encodedText;
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
        private static SortedDictionary<TextID, EncodedTextPosition> MapEncodedTextPositionById(
            IDictionary<uint, EncodedText> encodedTexts,
            IDictionary<EncodedText, EncodedTextPosition> uniqueTextPositions)
        {
            SortedDictionary<TextID, EncodedTextPosition> textsSortedById = new SortedDictionary<TextID, EncodedTextPosition>();
            foreach (KeyValuePair<uint, EncodedText> entry in encodedTexts)
            {
                TextID id = new TextID(entry.Key);
                textsSortedById.Add(id, uniqueTextPositions[entry.Value]);
            }

            return textsSortedById;
        }

        /// <summary>
        /// Using Frostys NativeWriter / Reader to persist texts in the mod format does break certain non ascii characters (even though unicode utf-8 is used...?).
        /// To circumvent that we write the texts ourselves, guaranteed in ut-8
        /// </summary>
        /// <param name="textEntriesToWrite"></param>
        /// <returns></returns>
        public static byte[] ConvertTextEntriesToBytes(Dictionary<uint, string> textEntriesToWrite)
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

                        foreach (string declination in declinationsList)
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
            using (BinaryReader reader = new BinaryReader(new MemoryStream(parseable), Encoding.UTF8))
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

            int offset = (int)(reader.Position - position);

            reader.Position = position;
            byte[] modTextBytes = reader.ReadBytes(stringLength + offset);

            return ConvertBytesToString(modTextBytes);
        }
    }
}
