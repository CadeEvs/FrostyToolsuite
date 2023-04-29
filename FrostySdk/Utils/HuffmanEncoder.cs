using Frosty.Sdk.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Frosty.Sdk.Utils;

/// <summary>
/// A <see cref="HuffmanNode"/> with additional integer for how many times this node was encountered in the data to encode. This is used to construct the huffman tree.
/// </summary>
internal class HuffManConstructionNode : HuffmanNode, IComparable<HuffManConstructionNode>
{
    public int Occurences { get; set; }

    public new HuffManConstructionNode? Left { get; private set; }

    public new HuffManConstructionNode? Right { get; private set; }

    public HuffManConstructionNode()
    {
        Occurences = 0;
    }

    public HuffManConstructionNode(char inValueChar, int inOccurences)
    {
        Value = ~(uint)inValueChar;
        Occurences = inOccurences;
    }

    public void SetLeftNode(HuffManConstructionNode leftNode)
    {
        base.SetLeftNode(leftNode);
        Left = leftNode;
        Occurences += leftNode.Occurences;
    }

    public void SetRightNode(HuffManConstructionNode rightNode)
    {
        base.SetRightNode(rightNode);
        Right = rightNode;
        Occurences += rightNode.Occurences;
    }

    public int CompareTo(HuffManConstructionNode? other)
    {
        int cmp = Occurences.CompareTo(other?.Occurences);
        if (cmp == 0)
        {
            cmp = GetRemainingDepth().CompareTo(other?.GetRemainingDepth());
        }
        return cmp;
    }

    private int GetRemainingDepth()
    {
        int ld = Left != null ? Left.GetRemainingDepth() : 0;
        int rd = Right != null ? Right.GetRemainingDepth() : 0;

        return Math.Max(ld, rd);
    }
}

public class HuffmanEncoder
{

    private HuffManConstructionNode? m_rootNode;

    private IDictionary<char, IList<bool>> m_characterEncoding = new Dictionary<char, IList<bool>>();

    /// <summary>
    /// Uses the given input to construct the huffman encoding table (or tree). The created encoding includes end delimemeter character (char 0x0) with a number of occurences of the number of given strings.
    /// Also temporarily stores the given texts to use them in other methods if the need arises.
    /// Returns the uint representation of the huffman encoding.
    /// </summary>
    /// <param name="texts">The texts to encode, or a suitable approximation of the characters appearances.</param>
    /// <returns>The list of huffman node values in the order they should be written.</returns>
    public IList<uint> BuildHuffmanEncodingTree(IEnumerable<string> texts)
    {
        IList<string> strings = texts.ToList();
        m_rootNode = CalculateHuffmanEncoding(strings);

        IList<HuffmanNode> encodingNodes = GetNodeListToWrite(m_rootNode);
        m_characterEncoding = GetCharEncoding(encodingNodes);

        return encodingNodes.Select(node=>node.Value).ToList();
    }

    /// <summary>
    /// Encodes the given text into a bool values, using the encoding set previously by <see cref="HuffmanEncoder.BuildHuffmanEncodingTree(IEnumerable{string})"/>.
    /// </summary>
    /// <param name="text">A single text to encode.</param>
    /// <param name="includeEndDelimeter">Whether or not the encoding for a 0x0 character should be added to the end of the text.</param>
    /// <returns>the bool representation of the encoded text.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">If a character or symbol to encode was not found in the dictionary</exception>
    public IList<bool> EncodeText(string text, bool includeEndDelimeter = true)
    {
        return GetEncodedText(text, m_characterEncoding, includeEndDelimeter);
    }

    public Span<byte> WriteAllEncodedTexts(IEnumerable<Tuple<object, string>> textsPerIdentifier,Endian endian = Endian.Little, bool compressResults = false)
    {

        List<Tuple<object, int>> positionsOfStrings = new();
        List<bool> encodedTexts = new();

        foreach (var textWithIdentifier in textsPerIdentifier)
        {
            int position = encodedTexts.Count;

            positionsOfStrings.Add( new Tuple<object, int>(textWithIdentifier.Item1, position));

            // TODO enable compression and re use of already existing sub lists
            encodedTexts.AddRange(EncodeText(textWithIdentifier.Item2));
        }


        // TODO implement me!
        return null;
    }

    public void Dispose()
    {
        // TODO implement me!
    }

