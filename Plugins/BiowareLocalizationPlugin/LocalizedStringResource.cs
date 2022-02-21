using Frosty.Core;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BiowareLocalizationPlugin
{
    #region -- Classes --
    internal class HuffmanNode
    {
        public char Letter => (char)(~value);
        public uint Value => (uint)(~value);
        public object Data { get; set; }

        public uint value;
        public HuffmanNode left;
        public HuffmanNode right;
    }

    internal class LocalizedString
    {
        public uint Id;
        public string Value;

        public override string ToString()
        {
            return Value;
        }
    }
    #endregion

    public class LocalizedStringResource : Resource
    {
        public IEnumerable<KeyValuePair<uint, string>> Strings
        {
            get
            {
                for (int i = 0; i < strings.Count; i++)
                    yield return new KeyValuePair<uint, string>(strings[i].Id, strings[i].Value);
            }
        }
        private List<LocalizedString> strings = new List<LocalizedString>();

        public LocalizedStringResource()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
            {
                uint unk1 = reader.ReadUInt();
                uint unk2 = reader.ReadUInt();
                uint unk3 = reader.ReadUInt();

                long numStrings = reader.ReadLong();
                reader.Position += 0x18;

                Dictionary<uint, List<uint>> hashToStringIdMapping = new Dictionary<uint, List<uint>>();

                for (int i = 0; i < numStrings; i++)
                {
                    uint hash = reader.ReadUInt();
                    uint stringId = reader.ReadUInt();
                    reader.Position += 8;
                    if (!hashToStringIdMapping.ContainsKey(hash))
                        hashToStringIdMapping.Add(hash, new List<uint>());
                    hashToStringIdMapping[hash].Add(stringId);
                }

                reader.Position += 0x18;

                while (reader.Position < reader.Length)
                {
                    uint hash = reader.ReadUInt();
                    int stringLen = reader.ReadInt();
                    string str = reader.ReadSizedString(stringLen);

                    if (hashToStringIdMapping.ContainsKey(hash))
                    {
                        foreach (uint stringId in hashToStringIdMapping[hash])
                            strings.Add(new LocalizedString() { Id = stringId, Value = str });
                    }
                    else
                    {
                        App.Logger.Log("Cannot find {0} in {1}", hash.ToString("x8"), entry.Name);
                    }
                }

                return;
            }

            uint magic = reader.ReadUInt();
            if (magic != 0xd78b40eb)
                throw new InvalidDataException();

            reader.ReadUInt();
            uint dataOffset = reader.ReadUInt();
            reader.ReadUInt();
            reader.ReadUInt();
            reader.ReadUInt();

            uint nodeCount = reader.ReadUInt();
            uint nodeOffset = reader.ReadUInt();
            uint stringsCount = reader.ReadUInt();
            uint stringsOffset = reader.ReadUInt();

            uint[] unknownCounts = new uint[3];
            uint[] unknownOffsets = new uint[3];

            for (int i = 0; i < 3; i++)
            {
                if (i == 2 && unknownCounts[i - 1] == 0)
                    continue;

                unknownCounts[i] = reader.ReadUInt();
                unknownOffsets[i] = reader.ReadUInt();
            }

            int[] stringData = new int[stringsCount];

            HuffmanNode rootNode = ReadNodes(reader, nodeCount);
            ReadStringData(reader, stringsCount, stringData);

            reader.Position = dataOffset;
            ReadStrings(reader, rootNode, stringData);
        }

        private HuffmanNode ReadNodes(NativeReader reader, uint nodeCount)
        {
            HuffmanNode rootNode = null;
            HuffmanNode leftNode = null;
            HuffmanNode rightNode = null;

            List<HuffmanNode> nodes = new List<HuffmanNode>();
            int nodeValue = 0;

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

        private void ReadStringData(NativeReader reader, uint stringsCount, int[] stringData)
        {
            for (int i = 0; i < stringsCount; i++)
            {
                uint stringId = reader.ReadUInt();
                stringData[i] = reader.ReadInt();

                strings.Add(new LocalizedString() { Id = stringId });
            }
        }

        private void ReadStrings(NativeReader reader, HuffmanNode rootNode, int[] stringData)
        {
            byte[] values = reader.ReadBytes((int)(reader.Length - reader.Position));
            using (BitReader bitReader = new BitReader(new MemoryStream(values)))
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < strings.Count; index++)
                {
                    bitReader.SetPosition(stringData[index]);
                    while (true)
                    {
                        HuffmanNode n = rootNode;
                        while (n.left != null && n.right != null)
                        {
                            bool b = bitReader.GetBit();
                            if (b) n = n.right;
                            else n = n.left;
                        }

                        if (n.Letter == 0x00)
                        {
                            strings[index].Value = sb.ToString();
                            sb.Clear();
                            break;
                        }
                        else
                            sb.Append(n.Letter);
                    }
                }
            }
        }
    }
}
