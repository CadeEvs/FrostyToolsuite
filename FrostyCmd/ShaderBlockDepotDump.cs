using Frosty.Hash;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FrostySdk.Managers.Entries;

namespace FrostyCmd
{
    class BaseDepotEntry
    {
        public ulong Hash;
        public virtual void Read(NativeReader reader, AssetManager am)
        {
            Hash = reader.ReadULong();
        }

        public string Tab(int tabCount)
        {
            return "".PadLeft(tabCount);
        }

        public virtual string Write(ref int tabCount)
        {
            return "";
        }
    }

    class StaticBlockEntry : BaseDepotEntry
    {
        public List<BaseDepotEntry> Resources = new List<BaseDepotEntry>();
        public override void Read(NativeReader reader, AssetManager am)
        {
            base.Read(reader, am);

            long offset = reader.ReadLong();
            int count = reader.ReadInt();

            reader.Pad(16);

            for (int i = 0; i < count; i++)
            {
                ulong resRid = reader.ReadULong();
                ResAssetEntry entry = am.GetResEntry(resRid);

                int value = BitConverter.ToInt32(entry.ResMeta, 0x0c);
                BaseDepotEntry bde = null;

                switch (value)
                {
                    case 1: bde = new PersistentBlockEntry(); break;
                    case 2: bde = new StaticBlockEntry(); break;
                    case 3: bde = new MeshBlockEntry(); break;
                    default: throw new NotImplementedException();
                }

                Resources.Add(bde);

                using (NativeReader subReader = new NativeReader(am.GetRes(entry)))
                    bde.Read(subReader, am);
            }
        }

