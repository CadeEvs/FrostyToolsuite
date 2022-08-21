using Frosty.Core;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundEditorPlugin.Resources
{
    #region EbxData
    [EbxClassMeta(EbxFieldType.Struct)]
    public class Variation
    {
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint VariationId { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint FirstSubtitleIndex { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint SubtitleCount { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint MemoryChunkIndex { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint StreamChunkIndex { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint FirstSegmentIndex { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint SegmentCount { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint FirstLoopSegmentIndex { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint LastLoopSegmentIndex { get; set; }
    }
    [EbxClassMeta(EbxFieldType.Struct)]
    public class Segment
    {
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint SamplesOffset { get; set; }
        [EbxFieldMeta(EbxFieldType.Int32)]
        public uint SamplesOffsetFlag { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint SeekTableOffset { get; set; }
        [EbxFieldMeta(EbxFieldType.Int32)]
        public uint SeekTableFlag { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public float SegmentLength { get; set; }
    }
    [EbxClassMeta(EbxFieldType.Struct)]
    public class Chunk
    {
        [EbxFieldMeta(EbxFieldType.Guid)]
        public Guid ChunkId { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint ChunkSize { get; set; }
    }
    [EbxClassMeta(EbxFieldType.Struct)]
    public class Persistence
    {
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint SelectionParameterCount { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint RequiredVariationCount { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint DesiredVariationCount { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint Speed { get; set; }
        [DisplayName("2C40720C")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown00 { get; set; }
        [DisplayName("B355F9DD")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown01 { get; set; }
        [DisplayName("E230EDBE")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown02 { get; set; }
        [DisplayName("20AFEB30")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown03 { get; set; }
        [DisplayName("804D52B0")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown04 { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint WaterDepth { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint Material { get; set; }
        [DisplayName("1FFFFB8F")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown07 { get; set; }
        [DisplayName("E6CC1A67")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown08 { get; set; }
        [DisplayName("E8A87023")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown09 { get; set; }
        [DisplayName("FBB4809F")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown10 { get; set; }
        [DisplayName("7CCE9E8F")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown11 { get; set; }
        [DisplayName("8F3AEF43")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown12 { get; set; }
        [DisplayName("F6595120")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown13 { get; set; }
        [DisplayName("1B45F0F4")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown14 { get; set; }
        [DisplayName("37301D85")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown15 { get; set; }
        [DisplayName("7D4505A4")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown16 { get; set; }
        [DisplayName("C092D707")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown17 { get; set; }
        [DisplayName("E49FA414")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint unknown18 { get; set; }
    }
    [EbxClassMeta(EbxFieldType.Struct)]
    public class Subtitle
    {
        [EbxFieldMeta(EbxFieldType.CString)]
        public CString StringId { get; set; }
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float Time { get; set; }
        [EbxFieldMeta(EbxFieldType.Int32)]
        public int AdditionalSubtitleInfoType { get; set; }
    }
    [EbxClassMeta(EbxFieldType.Struct)]
    public class Selection
    {
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint VariationId { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint VariationIndex { get; set; }
        [DisplayName("80268F2E")]
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public int unkown { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public int PreDelay { get; set; }
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool IsDay { get; set; }
    }
    [EbxClassMeta(EbxFieldType.Struct)]
    public class SelectionParameter
    {
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint ParameterId { get; set; }
        [EbxFieldMeta(EbxFieldType.UInt32)]
        public uint ParameterIndex { get; set; }
    }
    #endregion

    internal static class ArrayConverter
    {

        public static byte[] GetBytes(long value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            Array.Reverse(ret);
            return ret;
        }

        public static bool ToBoolean(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToBoolean(value, 0);
        }
        public static int ToInt32(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToInt32(value, 0);
        }
        public static uint ToUInt32(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToUInt32(value, 0);
        }
        public static long ToInt64(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToInt64(value, 0);
        }
        public static ulong ToUInt64(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToUInt64(value, 0);
        }
        public static float ToSingle(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToSingle(value, 0);
        }
        public static double ToDouble(byte[] value)
        {
            Array.Reverse(value);
            return BitConverter.ToDouble(value, 0);
        }
    }

    internal static class Extensions
    {
        public static byte[] ReadBytes(this NativeReader reader, int size, Endian endian = Endian.Little)
        {
            byte[] retArray = reader.ReadBytes(size);
            if (endian == Endian.Little)
                Array.Reverse(retArray);
            return retArray;
        }
    }

    public class NewWaveResource : Resource
    {
        public List<Variation> Variations { get; set; } = new List<Variation>();
        public List<Chunk> Chunks { get; set; } = new List<Chunk>();
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public List<SelectionParameter> SelectionParameters { get; set; } = new List<SelectionParameter>();
        public List<Selection> Selections { get; set; } = new List<Selection>();
        public List<Subtitle> Subtitles { get; set; } = new List<Subtitle>();
        public List<Persistence> Persistences { get; set; } = new List<Persistence>();

        private static Endian endian;
        private ushort unkown1;
        private ushort dsetCount;
        private uint unkown2;
        private ulong unkown3;
        private uint offset;
        private static uint dataOffset;
        private Dset[] dsets;
        private static List<byte> data;

        #region DataSet
        internal class SoundBankContainer
        {
            public class RelocPtr
            {
                public string Type;
                public uint Offset;
                public object Data;
                public uint DataOffset;

                public RelocPtr(string type, object data)
                {
                    Type = type;
                    Data = data;
                }
            }
            public class DataPtr
            {
                public List<uint> Offsets = new List<uint>();
                public uint DataOffset;
            }

            private List<RelocPtr> relocPtrs = new List<RelocPtr>();
            private DataPtr dataPtr = new DataPtr();

            public SoundBankContainer()
            {
            }

            public void WriteDataPtr(NativeWriter writer)
            {
                dataPtr.Offsets.Add((uint)writer.Position);
                writer.Write(0xdeadbeefdeadbeef);
            }

            public void AddDataOffset(NativeWriter writer)
            {
                dataPtr.DataOffset = (uint)writer.Position;
            }

            public void AddRelocPtr(string type, object obj)
            {
                relocPtrs.Add(new RelocPtr(type, obj));
            }

            public void WriteRelocPtr(string type, object obj, NativeWriter writer)
            {
                RelocPtr ptr = FindRelocPtr(type, obj);
                if (ptr == null)
                    writer.Write((long)0);
                ptr.Offset = (uint)writer.Position;
                writer.Write(0xdeadbeefdeadbeef);
            }

            public void AddOffset(string type, object data, NativeWriter writer)
            {
                RelocPtr ptr = FindRelocPtr(type, data);
                if (ptr != null)
                    ptr.DataOffset = (uint)writer.Position;
            }

            public void FixupRelocPtrs(NativeWriter writer)
            {
                // hack for data pointers, bc idk how to do it better
                for (int i = 0; i < dataPtr.Offsets.Count; i++)
                {
                    RelocPtr ptr = new RelocPtr("DATA", i);
                    ptr.Offset = dataPtr.Offsets[i];
                    ptr.DataOffset = dataPtr.DataOffset;
                    relocPtrs.Add(ptr);
                }

                // fixup pointers
                relocPtrs.Sort(new RelocPtrComparer());
                for (int i = 0; i < relocPtrs.Count; i++)
                {
                    writer.Position = relocPtrs[i].Offset;
                    writer.Write(relocPtrs[i].DataOffset, endian);
                    if (i + 1 == relocPtrs.Count)
                    {
                        writer.Write(0xFFFFFFFF);
                        return;
                    }
                    writer.Write(relocPtrs[i + 1].Offset, endian);
                }
            }

            private RelocPtr FindRelocPtr(string type, object obj)
            {
                foreach (var ptr in relocPtrs)
                {
                    if (ptr.Type == type && ptr.Data.Equals(obj))
                        return ptr;
                }
                return null;
            }

            private class RelocPtrComparer : IComparer<RelocPtr>
            {
                public int Compare(RelocPtr ptr1, RelocPtr ptr2)
                {
                    return ptr1.Offset.CompareTo(ptr2.Offset);
                }
            }
        }

        public class Dset
        {
            public uint NameHash => nameHash;
            public uint ElemCount => elemCount;
            public List<Field> Fields
            {
                get { return fields; }
                set { fields = value; }
            }

            private uint nameHash;
            private uint unknown1;
            private uint elemCount;

            private ushort offset1;
            private byte[] array1;
            private ushort offset2;
            private byte[] array2;
            private uint offset3;
            private byte[] array3;

            private List<Field> fields;

            private List<UnkownFieldsThing> unkownFieldsThing;

            public Dset(NativeReader reader)
            {
                long start = reader.Position;
                if (reader.ReadUInt(endian) != 0x44534554)
                    throw new FileFormatException("Wrong format of DataSet");
                reader.ReadInt(); // size
                nameHash = reader.ReadUInt(endian);
                unknown1 = reader.ReadUInt(endian);
                reader.ReadInt(); // always 0?? offset for bank start maybe?
                reader.ReadInt();
                if (dataOffset != reader.ReadUInt(endian))
                    App.Logger.LogWarning($"Different data offset of NewWaveResource");
                reader.ReadInt();
                reader.ReadBytes(0x18);
                elemCount = reader.ReadUInt(endian);
                ushort fieldCount = reader.ReadUShort(endian);
                ushort extraFieldCount = reader.ReadUShort(endian);
                offset1 = reader.ReadUShort(endian);
                offset2 = reader.ReadUShort(endian);
                offset3 = reader.ReadUInt(endian);

                if (offset1 != 0)
                {
                    array1 = reader.ReadBytes((int)(start + offset1 - reader.Position)); // should be empty, but i dont trust dice

                    fields = new List<Field>(fieldCount);
                    for (int i = 0; i < fieldCount; i++)
                    {
                        Field field = new Field(reader, this);
                        fields.Add(field);
                    }
                }

                if (offset2 != 0)
                {
                    array2 = reader.ReadBytes((int)(start + offset2 - reader.Position)); // should be empty, but i dont trust dice

                    unkownFieldsThing = new List<UnkownFieldsThing>(extraFieldCount);
                    for (int i = 0; i < extraFieldCount; i++)
                    {
                        UnkownFieldsThing u = new UnkownFieldsThing(reader);
                        unkownFieldsThing.Add(u);
                    }
                }

                if (offset3 != 0)
                {
                    array3 = reader.ReadBytes((int)(start + offset3 - reader.Position)); // should be empty, but i dont trust dice

                    for (int i = 0; i < extraFieldCount; i++)
                    {
                        var u = unkownFieldsThing[i];
                        for (int j = 0; j < u.FieldCount; j++)
                        {
                            ExtraField extraField = new ExtraField(reader);
                            u.Fields.Add(extraField);
                        }
                    }
                }
            }

            public override bool Equals(object obj)
            {
                Dset dset = (Dset)obj;
                return nameHash == dset.NameHash && fields == dset.Fields;
            }

            public override int GetHashCode()
            {
                return (int)nameHash;
            }

            internal void PreProcess(SoundBankContainer sbContainer)
            {
                sbContainer.AddRelocPtr("BANK", nameHash);

                foreach (var field in fields)
                    field.PreProcess(sbContainer);

                foreach (var thing in unkownFieldsThing)
                    thing.PreProcess(sbContainer);
            }
            internal void Process(NativeWriter writer, SoundBankContainer sbContainer)
            {
                long start = writer.Position;

                writer.Write(0x44534554, endian);
                writer.Write(0xdeadbeef);
                writer.Write(nameHash, endian);
                writer.Write(unknown1, endian);

                sbContainer.WriteRelocPtr("BANK", nameHash, writer);
                sbContainer.WriteDataPtr(writer);

                for (int i = 0; i < 3; i++)
                    writer.Write((long)0);

                writer.Write(elemCount, endian);
                writer.Write((ushort)fields.Count, endian);
                writer.Write((ushort)unkownFieldsThing.Count, endian);

                writer.Write(0xdeadbeefdeadbeef);

                writer.Write(array1); // should be empty

                long curPos = writer.Position;
                writer.Position = start + 0x40;
                writer.Write((ushort)(curPos - start), endian);
                writer.Position = curPos;

                foreach (var field in fields)
                    field.Process(writer, sbContainer);

                writer.Write(array2); // should be empty

                curPos = writer.Position;
                writer.Position = start + 0x42;
                writer.Write((ushort)(curPos - start), endian);
                writer.Position = curPos;

                foreach (var thing in unkownFieldsThing)
                    thing.Process(writer, sbContainer);

                writer.Write(array3); // should be empty

                curPos = writer.Position;
                writer.Position = start + 0x44;
                writer.Write((uint)(curPos - start), endian);
                writer.Position = curPos;

                foreach (var thing in unkownFieldsThing)
                {
                    foreach (var extraField in thing.Fields)
                        extraField.Process(writer, sbContainer);
                }

                curPos = writer.Position;
                writer.Position = start + 4;
                writer.Write(curPos - start);
                writer.Position = curPos;

                foreach (var field in fields)
                {
                    sbContainer.AddOffset("FIELDTABLE", field, writer);
                    field.WriteTable(writer);
                }

                foreach (var field in fields)
                    field.WriteDebugTable(writer);

                foreach (var thing in unkownFieldsThing)
                {
                    // TODO: figure out what the indices do
                    sbContainer.AddOffset("UNKOWNINDICES", thing, writer);
                }
            }

            public Field GetField(string name)
            {
                foreach (var i in fields)
                {
                    if (i.NameHash == (uint)Fnv1.HashString(name))
                        return i;
                }
                return null;
            }
        }
        public class UnkownFieldsThing
        {
            public uint Offset { get; set; }

            public ushort FieldCount { get; set; }
            public List<ExtraField> Fields { get; internal set; }

            private uint offset;
            private uint u1;
            private ushort u2;
            private ushort fieldCount;

            public UnkownFieldsThing(NativeReader reader)
            {
                offset = reader.ReadUInt(endian);
                reader.ReadBytes(0x14);
                u1 = reader.ReadUInt(endian);
                u2 = reader.ReadUShort(endian);
                fieldCount = (ushort)(reader.ReadUShort(endian) >> 8);
                Fields = new List<ExtraField>(fieldCount);
            }

            public override bool Equals(object obj)
            {
                UnkownFieldsThing thing = (UnkownFieldsThing)obj;
                return offset == thing.offset && u1 == thing.u1 && u2 == thing.u2 && fieldCount == thing.fieldCount;
            }

            internal void PreProcess(SoundBankContainer sbContainer)
            {
                if (offset != 0)
                    sbContainer.AddRelocPtr("UNKOWNINDICES", this);
            }
            internal void Process(NativeWriter writer, SoundBankContainer sbConatainer)
            {
                sbConatainer.WriteRelocPtr("UNKOWNINDICES", this, writer);
                for (int i = 0; i < 2; i++)
                    writer.Write((long)0);
                writer.Write(u1, endian);
                writer.Write(u2, endian);
                writer.Write(fieldCount << 8, endian);
            }
        }
        public class ExtraField
        {
            public uint NameHash => nameHash;
            public List<dynamic> Values
            {
                get { return values; }
                set { values = value; }
            }

            private uint nameHash;
            private ushort elemCount;
            private ushort storeType;
            private int storeParam1;
            private int storeParam2;
            private uint tableOffset;

            private byte[] table;

            List<dynamic> values;

            public ExtraField(NativeReader reader)
            {
                nameHash = reader.ReadUInt(endian);
                elemCount = reader.ReadUShort(endian);
                storeType = reader.ReadUShort(endian);
                storeParam1 = reader.ReadInt(endian);
                storeParam2 = reader.ReadInt(endian);
                tableOffset = reader.ReadUInt(endian);
            }

            public override bool Equals(object obj)
            {
                ExtraField field = (ExtraField)obj;
                return nameHash == field.NameHash && Values == field.Values;
            }

            internal void PreProcess(SoundBankContainer sbContainer)
            {
                StoreValues();
                if (table != null)
                    sbContainer.AddRelocPtr("EXTRAFIELDTABLE", this);
            }

            internal void Process(NativeWriter writer, SoundBankContainer sbContainer)
            {
                writer.Write(nameHash, endian);
                writer.Write(elemCount, endian);
                writer.Write(storeType, endian);
                writer.Write(storeParam1, endian);
                writer.Write(storeParam2, endian);
                sbContainer.WriteRelocPtr("EXTRAFIELDTABLE", this, writer);
            }

            internal void StoreValues()
            {

            }
        }
        public class Field
        {
            public uint NameHash => nameHash;
            public List<dynamic> Values
            {
                get { return values; }
                set { values = value; }
            }

            enum SbDataType
            {
                Boolean = 0,
                Int32 = 1,
                UInt32 = 2,
                Int64 = 3,
                UInt64 = 4,
                Float32 = 5,
                Float64 = 6,
                String = 7,
                Guid = 8
            }

            private uint nameHash;
            private SbDataType dataType;
            private byte storeType;
            private short storeParam1;
            private long storeParam2;
            private uint tableOffset;

            private byte[] table;
            private byte[] debugTable;

            private List<dynamic> values;

            public Field(NativeReader reader, Dset parent)
            {
                nameHash = reader.ReadUInt(endian);
                dataType = (SbDataType)reader.ReadByte();
                storeType = reader.ReadByte();
                storeParam1 = reader.ReadShort(endian);
                storeParam2 = reader.ReadLong(endian);
                tableOffset = reader.ReadUInt(endian);
                reader.ReadBytes(4);
                long curPos = reader.Position;

                if (nameHash == 0xD00A6005)
                    nameHash = 0xD00A6005;

                values = new List<dynamic>((int)parent.ElemCount);
                for (int i = 0; i < parent.ElemCount; i++)
                {
                    byte[] storedValue = new byte[8];
                    switch (storeType)
                    {
                        case 0x00:
                            ArrayConverter.GetBytes(storeParam2).CopyTo(storedValue, 0);
                            break;
                        case 0x01:
                            ArrayConverter.GetBytes(storeParam2 + (storeParam1 * i)).CopyTo(storedValue, 0);
                            break;
                        case 0x02:
                            byte shift = (byte)((storeParam1 & 0x00FF) >> 0);
                            byte size = (byte)((storeParam1 & 0xFF00) >> 8);
                            reader.Position = tableOffset + (size * i);
                            List<byte> buffer = new List<byte>(8);
                            buffer.AddRange(reader.ReadBytes(size, endian));
                            for (int j = 0; j < 8 - size; j++)
                                buffer.Insert(0, 0);
                            long temp = ArrayConverter.ToInt64(buffer.ToArray());
                            temp <<= shift;
                            temp += storeParam2;
                            buffer.Clear();
                            buffer.AddRange(ArrayConverter.GetBytes(temp));
                            buffer.CopyTo(storedValue, 0);
                            break;
                        case 0x03:
                            byte bits = (byte)((storeParam1 & 0x00FF) >> 0);
                            size = (byte)((storeParam1 & 0xFF00) >> 8);
                            long indexOffset = tableOffset + storeParam2 * size + (i * bits) / 8;
                            reader.Position = indexOffset;
                            byte index = reader.ReadByte();
                            index >>= (i * bits) % 8;
                            index &= (byte)((1 << bits) - 1);
                            reader.Position = tableOffset + (size * index);
                            reader.ReadBytes(size, endian).CopyTo(storedValue, 8 - size);
                            break;
                    }
                    dynamic val = null;
                    switch (dataType)
                    {
                        case SbDataType.Boolean:
                            val = ArrayConverter.ToBoolean(storedValue);
                            break;
                        case SbDataType.Int32:
                            val = ArrayConverter.ToInt32(storedValue);
                            break;
                        case SbDataType.UInt32:
                            val = ArrayConverter.ToUInt32(storedValue);
                            break;
                        case SbDataType.Int64:
                            val = ArrayConverter.ToInt64(storedValue);
                            break;
                        case SbDataType.UInt64:
                            val = ArrayConverter.ToUInt64(storedValue);
                            break;
                        case SbDataType.Float32:
                            val = ArrayConverter.ToSingle(storedValue);
                            break;
                        case SbDataType.Float64:
                            val = ArrayConverter.ToDouble(storedValue);
                            break;
                        case SbDataType.String:
                            val = ArrayConverter.ToUInt64(storedValue);
                            reader.Position = (long)(dataOffset + val - 1);
                            val = reader.ReadNullTerminatedString();
                            break;
                        case SbDataType.Guid:
                            val = ArrayConverter.ToUInt64(storedValue);
                            reader.Position = (long)(dataOffset + val - 1);
                            val = reader.ReadGuid(endian);
                            break;
                    }
                    values.Add(val);
                }
                reader.Position = curPos;
            }

            public override bool Equals(object obj)
            {
                Field field = (Field)obj;
                return nameHash == field.NameHash && Values == field.Values;
            }

            internal void PreProcess(SoundBankContainer sbContainer)
            {
                StoreValues();
                if (table != null)
                    sbContainer.AddRelocPtr("FIELDTABLE", this);
            }

            internal void Process(NativeWriter writer, SoundBankContainer sbContainer)
            {
                writer.Write(nameHash, endian);
                writer.Write((byte)dataType, endian);
                writer.Write(storeType, endian);
                writer.Write(storeParam1, endian);
                writer.Write(storeParam2, endian);
                sbContainer.WriteRelocPtr("FIELDTABLE", this, writer);
            }

            internal void WriteTable(NativeWriter writer)
            {
                writer.WritePadding(0x04);
                if (table != null)
                    writer.Write(table);
            }

            internal void WriteDebugTable(NativeWriter writer)
            {
                writer.WritePadding(0x04);
                if (debugTable != null)
                    writer.Write(debugTable);
            }

            internal void StoreValues()
            {
                /* the same -> 0x00
                 * linear -> 0x01 (0, 1, 2, 3,...)
                 * individual -> 0x02
                 * not that many different values -> 0x03 (8x 0, 1x 3, 2x 6)
                 * offsets for guid/string -> 0x04
                 */

                List<long> newValues = new List<long>(values.Count);
                foreach (var item in values)
                {
                    if (storeType == 7 || storeType == 8)
                    {
                        using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                        {
                            for (int i = 0; i < values.Count; i++)
                            {
                                int inDataOffset = (int)writer.Position;

                                if (values[i] is Guid)
                                    writer.Write(values[i], endian);

                                else if (values[i] is string)
                                    writer.WriteNullTerminatedString(values[i]);

                                byte[] offset = new byte[8];
                                BitConverter.GetBytes(inDataOffset + 1 + data.Count).CopyTo(offset, 0);
                                newValues.Add(BitConverter.ToInt64(offset, 0));
                            }
                            data.AddRange(writer.ToByteArray());
                        }
                        break;
                    }

                    byte[] value = new byte[8];
                    BitConverter.GetBytes(item).CopyTo(value, 0);
                    newValues.Add(BitConverter.ToInt64(value, 0));
                }

                #region StoreTypes

                bool type0()
                {
                    var item = newValues.FirstOrDefault();
                    if (!newValues.Skip(1).All(i => i == item))
                        return false;
                    storeType = 0x00;
                    storeParam1 = 0x00;
                    storeParam2 = item;
                    table = null;
                    return true;
                }

                bool type1()
                {
                    var item = newValues[0];
                    var v = newValues[1] - item;
                    if (v > short.MaxValue)
                        return false;
                    else if (v < short.MinValue)
                        return false;

                    for (int i = 1; i < newValues.Count; i++)
                    {
                        if (newValues[i] - newValues[i - 1] != v)
                            return false;
                    }
                    storeType = 0x01;
                    storeParam1 = (short)v;
                    storeParam2 = newValues[0];
                    table = null;
                    return true;
                }

                #endregion

                if (values.Count > 0)
                {
                    if (type0())
                        return;

                    else if (type1())
                        return;

                    else
                    {
                        // check type2
                        long[] sizeDiffs = new long[newValues.Count];
                        for (int i = 0; i < newValues.Count; i++)
                            sizeDiffs[i] = newValues[i] - newValues.GetLowest();
                        long[] outDiffs = new long[newValues.Count];
                        byte shift = sizeDiffs.GetShift(out outDiffs);

                        byte[] result2 = outDiffs.ConvertToBytes(endian);

                        int tableSize = result2.Length;

                        byte size2 = outDiffs.GetBiggestSize();


                        // check type3
                        Dictionary<long, int> keyValuePairs = new Dictionary<long, int>();
                        foreach (var item in newValues)
                        {
                            if (!keyValuePairs.ContainsKey(item))
                                keyValuePairs.Add(item, 0);
                            keyValuePairs[item]++;
                        }
                        byte bits = 0x1;
                        if (keyValuePairs.Count > 16)
                            bits = 0x8;
                        else if (keyValuePairs.Count > 4)
                            bits = 0x4;
                        else if (keyValuePairs.Count > 2)
                            bits = 0x2;

                        byte size3 = newValues.ToArray().GetBiggestSize();


                        using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                        {
                            writer.Write(keyValuePairs.Keys.ToArray().ConvertToBytes(endian));

                            List<int> idx = new List<int>();
                            for (int j = 0; j < values.Count; j++)
                            {
                                for (int i = 0; i < keyValuePairs.Count; i++)
                                {
                                    if (newValues[j] == keyValuePairs[i])
                                        idx.Add(i);
                                }
                            }

                            int length = (idx.Count % (8 / size3)) == 0 ? idx.Count / (8 / size3) : (idx.Count / (8 / size3)) + 1;
                            byte[] idxes = new byte[length];
                            for (int j = 0; j < values.Count; j++)
                            {
                                idxes[j / (8 / size3)] |= (byte)(idx[j] << ((j * size3) % 8));
                            }

                            writer.Write(idxes);

                            byte[] result3 = writer.ToByteArray();

                            if (result3.Length < result2.Length)
                            {
                                storeType = 0x03;
                                storeParam1 = (short)((size3 << 8) | bits);
                                storeParam2 = keyValuePairs.Count;
                                table = result3;

                                // TODO: debug table for type3
                            }
                            else
                            {
                                storeType = 0x02;
                                storeParam1 = (short)((size2 << 8) | shift);
                                storeParam2 = newValues.GetLowest();
                                table = result2;
                                List<long> debug = new List<long>(outDiffs);
                                debug.Sort();
                                debugTable = debug.ToArray().ConvertToBytes(endian);
                            }
                        }

                        return;
                    }
                }
                else if (values.Count == 0)
                {
                    storeParam1 = 0;
                    storeParam2 = 0;
                    tableOffset = 0;
                }
            }
        }

        public Dset GetDSET(string name)
        {
            foreach (var i in dsets)
            {
                if (i.NameHash == (uint)Fnv1.HashString(name))
                    return i;
            }
            return null;
        }
        #endregion

        /*
          * "VariationId"
          * "VariationIndex"
          * "StreamChunkIndex"
          * "MemoryChunkIndex"
          * "FirstSegmentIndex"
          * "FirstLoopSegmentIndex"
          * "LastLoopSegmentIndex"
          * "SegmentCount"
          * "SamplesOffset"
          * "SeekTableOffset"
          * "Duration"
          * "ChunkSize"
          * "ChunkId"
          * "ParameterIndex"
          * "ParameterId"
          * "Time"
          * "AdditionalSubtitleInfoType"
          * "FirstSubtitleIndex"
          * "SelectionParameterCount"
          * "StringId"
          * "SubtitleCount"
          * "Speed"
          * "IsDay"
          * "RequiredVariationCount"
          * "DesiredVariationCount"
          * "Material"
          * "WaterDepth"
          * "PreDelay"
          * "Actor"
          * "ChunkIndex"
          * "city"
          * "Direction"
          * "Distance"
          * "Emotion"
          * "Exertion"
          * "Gender"
          * "ImpactLocation"
          * "Intensity"
          * "Language"
          * "Material"
          * "Progress"
          * "Rarity"
          * "SegmentIndex"
          * "Size"
          * "SourceSize"
          * "Speed"
          * "SubtitleIndex"
          * "Team"
          * "Type"
          * "Variation"
          * "VariationIndex"
          * "Voice"
          * "WaterDepth"
          * "Weight"
          */

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            endian = reader.ReadSizedString(4).Equals("SBle") ? Endian.Little : Endian.Big;
            reader.ReadInt(endian); // size of bank
            unkown1 = reader.ReadUShort(endian);
            dsetCount = reader.ReadUShort(endian);
            unkown2 = reader.ReadUInt(endian);
            unkown3 = reader.ReadULong(endian);
            offset = reader.ReadUInt(endian);
            reader.ReadInt(endian); // offset for the next offset
            dataOffset = reader.ReadUInt(endian);
            reader.ReadInt(endian); // offset for the next offset
            reader.Position = offset;
            uint[] dsetOffsets = new uint[dsetCount];
            dsets = new Dset[dsetCount];
            for (int i = 0; i < dsetCount; i++)
            {
                dsetOffsets[i] = reader.ReadUInt(endian);
                reader.ReadInt();
            }
            reader.Pad(0x10);
            if (reader.Position != dsetOffsets[0])
                App.Logger.LogWarning($"Weird layout of NewWaveResource: {entry.Name}");
            for (int i = 0; i < dsetCount; i++)
            {
                reader.Position = dsetOffsets[i];
                dsets[i] = new Dset(reader);

                if (dsets[i].NameHash == (uint)Fnv1.HashString("SelectionParameters"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        SelectionParameter sp = new SelectionParameter();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0x0E6AC212: sp.ParameterIndex = field.Values[j]; break;
                                case 0x8D21F9A1: sp.ParameterId = field.Values[j]; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: SelectionParameters"); break;
                            }
                        }
                        SelectionParameters.Add(sp);
                    }
                }

                else if (dsets[i].NameHash == (uint)Fnv1.HashString("Selection"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        Selection se = new Selection();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0xF5F914D9: se.VariationId = field.Values[j]; break;
                                case 0x6AC4E4EA: se.VariationIndex = field.Values[j]; break;
                                case 0x0CD87DC3: se.IsDay = field.Values[j]; break;
                                case 0x9DA8ED37: se.PreDelay = field.Values[j]; break;
                                case 0x80268F2E: se.unkown = field.Values[j]; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: Selection"); break;
                            }
                        }
                        Selections.Add(se);
                    }
                }

                else if (dsets[i].NameHash == (uint)Fnv1.HashString("Variations"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        Variation v = new Variation();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0xF5F914D9: v.VariationId = field.Values[j]; break;
                                case 0x678C1CBC: v.StreamChunkIndex = field.Values[j] >> 1; break;
                                case 0x4E5B3721: v.MemoryChunkIndex = field.Values[j] >> 1; break;
                                case 0xE4660A62: v.FirstSegmentIndex = field.Values[j]; break;
                                case 0x6B89F83E: v.FirstLoopSegmentIndex = field.Values[j]; break;
                                case 0x03DA5B4E: v.LastLoopSegmentIndex = field.Values[j]; break;
                                case 0xD00A6005: v.SegmentCount = field.Values[j]; break;
                                case 0x4AF36C62: v.SubtitleCount = field.Values[j]; break;
                                case 0xD7C152C5: v.FirstSubtitleIndex = field.Values[j]; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: Variations"); break;
                            }
                        }
                        Variations.Add(v);
                    }
                }

                else if (dsets[i].NameHash == (uint)Fnv1.HashString("Segments"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        Segment s = new Segment();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0x6CFCCE5B: s.SegmentLength = field.Values[j]; break;
                                case 0xD506D74E:
                                    s.SeekTableFlag = field.Values[j] & 0x03;
                                    s.SeekTableOffset = field.Values[j] & (uint)0xFFFFFFFC; break;
                                case 0xE8E591DD:
                                    s.SamplesOffsetFlag = field.Values[j] & 0x03;
                                    s.SamplesOffset = field.Values[j] & (uint)0xFFFFFFFC; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: Segments"); break;
                            }
                        }
                        Segments.Add(s);
                    }
                }

                else if (dsets[i].NameHash == (uint)Fnv1.HashString("Chunks"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        Chunk c = new Chunk();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0xF4369173: c.ChunkId = field.Values[j]; break;
                                case 0xDC19107B: c.ChunkSize = field.Values[j]; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: Chunks"); break;
                            }
                        }
                        Chunks.Add(c);
                    }
                }

                else if (dsets[i].NameHash == (uint)Fnv1.HashString("Subtitles"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        Subtitle su = new Subtitle();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0x7C8865D0: su.Time = field.Values[j]; break;
                                case 0xF8DC368E: su.AdditionalSubtitleInfoType = field.Values[j]; break;
                                case 0xD2C4765D: su.StringId = (CString)field.Values[j]; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: Subtitles"); break;
                            }
                        }
                        Subtitles.Add(su);
                    }
                }

                else if (dsets[i].NameHash == (uint)Fnv1.HashString("Persistence"))
                {
                    for (int j = 0; j < dsets[i].ElemCount; j++)
                    {
                        Persistence p = new Persistence();
                        foreach (var field in dsets[i].Fields)
                        {
                            switch (field.NameHash)
                            {
                                case 0x5C8501CF: p.SelectionParameterCount = field.Values[j]; break;
                                case 0x6BEE3A3E: p.RequiredVariationCount = field.Values[j]; break;
                                case 0x81C2613F: p.DesiredVariationCount = field.Values[j]; break;
                                case 0x0DC30E82: p.Speed = field.Values[j]; break;
                                case 0x2C40720C: p.unknown00 = field.Values[j]; break;
                                case 0xB355F9DD: p.unknown01 = field.Values[j]; break;
                                case 0xE230EDBE: p.unknown02 = field.Values[j]; break;
                                case 0x20AFEB30: p.unknown03 = field.Values[j]; break;
                                case 0x804D52B0: p.unknown04 = field.Values[j]; break;
                                case 0x00D6E91D: p.WaterDepth = field.Values[j]; break;
                                case 0x326770EE: p.Material = field.Values[j]; break;
                                case 0x1FFFFB8F: p.unknown07 = field.Values[j]; break;
                                case 0xE6CC1A67: p.unknown08 = field.Values[j]; break;
                                case 0xE8A87023: p.unknown09 = field.Values[j]; break;
                                case 0xFBB4809F: p.unknown10 = field.Values[j]; break;
                                case 0x7CCE9E8F: p.unknown11 = field.Values[j]; break;
                                case 0x8F3AEF43: p.unknown12 = field.Values[j]; break;
                                case 0xF6595120: p.unknown13 = field.Values[j]; break;
                                case 0x1B45F0F4: p.unknown14 = field.Values[j]; break;
                                case 0x37301D85: p.unknown15 = field.Values[j]; break;
                                case 0x7D4505A4: p.unknown16 = field.Values[j]; break;
                                case 0xC092D707: p.unknown17 = field.Values[j]; break;
                                case 0xE49FA414: p.unknown18 = field.Values[j]; break;
                                default: App.Logger.LogWarning("Unkown field: " + field.NameHash.ToString("X8") + "Dset: Persistence"); break;
                            }
                        }
                        Persistences.Add(p);
                    }
                }

                else
                    App.Logger.LogWarning("Unknown DataSet: " + dsets[i].NameHash.ToString("X8"));
            }
        }
        public override byte[] SaveBytes()
        {
            UpdateDSETS();

            SoundBankContainer sbContainer = new SoundBankContainer();

            PreProcess(sbContainer);

            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                Process(writer, sbContainer);

                // write data for guids and strings
                sbContainer.AddDataOffset(writer);
                writer.Write(data.ToArray());

                sbContainer.FixupRelocPtrs(writer);

                uint size = (uint)writer.Position;

                writer.Position = 4;
                writer.Write(size);

                return writer.ToByteArray();
            }
        }

        internal void PreProcess(SoundBankContainer sbContainer)
        {
            UpdateDSETS();

            sbContainer.AddRelocPtr("DSETOFFSETS", dsets);

            sbContainer.AddRelocPtr("DATA", "BANKDATA");

            foreach (var dset in dsets)
            {
                sbContainer.AddRelocPtr("DSET", dset);
                dset.PreProcess(sbContainer);
            }
        }

        internal void Process(NativeWriter writer, SoundBankContainer sbContainer)
        {
            writer.Write(endian == Endian.Little ? 0x53426c65 : 0x53426265, Endian.Big);
            writer.Write(0xdeadbeef);
            writer.Write(unkown1, endian);
            writer.Write(dsetCount, endian);
            writer.Write(unkown2, endian);
            writer.Write(unkown3, endian);

            sbContainer.WriteRelocPtr("DSETOFFSETS", dsets, writer);
            sbContainer.WriteDataPtr(writer);

            for (int i = 0; i < 3; i++)
                writer.Write(0);

            sbContainer.AddOffset("DSETOFFSETS", dsets, writer);
            foreach (var dset in dsets)
                sbContainer.WriteRelocPtr("DSET", dset, writer);

            foreach (var dset in dsets)
            {
                writer.WritePadding(0x10);
                dset.Process(writer, sbContainer);
            }
        }

        internal void UpdateDSETS()
        {
            // TODO: add all dsets
            UpdateVariations();
            UpdateSegments();
            UpdateChunks();
        }

        internal void UpdateVariations()
        {
            List<dynamic> variIds = new List<dynamic>();
            List<dynamic> mchunkIdxs = new List<dynamic>();
            List<dynamic> schunkIdxs = new List<dynamic>();
            List<dynamic> firstIdxs = new List<dynamic>();
            List<dynamic> segCounts = new List<dynamic>();
            List<dynamic> firstLoops = new List<dynamic>();
            List<dynamic> lastLoops = new List<dynamic>();
            foreach (var vari in Variations)
            {
                variIds.Add(vari.VariationId);
                mchunkIdxs.Add((uint)((vari.MemoryChunkIndex << 1) | 0x1)); // used flag i think
                schunkIdxs.Add((uint)((vari.StreamChunkIndex << 1) | 0x1));
                firstIdxs.Add(vari.FirstSegmentIndex);
                segCounts.Add(vari.SegmentCount);
                firstLoops.Add(vari.FirstLoopSegmentIndex);
                lastLoops.Add(vari.LastLoopSegmentIndex);
            }
            Dset dset = GetDSET("Variations");
            if (dset == null)
                return;
            Field variId = dset.GetField("VariationId");
            Field mchunkIdx = dset.GetField("MemoryChunkIndex");
            Field schunkIdx = dset.GetField("StreamChunkIndex");
            Field firstIdx = dset.GetField("FirstSegmentIndex");
            Field segCount = dset.GetField("SegmentCount");
            Field firstLoop = dset.GetField("FirstLoopSegmentIndex");
            Field lastLoop = dset.GetField("LastLoopSegmentIndex");
            if (!variId.Values.Equals(variIds))
            {
                variId.Values = variIds;
            }
            if (!mchunkIdx.Values.Equals(mchunkIdxs))
            {
                mchunkIdx.Values = mchunkIdxs;
            }
            if (!schunkIdx.Values.Equals(schunkIdxs))
            {
                schunkIdx.Values = schunkIdxs;
            }
            if (!firstIdx.Values.Equals(firstIdxs))
            {
                firstIdx.Values = firstIdxs;
            }
            if (!segCount.Values.Equals(segCounts))
            {
                segCount.Values = segCounts;
            }
            if (!firstLoop.Values.Equals(firstLoops))
            {
                firstLoop.Values = firstLoops;
            }
            if (!lastLoop.Values.Equals(lastLoops))
            {
                lastLoop.Values = lastLoops;
            }
        }

        internal void UpdateSegments()
        {
            List<dynamic> sampOffsets = new List<dynamic>();
            List<dynamic> seekOffsets = new List<dynamic>();
            List<dynamic> durations = new List<dynamic>();

            foreach (var seg in Segments)
            {
                sampOffsets.Add(seg.SamplesOffset | seg.SamplesOffsetFlag); // flag for what chunk is used: 0 = none; 1 = memory?; 3 = stream?
                seekOffsets.Add(seg.SeekTableOffset | seg.SeekTableFlag);
                durations.Add(seg.SegmentLength);
            }
            Dset dset = GetDSET("Segments");
            if (dset == null)
                return;
            Field sampOffset = dset.GetField("SamplesOffset");
            Field seekOffset = dset.GetField("SeekTableOffset");
            Field duration = dset.GetField("Duration");
            if (!sampOffset.Values.Equals(sampOffsets))
            {
                sampOffset.Values = sampOffsets;
            }
            if (!seekOffset.Values.Equals(seekOffsets))
            {
                seekOffset.Values = seekOffsets;
            }
            if (!duration.Values.Equals(durations))
            {
                duration.Values = durations;
            }
        }

        internal void UpdateChunks()
        {
            List<dynamic> ids = new List<dynamic>();
            List<dynamic> sizes = new List<dynamic>();
            foreach (var chunk in Chunks)
            {
                ids.Add(chunk.ChunkId);
                sizes.Add(chunk.ChunkSize);
            }
            Dset dset = GetDSET("Chunks");
            if (dset == null)
                return;
            Field id = dset.GetField("ChunkId");
            Field size = dset.GetField("ChunkSize");
            if (!id.Values.Equals(ids))
            {
                id.Values = ids;
            }
            if (!size.Values.Equals(sizes))
            {
                size.Values = sizes;
            }
        }
    }

    public static class SoundBankUtils
    {
        public static long GetLowest(this List<long> list)
        {
            return list.ToArray().GetLowest();
        }
        public static long GetLowest(this long[] array)
        {
            if (array.Length < 1)
                throw new Exception("array can't be empty");
            long result = array[0];
            foreach (var item in array)
            {
                if (result > item)
                    result = item;
            }
            return result;
        }
        // TODO: check if these work
        public static byte GetBiggestSize(this long[] array)
        {
            byte result = 0;
            foreach (var item in array)
            {
                byte[] buffer = BitConverter.GetBytes(item);
                Array.Reverse(buffer);
                byte c = 0;
                foreach (byte b in buffer)
                {
                    if (c != 0)
                        break;
                    c++;
                }
                c = (byte)(8 - c);
                if (c > result)
                    result = c;
            }
            if (result > 4)
                result = 8;
            else if (result > 2)
                result = 4;
            else if (result > 1)
                result = 2;
            return result;
        }
        public static byte GetShift(this long[] array, out long[] outArray)
        {
            byte size = GetBiggestSize(array);
            Dictionary<int, List<long>> shifts = new Dictionary<int, List<long>>();

            foreach (var item in array)
            {
                for (int i = 0; i <= byte.MaxValue; i++)
                {
                    double b = (double)item / Math.Pow(2, i);
                    if (b % 1 == 0)
                    {
                        if (!shifts.ContainsKey(i))
                            shifts.Add(i, new List<long>());
                        shifts[i].Add(Convert.ToInt64(b));
                    }
                }
            }
            List<long> bytes = new List<long>(byte.MaxValue);
            foreach (var item in shifts)
            {
                if (item.Value.Count != array.Length)
                    continue;
                bytes.Add(item.Key);
            }
            byte result = (byte)bytes.GetLowest();
            if (shifts[result].ToArray().GetBiggestSize() < size)
            {
                outArray = shifts[result].ToArray();
                return result;
            }
            outArray = array;
            return 0;
        }
        public static byte[] ConvertToBytes(this long[] array, Endian endian)
        {
            byte size = array.GetBiggestSize();
            byte[] outArray = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                byte[] buffer = BitConverter.GetBytes(array[i]);
                for (int j = 0; i < size; j++)
                {
                    if (endian == Endian.Little)
                        outArray[i * size + j] = buffer[j];
                    else
                        outArray[i * size + j] = buffer[size - 1 - j];
                }
            }
            return outArray;
        }
    }

    public class NewWaveAssetOverride : BaseTypeOverride
    {
        [FieldIndex(0)]
        public BaseFieldOverride Chunks { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(10)]
        public List<Variation> RuntimeVariations { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(11)]
        public List<Selection> Selection { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(12)]
        public List<SelectionParameter> SelectionParameters { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(16)]
        public List<Segment> Segments { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(17)]
        public List<Persistence> Persistence { get; set; }

        public override void Load()
        {
            dynamic og = Original;
            ResAssetEntry entry = App.AssetManager.GetResEntry((string)og.Name);
            NewWaveResource res = App.AssetManager.GetResAs<NewWaveResource>(entry);
            SelectionParameters = res.SelectionParameters;
            Selection = res.Selections;
            RuntimeVariations = res.Variations;
            Segments = res.Segments;
            Persistence = res.Persistences;
        }
    }
    public class LocalizedWaveAssetOverride : BaseTypeOverride
    {
        [FieldIndex(0)]
        public BaseFieldOverride Chunks { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(10)]
        public List<Variation> RuntimeVariations { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(11)]
        public List<Selection> Selection { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(12)]
        public List<SelectionParameter> SelectionParameters { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(16)]
        public List<Segment> Segments { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(17)]
        public List<Persistence> Persistence { get; set; }

        [EbxFieldMeta(EbxFieldType.Struct)]
        [FieldIndex(18)]
        public List<Subtitle> Subtitles { get; set; }

        public override void Load()
        {
            dynamic og = Original;
            ResAssetEntry entry = App.AssetManager.GetResEntry((string)og.Name);
            NewWaveResource res = App.AssetManager.GetResAs<NewWaveResource>(entry);
            SelectionParameters = res.SelectionParameters;
            Selection = res.Selections;
            RuntimeVariations = res.Variations;
            Segments = res.Segments;
            Subtitles = res.Subtitles;
            Persistence = res.Persistences;
        }
    }
}
