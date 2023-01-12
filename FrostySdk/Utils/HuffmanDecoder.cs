using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk
{
    internal class HuffmanNode
    {
        public char Character => (char)(~m_value);
        public uint Value => m_value;
        public bool IsLeaf => LeftNode == null && RightNode == null;

        public HuffmanNode LeftNode;
        public HuffmanNode RightNode;

        private uint m_value;

        public HuffmanNode(uint inValue, HuffmanNode inLeftNode, HuffmanNode inRightNode)
        {
            m_value = inValue;
            LeftNode = inLeftNode;
            RightNode = inRightNode;
        }

        public HuffmanNode(NativeReader reader, Endian endian)
        {
            m_value = reader.ReadUInt(endian);
        }
    }

    public class HuffmanDecoder
    {
        /// <summary>
        /// Root Node of the Huffman Table.
        /// </summary>
        private HuffmanNode m_rootNode;

        private int[] m_data;

        /// <summary>
        /// Parses a Huffman Table.
        /// </summary>
        /// <param name="reader">The <see cref="NativeReader"/> the Huffman Table gets read from.</param>
        /// <param name="count">The number of Huffman Nodes the Table contains.</param>
        /// <param name="endian">The Endianess in which the Huffman Table gets read.</param>
        public void ReadHuffmanTable(NativeReader reader, uint count, Endian endian = Endian.Little)
        {
            m_rootNode = null;
            HuffmanNode leftNode = null;
            HuffmanNode rightNode = null;

            List<HuffmanNode> nodes = new List<HuffmanNode>();
            uint nodeValue = 0;

            for (int i = 0; i < count; i++)
            {
                HuffmanNode n = new HuffmanNode(reader, endian);
                int idx = nodes.FindIndex((HuffmanNode a) => { return a.Value == n.Value; });
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
        /// <param name="reader">The <see cref="NativeReader"/> the encoded data gets read from.</param>
        /// <param name="count">The number of <see cref="int"/>s the data contains.</param>
        /// <param name="endian">The Endianess in which the encoded data gets read.</param>
        public void ReadEncodedData(NativeReader reader, uint count, Endian endian = Endian.Little)
        {
            m_data = new int[count];

            for (int i = 0; i < count; i++)
            {
                m_data[i] = reader.ReadInt(endian);
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
                        node = node.LeftNode;
                    }
                    else
                    {
                        node = node.RightNode;
                    }
                    bitIndex++;
                }

                if (node.Character == 0x00)
                {
                    return sb.ToString();
                }
                else
                {
                    sb.Append(node.Character);
                }
            }
        }

        public void Dispose()
        {
            m_rootNode = null;
            m_data = null;
        }
    }
}
