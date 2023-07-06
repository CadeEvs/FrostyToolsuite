using Frosty.Sdk.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Frosty.Sdk.Utils;

/// <summary>
/// A node in the huffman coding scheme
/// </summary>
internal class HuffmanNode : IComparable<HuffmanNode>
{
    public bool IsLeaf => Left == null && Right == null;
    public char Letter => (char)(~Value);

    public uint Value;
    public HuffmanNode? Left { get; private set; }
    public HuffmanNode? Right { get; private set; }

    public HuffmanNode? Parent { get; private set; }

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
        Left = leftNode;
        Left.Parent = this;
    }

    public void SetRightNode(HuffmanNode rightNode)
    {
        Right = rightNode;
        Right.Parent = this;
    }

    public override string ToString()
    {
        string printLetter = Value switch
        {
            uint.MaxValue => "endDelimiter",
            4294967285 => "newLine",
            _ => Letter.ToString(),
        };
        return $"[Value = <{Value}> | Letter = <{printLetter}>]";
    }

    /// <summary>
    /// Returns the bit representation of this node, to be used in tests.
    /// </summary>
    /// <returns>The bit representation of this node.</returns>
    public string GetBitRepresentation()
    {
        if (Parent == null)
        {
            return "";
        }

        string bitVal;
        if (this == Parent.Left)
        {
            bitVal = "0";
        }
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

        List<HuffmanNode> nodes = new();
        uint nodeValue = 0;

        for (int i = 0; i < count; i++)
        {
            HuffmanNode n = new(stream, endian);
            int idx = nodes.FindIndex(a => a.Value == n.Value);
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
    /// <param name="stream">The <see cref="DataStream"/> the encoded data gets read from.</param>
    /// <param name="integerCount">The number of <see cref="int"/>s the data contains.</param>
    /// <param name="endian">The <see cref="Endian"/> in which the encoded data gets read.</param>
    public void ReadEncodedData(DataStream stream, uint integerCount, Endian endian = Endian.Little)
    {
        m_data = new int[integerCount];

        for (int i = 0; i < integerCount; i++)
        {
            m_data[i] = stream.ReadInt32(endian);
        }
    }

    /// <summary>
    /// Reads in the encoded data of the given byte length, for cases that the data to read does not match integer sizes.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> the encoded data gets read from.</param>
    /// <param name="byteCount">The number of <see cref="byte"/>s to read.</param>
    /// <param name="endian">The <see cref="Endian"/> in which the encoded data gets read.</param>
    public void ReadOddSizedEncodedData(DataStream stream, uint byteCount, Endian endian = Endian.Little)
    {


        uint intLength = byteCount / 4;

        m_data = new int[intLength + 1];

        for (int i = 0; i < intLength; i++)
        {
            m_data[i] = stream.ReadInt32(endian);
        }

        // read remaining bytes as full int
        byte[] remaining = new byte[4];
        stream.ReadBytes((int)byteCount % 4).CopyTo(remaining, 0);

        bool switchBytes = (endian == Endian.Little && !BitConverter.IsLittleEndian) || (endian == Endian.Big && BitConverter.IsLittleEndian);
        if (switchBytes)
        {
            remaining = remaining.Reverse().ToArray();
        }
        // might be better to replace this with the same method used in the DataStream readInt method
        m_data[intLength] = BitConverter.ToInt32(remaining);
    }

    /// <summary>
    /// Reads a Huffman encoded string.
    /// </summary>
    /// <param name="bitIndex">The index of the bit at which the encoded string starts.</param>
    /// <returns>The Huffman decoded string.</returns>
    /// <exception cref="InvalidDataException">Is thrown when the root node or encoded data were not read in before.</exception>
    public string ReadHuffmanEncodedString(int bitIndex)
    {
        if (m_rootNode == null || m_data == null)
        {
            string rootNodeMissingErrorPart = m_rootNode != null ? "" : " The root node was not established by calling 'ReadHuffmanTable'!";
            string dataMissingErrorPart = m_data != null ? "" : " No huffman encoded data was read with 'ReadEncodedData'!";
            throw new InvalidDataException(string.Format("HuffmanDecoder state is not initialized!{0}{1}", rootNodeMissingErrorPart, dataMissingErrorPart));
        }

        int dataLengthInBits = m_data.Length * 32;

        StringBuilder sb = new();
        while (true)
        {
            HuffmanNode node = m_rootNode;

            while (!node.IsLeaf && bitIndex < dataLengthInBits)
            {
                int bit = (m_data[bitIndex / 32] >> (bitIndex % 32)) & 1;
                if (bit == 0)
                {
                    node = node.Left!;
                }
                else
                {
                    node = node.Right!;
                }
                bitIndex++;
            }

            if (node.Letter == 0x00
                // should probably throw an exception instead or at least print a warning when the bit index is outside the read data array
                || bitIndex >= dataLengthInBits)
            {
                return sb.ToString();
            }

            sb.Append(node.Letter);
        }
    }

    public void Dispose()
    {
        m_rootNode = null;
        m_data = null;
    }
}