    /// <summary>
    /// Calculates the huffman encoding for the given texts, and returns the root node of the resulting tree.
    /// </summary>
    /// <param name="texts"></param>
    /// <returns>Huffman root node.</returns>
    private static HuffManConstructionNode CalculateHuffmanEncoding(IEnumerable<string> texts)
    {

        // get set of chars and their number of occurences...
        Dictionary<char, int> charNumbers = new();
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

        List<HuffManConstructionNode> nodeList = new();
        foreach (var entry in charNumbers)
        {
            nodeList.Add(new HuffManConstructionNode(entry.Key, entry.Value));
        }

        uint nodeValue = 0;
        while (nodeList.Count > 1)
        {

            nodeList.Sort();

            HuffManConstructionNode left = nodeList[0];
            HuffManConstructionNode right = nodeList[1];

            nodeList.RemoveRange(0, 2);

            HuffManConstructionNode composite = new()
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
    /// Returns a dictionary of characters to their encoded bit representation.
    /// </summary>
    /// <param name="encodingNodes">The (limited) list of Huffman code nodes used.</param>
    /// <returns>Dictionary of char - code values</returns>
    private static IDictionary<char, IList<bool>> GetCharEncoding(IList<HuffmanNode> encodingNodes)
    {

        Dictionary<char, IList<bool>> charEncodings = new();

        foreach (HuffmanNode node in encodingNodes)
        {
            if (node.Left == null && node.Right == null)
            {
                char c = node.Letter;
                IList<bool> path = GetCharEncodingRecursive(node);

                charEncodings.Add(c, path);
            }
        }
        return charEncodings;
    }

    /// <summary>
    /// Recalculates the nodelist to write to the resource based on the root node.
    /// This method returns a flat list or huffman nodes, that do not include the given root node.
    /// </summary>
    /// <param name="rootNode">the root node of the tree to flatten into a single list.</param>
    /// <returns>list of nodes in the order to write</returns>
    private static IList<HuffmanNode> GetNodeListToWrite(HuffmanNode rootNode)
    {
        List<HuffmanNode> nodesSansRoot = new();

        if (rootNode == null)
        {
            return nodesSansRoot;
        }

        // get all branches
        List<HuffmanNode> branches = GetAllBranchNodes(new List<HuffmanNode>() { rootNode });

        // sort branches by their value, so that the write out can happen in the correct order
        branches.Sort();

        // add all the children in the order of their parent's value
        foreach (HuffmanNode branch in branches)
        {
            nodesSansRoot.Add(branch.Left!);
            nodesSansRoot.Add(branch.Right!);
        }

        return nodesSansRoot;
    }

    private static List<HuffmanNode> GetAllBranchNodes(List<HuffmanNode> currentNodes)
    {
        List<HuffmanNode> branchNodes = new();

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
    /// Return the encoding for the given node as path in the tree.
    /// </summary>
    /// <param name="node">The node for which to find the encoding.</param>
    /// <returns>the encoding as list of bools.</returns>
    private static IList<bool> GetCharEncodingRecursive(HuffmanNode node)
    {
        HuffmanNode? parent = node.Parent;
        if (parent == null)
        {
            return new List<bool>();
        }

        IList<bool> encoding = GetCharEncodingRecursive(parent);

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
                    "Trying to find encoding for node <{0}> failed due to incorrectly setup encoding tree!",
                    node.ToString()));
        }

        return encoding;
    }

    /// <summary>
    /// Returns the bit encoded text as list of bools.
    /// The end of the text is marked with the delimiter character 0x0 / huffman node value = uint.MaxValue.
    /// </summary>
    /// <param name="toEncode">The text to encode, given as char array or similar char enumerable</param>
    /// <param name="charEncoding">The character encoding to use for the text.</param>
    /// <param name="includeEndDelimeter">Whether or not to include the end delimeter in the returned encoding.</param>
    /// <returns>The encoded text.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">If a character or symbol to encode was not found in the dictionary</exception>
    private static IList<bool> GetEncodedText(IEnumerable<char> toEncode, IDictionary<char, IList<bool>> charEncoding, bool includeEndDelimeter)
    {
        List<bool> encodedText = new();

        foreach (char c in toEncode)
        {
            encodedText.AddRange(TryGetCharacterEncoding(c, charEncoding));
        }

        if(includeEndDelimeter)
        {
            char delimiter = (char)0x0;
            encodedText.AddRange(TryGetCharacterEncoding(delimiter, charEncoding));
        }

        return encodedText;
    }

    private static IList<bool> TryGetCharacterEncoding(char c, IDictionary<char, IList<bool>> charEncoding)
    {

        bool found = charEncoding.TryGetValue(c, out IList<bool>? encodedChar);
        if(found)
        {
            return encodedChar!;
        }

        if(c == 0x0)
        {
            throw new KeyNotFoundException("Encoding does not contain mapping for end delimiter!");
        }

        string errorMessage = string.Format("Encoding does not contain a mapping for symbol of value {0}: '{1}'!", (int)c, c);
        throw new KeyNotFoundException(errorMessage);
    }

}