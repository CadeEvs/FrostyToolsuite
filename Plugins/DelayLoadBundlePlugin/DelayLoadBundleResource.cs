using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DelayLoadBundlePlugin
{
    public class HuffmanNode
    {
        public char Letter => (char)(~value);
        public uint Value => (uint)(~value);
        public object Data { get; set; }

        public uint value;
        public HuffmanNode left;
        public HuffmanNode right;
    }

    public class PartitionBundle
    {
        public DelayLoadBundle Bundle
        {
            get => bundle;
            internal set => bundle = value;
        }
        public Guid Guid => guid;
        public uint Hash => hash;

        private DelayLoadBundle bundle;
        private Guid guid;
        private uint hash;

        public PartitionBundle(Guid inGuid, uint inHash)
        {
            guid = inGuid;
            hash = inHash;
        }
    }

    public class DelayLoadBundle
    {
        public string Name
        {
            get => name;
            internal set => name = value;
        }
        public uint Hash => hash;

        private string name;
        private int unknown;
        private uint hash;

        public DelayLoadBundle(int inUnk, uint inHash)
        {
            unknown = inUnk;
            hash = inHash;
        }
    }

    public class DelayLoadBundleResource : Resource
    {
        private List<PartitionBundle> partitionBundles = new List<PartitionBundle>();
        private List<DelayLoadBundle> bundles = new List<DelayLoadBundle>();

        public DelayLoadBundleResource()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            List<int> stringIds = new List<int>();
            List<byte> lengths = new List<byte>();
            List<int> offsets = new List<int>();

            List<Tuple<uint, uint>> offsetAndCounts = new List<Tuple<uint, uint>>();
            for (int i = 0; i < 4; i++)
            {
                offsetAndCounts.Add(new Tuple<uint, uint>
                (
                    reader.ReadUInt(),
                    reader.ReadUInt()
                ));
            }

            List<Tuple<uint, uint, uint>> huffmanOffsets = new List<Tuple<uint, uint, uint>>();
            for (int i = 0; i < 2; i++)
            {
                huffmanOffsets.Add(new Tuple<uint, uint, uint>
                (
                    reader.ReadUInt(),
                    reader.ReadUInt(),
                    reader.ReadUInt()
                ));
            }

            offsetAndCounts.Add(new Tuple<uint, uint>
            (
                reader.ReadUInt(),
                reader.ReadUInt()
            ));

            uint endOffset = reader.ReadUInt();

            reader.Position = offsetAndCounts[0].Item1;
            for (int i = 0; i < offsetAndCounts[0].Item2; i++)
            {
                int stringId = reader.ReadInt();
                Guid guid = reader.ReadGuid();
                uint hash = reader.ReadUInt();

                partitionBundles.Add(new PartitionBundle(guid, hash));
                stringIds.Add(stringId);
            }

            reader.Position = offsetAndCounts[1].Item1;
            for (int i = 0; i < offsetAndCounts[1].Item2; i++)
            {
                byte dirLength = reader.ReadByte();
                int strOffset = reader.ReadInt();
                reader.ReadBytes(3);
                int unknown = reader.ReadInt();
                uint hash = reader.ReadUInt();

                lengths.Add(dirLength);
                offsets.Add(strOffset);
                bundles.Add(new DelayLoadBundle(unknown, hash));
            }

            reader.Position = offsetAndCounts[2].Item1;
            List<ulong> unknown3 = new List<ulong>();
            for (int i = 0; i < offsetAndCounts[2].Item2; i++)
            {
                unknown3.Add(reader.ReadULong());
            }

            reader.Position = offsetAndCounts[3].Item1;
            List<uint> unknown4 = new List<uint>();
            for (int i = 0; i < offsetAndCounts[3].Item2; i++)
            {
                unknown4.Add(reader.ReadUInt());
            }

            reader.Position = huffmanOffsets[0].Item1;
            HuffmanNode rootNode1 = ReadNodes(reader, huffmanOffsets[0].Item2);
            reader.Position = huffmanOffsets[0].Item3;
            byte[] data = reader.ReadBytes((int)(huffmanOffsets[1].Item1 - reader.Position));

            reader.Position = huffmanOffsets[1].Item1;
            HuffmanNode rootNode2 = ReadNodes(reader, huffmanOffsets[1].Item2);
            reader.Position = huffmanOffsets[1].Item3;
            List<string> strings2 = ReadPaths(reader, rootNode2, (int)(offsetAndCounts[4].Item1 - reader.Position), lengths, offsets, rootNode1, data);

            for (int i = 0; i < bundles.Count; i++)
                bundles[i].Name = strings2[i];
            for (int i = 0; i < partitionBundles.Count; i++)
                partitionBundles[i].Bundle = bundles[stringIds[i]];

            reader.Position = offsetAndCounts[4].Item1;
            List<uint> unknown5 = new List<uint>();
            for (int i = 0; i < offsetAndCounts[4].Item2; i++)
            {
                unknown5.Add(reader.ReadUInt());
            }
        }

        public IEnumerable<DelayLoadBundle> EnumerateBundles()
        {
            for (int i = 0; i < bundles.Count; i++)
                yield return bundles[i];
        }

        private HuffmanNode ReadNodes(NativeReader reader, uint nodeCount)
        {
            HuffmanNode rootNode = null;
            HuffmanNode leftNode = null;
            HuffmanNode rightNode = null;

            List<HuffmanNode> nodes = new List<HuffmanNode>();
            int nodeValue = 0;
            int z = 0;

            for (int i = 0; i < nodeCount; i++)
            {
                HuffmanNode n = new HuffmanNode() { value = reader.ReadUInt() };
                int idx = nodes.FindIndex((HuffmanNode a) => { return a.value == n.value; });
                if (idx != -1)
                    n = nodes[idx];

                if (leftNode == null) leftNode = n;
                else if (rightNode == null)
                {
                    rightNode = n;
                    if (idx == -1)
                        nodes.Add(rightNode);

                    n = new HuffmanNode
                    {
                        value = (uint)nodeValue++,
                        left = leftNode,
                        right = rightNode
                    };
                    rootNode = n;

                    leftNode = null;
                    rightNode = null;
                    idx = -1;
                }

                if (idx == -1)
                    nodes.Add(n);
            }

            return rootNode;
        }

        private string ReadString(byte[] data, HuffmanNode rootNode, int offset)
        {
            using (BitReader bitReader = new BitReader(new MemoryStream(data)))
            {
                StringBuilder sb = new StringBuilder();
                bitReader.SetPosition(offset);

                while (true)
                {
                    HuffmanNode n = rootNode;
                    while (n.left != null && n.right != null)
                    {
                        bool b = bitReader.GetBit();
                        if (bitReader.EndOfStream)
                            break;

                        if (b) n = n.right;
                        else n = n.left;
                    }

                    if (n.Letter == 0x00)
                        break;
                    else
                        sb.Append(n.Letter);

                    if (bitReader.EndOfStream)
                        break;
                }

                return sb.ToString();
            }
        }

        private List<string> ReadPaths(NativeReader reader, HuffmanNode rootNode, int length, List<byte> lengths, List<int> offsets, HuffmanNode stringRootNode, byte[] stringData)
        {
            List<string> strings = new List<string>();

            byte[] values = reader.ReadBytes(length);
            int z = 0;

            using (BitReader bitReader = new BitReader(new MemoryStream(values)))
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lengths.Count; i++)
                {
                    bitReader.SetPosition(offsets[i]);
                    z = 0;

                    while (true)
                    {
                        HuffmanNode n = rootNode;
                        while (n.left != null && n.right != null)
                        {
                            bool b = bitReader.GetBit();
                            if (bitReader.EndOfStream)
                                break;

                            if (b) n = n.right;
                            else n = n.left;
                        }

                        sb.Append(ReadString(stringData, stringRootNode, (int)n.Value) + "/");
                        z++;

                        if (z >= lengths[i])
                        {
                            strings.Add(sb.ToString().Trim('/'));
                            sb.Clear();
                            break;
                        }

                        if (bitReader.EndOfStream)
                            break;
                    }
                }
            }

            return strings;
        }
    }
}
