using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Utils;

/// <summary>
/// A node in the huffman coding scheme
/// </summary>
internal class HuffmanNode : IComparable<HuffmanNode>
{
    public bool IsLeaf => Left == null && Right == null;
    public char Letter => (char)(~Value);

    public uint Value;
    public HuffmanNode Left { get; private set; }
    public HuffmanNode Right { get; private set; }

    public HuffmanNode Parent { get; private set; }

    public HuffmanNode()
    {
    }

    public HuffmanNode(uint inValue, HuffmanNode inLeft, HuffmanNode inRight)
    {
        Value = inValue;
        SetLeftNode(inLeft);
        SetRightNode(inRight);
    }

    public HuffmanNode(DataStream stream, Endian endian)
    {
        Value = stream.ReadUInt32(endian);
    }

    public void SetLeftNode(HuffmanNode leftNode)
    {
        this.Left = leftNode;
        Left.Parent = this;
    }

    public void SetRightNode(HuffmanNode rightNode)
    {
        this.Right = rightNode;
        Right.Parent = this;
    }

    public override string ToString()
    {
        string printLetter;

        switch (Value)
        {
            case uint.MaxValue:
                printLetter = "endDelimeter";
                break;
            case 4294967285:
                printLetter = "newLine";
                break;
            default:
                printLetter = Letter.ToString();
                break;
        }

        return string.Format("[Value = <{0}> | Letter = <{1}>]", Value.ToString(), printLetter);
    }

    /// <summary>
    /// Returns the bit representation of this node, to be used in tests.
    /// </summary>
    /// <returns></returns>
    public string GetBitRepresentation()
    {
        if (Parent == null)
        {
            return "";
        }

        string bitVal;
        if (this == Parent.Left)
            bitVal = "0";
        else if (this == Parent.Right)
        {
            bitVal = "1";
        }
        else
        {
            bitVal = "ERROR!";
        }
        return Parent.GetBitRepresentation() + bitVal;
    }

    public int CompareTo(HuffmanNode? other)
    {
        return Value.CompareTo(other?.Value);
    }
}

public class HuffmanDecoder
{
    /// <summary>
    /// Root Node of the Huffman Table.
    /// </summary>
    private HuffmanNode? m_rootNode;

    private int[]? m_data;

    /// <summary>
    /// Parses a Huffman Table.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> the Huffman Table gets read from.</param>
    /// <param name="count">The number of Huffman Nodes the Table contains.</param>
    /// <param name="endian">The Endianess in which the Huffman Table gets read.</param>
    public void ReadHuffmanTable(DataStream stream, uint count, Endian endian = Endian.Little)
    {
        m_rootNode = null;
        HuffmanNode? leftNode = null;
        HuffmanNode? rightNode = null;

        List<HuffmanNode> nodes = new List<HuffmanNode>();
        uint nodeValue = 0;

        for (int i = 0; i < count; i++)
        {
            HuffmanNode n = new HuffmanNode(stream, endian);
            int idx = nodes.FindIndex((HuffmanNode a) => a.Value == n.Value);
            if (idx != -1)
            {
                n = nodes[idx];
            }

            if (leftNode == null)
            {
                leftNode = n;
            }
            else if (rightNode == null)
            {
                rightNode = n;
                if (idx == -1)
                {
                    nodes.Add(rightNode);
                }

                n = new HuffmanNode(nodeValue++, leftNode, rightNode);
                m_rootNode = n;

                leftNode = null;
                rightNode = null;
                idx = -1;
            }

            if (idx == -1)
            {
                nodes.Add(n);
            }
        }
    }

    /// <summary>
    /// Reads in the encoded data.
    /// </summary>
    /// <param name="stream">The <see cref="NativeReader"/> the encoded data gets read from.</param>
    /// <param name="count">The number of <see cref="int"/>s the data contains.</param>
    /// <param name="endian">The Endianess in which the encoded data gets read.</param>
    public void ReadEncodedData(DataStream stream, uint count, Endian endian = Endian.Little)
    {
        m_data = new int[count];

        for (int i = 0; i < count; i++)
        {
            m_data[i] = stream.ReadInt32(endian);
        }
    }

    /// <summary>
    /// Reads a Huffman encoded string.
    /// </summary>
    /// <param name="bitIndex">The index of the bit at which the encoded string starts.</param>
    /// <returns>The Huffman decoded string.</returns>
    /// <exception cref="InvalidDataException">When the root node or encoded data were not read in before.</exception>
    public string ReadHuffmanEncodedString(int bitIndex)
    {
        if (m_rootNode == null || m_data == null)
        {
            throw new InvalidDataException();
        }

        StringBuilder sb = new StringBuilder();
        while (true)
        {
            HuffmanNode node = m_rootNode;

            while (!node.IsLeaf)
            {
                int bit = (m_data[bitIndex / 32] >> (bitIndex % 32)) & 1;
                if (bit == 0)
                {
                    node = node.Left;
                }
                else
                {
                    node = node.Right;
                }
                bitIndex++;
            }

            if (node.Letter == 0x00)
            {
                return sb.ToString();
            }
            else
            {
                sb.Append(node.Letter);
            }
        }
    }

    public void Dispose()
    {
        m_rootNode = null;
        m_data = null;
    }
}