        public override string Write(ref int tabCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tab(tabCount) + "StaticBlockEntry {");
            tabCount += 4;
            sb.AppendLine(Tab(tabCount) + "Hash = 0x" + Hash.ToString("x16") + ",");
            sb.AppendLine(Tab(tabCount) + "Resources = {");
            tabCount += 4;
            for (int i = 0; i < Resources.Count; i++)
                sb.Append(Resources[i].Write(ref tabCount));
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            return sb.ToString();
        }
    }

    class PersistentBlockEntry : BaseDepotEntry
    {
        public List<ParameterEntry> Parameters = new List<ParameterEntry>();
        public override void Read(NativeReader reader, AssetManager am)
        {
            base.Read(reader, am);

            ulong offset = reader.ReadULong();
            int size = reader.ReadInt();

            reader.Pad(16);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ParameterEntry param = new ParameterEntry();
                param.Read(reader);
                Parameters.Add(param);
            }
        }

        public override string Write(ref int tabCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tab(tabCount) + "PersistentBlockEntry = {");
            tabCount += 4;
            sb.AppendLine(Tab(tabCount) + "Hash = 0x" + Hash.ToString("x16") + ",");
            sb.AppendLine(Tab(tabCount) + "Parameters = {");
            tabCount += 4;
            for (int i = 0; i < Parameters.Count; i++)
                sb.Append(Parameters[i].Write(ref tabCount));
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            return sb.ToString();
        }
    }

    class MeshBlockEntry : BaseDepotEntry
    {
        public Guid UnknownGuid;
        public List<ParameterEntry> Parameters = new List<ParameterEntry>();

        public override void Read(NativeReader reader, AssetManager am)
        {
            base.Read(reader, am);

            ulong offset = reader.ReadULong();
            ulong size = reader.ReadULong();
            UnknownGuid = reader.ReadGuid();

            reader.Pad(16);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ParameterEntry param = new ParameterEntry();
                param.Read(reader);
                Parameters.Add(param);
            }
        }

        public override string Write(ref int tabCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tab(tabCount) + "MeshBlockEntry = {");
            tabCount += 4;
            sb.AppendLine(Tab(tabCount) + "Hash = 0x" + Hash.ToString("x16") + ",");
            sb.AppendLine(Tab(tabCount) + "UnknownGuid = " + UnknownGuid.ToString() + ",");
            sb.AppendLine(Tab(tabCount) + "Parameters = {");
            tabCount += 4;
            for (int i = 0; i < Parameters.Count; i++)
                sb.Append(Parameters[i].Write(ref tabCount));
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            return sb.ToString();
        }
    }

    class ParameterEntry
    {
        public ulong Unknown;
        public uint NameHash;
        public uint TypeHash;
        public ushort Used;
        public byte[] Value;

        public void Read(NativeReader reader)
        {
            Unknown = reader.ReadULong();
            TypeHash = reader.ReadUInt();
            Used = reader.ReadUShort();
            NameHash = (uint)((uint)reader.ReadUShort() << 16 | (((Unknown >> 48) & 0xFFFF)));
            Unknown = Unknown & 0x0000FFFFFFFFFFFF;

            int size = reader.ReadInt();
            if (TypeHash == 0xad0abfd3 /* ITexture */)
                size = 16;

            Value = reader.ReadBytes(size);
        }

        public string Write(ref int tabCount)
        {
            string typeName = TypeLibrary.GetType(TypeHash).Name;
            string name = "0x" + NameHash.ToString("x8");

            if (ShaderBlockDepotDump.strings.ContainsKey(NameHash))
                name = ShaderBlockDepotDump.strings[NameHash];

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tab(tabCount) + "Parameter {");
            tabCount += 4;
            sb.AppendLine(Tab(tabCount) + "Unknown = 0x" + Unknown.ToString("x16") + ",");
            sb.AppendLine(Tab(tabCount) + "Name = \"" + name + "\",");
            sb.AppendLine(Tab(tabCount) + "Type = \"" + typeName + "\",");
            sb.AppendLine(Tab(tabCount) + "Used = " + ((Used == 1) ? "True" : "False") + ",");

            sb.Append(Tab(tabCount) + "Value = { ");
            switch(TypeHash)
            {
                case 0xad0abfd3: /* ITexture */
                    {
                        Guid guid = new Guid(Value);
                        sb.Append(guid.ToString());
                    }
                    break;

                case 0x0cc971f4: /* Int64 */ sb.Append(BitConverter.ToUInt64(Value, 0).ToString()); break;
                case 0xb0bc3c22: /* Uint32 */ sb.Append(BitConverter.ToUInt32(Value, 0).ToString()); break;
                case 0x0d1cfa1b: /* Uint8 */ sb.Append(Value[0].ToString()); break;
                case 0x0b87fa95: /* Vec */
                    {
                        float x = BitConverter.ToSingle(Value, 0);
                        float y = BitConverter.ToSingle(Value, 4);
                        float z = BitConverter.ToSingle(Value, 8);
                        float w = BitConverter.ToSingle(Value, 12);
                        sb.Append(x.ToString("F4") + " " + y.ToString("F4") + " " + z.ToString("F4") + " " + w.ToString("F4"));
                    }
                    break;

                default:
                    for (int i = 0; i < Value.Length; i++)
                        sb.Append(Value[i].ToString("x2") + " ");
                    break;
            }
            
            sb.AppendLine(" },");

            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "},");
            return sb.ToString();
        }

        public string Tab(int tabCount)
        {
            return "".PadLeft(tabCount);
        }
    }

    class ShaderBlockDepotEntry : StaticBlockEntry
    {
        public string Filename;

        public override string Write(ref int tabCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tab(tabCount) + "ShaderBlockDepotEntry {");
            tabCount += 4;
            sb.AppendLine(Tab(tabCount) + "Filename = \"" + Filename + "\",");
            sb.AppendLine(Tab(tabCount) + "Hash = 0x" + Hash.ToString("x16") + ",");
            sb.AppendLine(Tab(tabCount) + "Resources = {");
            tabCount += 4;
            for (int i = 0; i < Resources.Count; i++)
            {
                sb.Append(Resources[i].Write(ref tabCount));
            }
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "}");
            tabCount -= 4;
            sb.AppendLine(Tab(tabCount) + "}");
            return sb.ToString();
        }
    }

    class ShaderBlockDepotDump
    {
        public static Dictionary<uint, string> strings = new Dictionary<uint, string>();

        public void Execute(AssetManager am, ILogger logger)
        {
            using (NativeReader reader = new NativeReader(new FileStream(@"E:\Projects\FrostbiteModding\FrostyEditor\bin\Developer\Debug\strings.txt", FileMode.Open, FileAccess.Read)))
            {
                while (reader.Position < reader.Length)
                {
                    string line = reader.ReadLine();
                    int newHash = Fnv1.HashString(line);

                    if (!strings.ContainsKey((uint)newHash))
                        strings.Add((uint)newHash, line);

                    newHash = Fnv1.HashString(line.ToLower());
                    if (!strings.ContainsKey((uint)newHash))
                        strings.Add((uint)newHash, line.ToLower());

                    newHash = Fnv1.HashString(line + "-Array");
                    if (!strings.ContainsKey((uint)newHash))
                        strings.Add((uint)newHash, line.ToLower());
                }
            }
            using (NativeReader reader = new NativeReader(new FileStream(@"H:\Projects\HashString\bin\x64\Debug\new_strings.txt", FileMode.Open, FileAccess.Read)))
            {
                while (reader.Position < reader.Length)
                {
                    string line = reader.ReadLine();
                    int newHash = Fnv1.HashString(line);

                    if (!strings.ContainsKey((uint)newHash))
                        strings.Add((uint)newHash, line);

                    newHash = Fnv1.HashString(line.ToLower());
                    if (!strings.ContainsKey((uint)newHash))
                        strings.Add((uint)newHash, line.ToLower());

                    newHash = Fnv1.HashString(line + "-Array");
                    if (!strings.ContainsKey((uint)newHash))
                        strings.Add((uint)newHash, line.ToLower());
                }
            }

            TypeLibrary.Initialize();
            List<ShaderBlockDepotEntry> sbds = new List<ShaderBlockDepotEntry>();
            foreach (ResAssetEntry entry in am.EnumerateRes(resType: (uint)ResourceType.ShaderBlockDepot))
            {
                int value = 0;
                value = BitConverter.ToInt32(entry.ResMeta, 0x0c);
                if (value == 0)
                {
                    using (NativeReader reader = new NativeReader(am.GetRes(entry)))
                    {
                        ShaderBlockDepotEntry sbd = new ShaderBlockDepotEntry();
                        sbd.Read(reader, am);
                        sbd.Filename = entry.Name;
                        sbds.Add(sbd);
                    }
                }
            }

            foreach (ShaderBlockDepotEntry entry in sbds)
            {
                string filename = entry.Filename;
                int idx = filename.LastIndexOf('/');
                filename = filename.Remove(0, idx);

                TextWriter writer = new StreamWriter(new FileStream("ShaderBlockDepots/" + filename + ".txt", FileMode.Create, FileAccess.Write));
                int tabCount = 0;
                writer.WriteLine(entry.Write(ref tabCount));
                writer.Close();
            }
        }

        void ReadEntry(ResAssetEntry entry, TextWriter writer, AssetManager am)
        {
            int value = BitConverter.ToInt32(entry.ResMeta, 0x0c);
            if (value == 0)
            {
                writer.WriteLine(entry.Name);
                using (NativeReader reader = new NativeReader(am.GetRes(entry)))
                {
                    ulong hash = reader.ReadULong();
                    long offset = reader.ReadLong();
                    int count = reader.ReadInt();
                    reader.Pad(0x10);

                    for (int i = 0; i < count; i++)
                    {
                        ulong resourceId = reader.ReadULong();
                        ResAssetEntry nextEntry = am.GetResEntry(resourceId);

                        writer.WriteLine(" " + nextEntry.Name);
                        ReadEntry(nextEntry, writer, am);
                    }
                }
            }
            else if (value == 2)
            {
                using (NativeReader reader = new NativeReader(am.GetRes(entry)))
                {
                    ulong hash = reader.ReadULong();
                    long offset = reader.ReadLong();
                    int count = reader.ReadInt();
                    reader.Pad(0x10);

                    for (int i = 0; i < count; i++)
                    {
                        ulong resourceId = reader.ReadULong();
                        ResAssetEntry nextEntry = am.GetResEntry(resourceId);

                        writer.WriteLine("  " + nextEntry.Name);
                        //ReadEntry(nextEntry, writer, am);
                    }
                }
            }
        }
    }
}
