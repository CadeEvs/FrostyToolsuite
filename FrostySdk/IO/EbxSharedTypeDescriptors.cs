using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.IO
{

    public class EbxSharedTypeDescriptors
    {
        public int ClassCount => classes.Count;

        private List<EbxClass?> classes = new List<EbxClass?>();
        private Dictionary<Guid, int> mapping = new Dictionary<Guid, int>();
        private List<EbxField?> fields = new List<EbxField?>();
        private List<Guid?> typeInfoGuids = new List<Guid?>();

        public EbxSharedTypeDescriptors(FileSystemManager fs, string name)
        {
            File.WriteAllBytes(name, fs.GetFileFromMemoryFs(name));
            bool patch = name.Contains("patch");
            using (NativeReader reader = new NativeReader(new MemoryStream(fs.GetFileFromMemoryFs(name))))
            {
                EbxVersion magic = (EbxVersion)reader.ReadUInt();
                if (magic == EbxVersion.Version4)
                    ReadV1(reader, patch);
                else if (magic == EbxVersion.Version6)
                    ReadRiff(reader, patch);
            }
        }

        private void ReadV1(NativeReader reader, bool patch)
        {
            ushort numClasses = reader.ReadUShort();
            ushort numFields = reader.ReadUShort();

            for (int i = 0; i < numFields; i++)
            {
                uint hash = reader.ReadUInt();

                EbxField field = new EbxField
                {
                    NameHash = hash,
                    Type = (ushort)(reader.ReadUShort() >> 1),
                    ClassRef = reader.ReadUShort(),
                    DataOffset = reader.ReadUInt(),
                    SecondOffset = reader.ReadUInt()
                };
                fields.Add(field);
            }

            int fieldIdx = 0;
            for (int i = 0; i < numClasses; i++)
            {
                long classOffset = reader.Position;

                Guid guid = reader.ReadGuid();
                Guid guid2 = reader.ReadGuid();

                if (guid == guid2)
                {
                    mapping.Add(guid, classes.Count);
                    classes.Add(null);
                    typeInfoGuids.Add(guid);
                    continue;
                }

                reader.Position -= 0x10;
                uint hash = reader.ReadUInt();
                uint fieldOffset = reader.ReadUInt();
                int fieldCount = reader.ReadByte();
                byte alignment = reader.ReadByte();
                ushort type = reader.ReadUShort();
                uint size = reader.ReadUInt();

                if ((alignment & 0x80) != 0)
                {
                    fieldCount += 0x100;
                    alignment &= 0x7F;
                }

                EbxClass ebxClass = new EbxClass
                {
                    NameHash = hash,
                    FieldIndex = (int)((classOffset - (fieldOffset - 0x08)) / 0x10),
                    FieldCount = (byte)fieldCount,
                    Alignment = alignment,
                    Size = (ushort)(size),
                    Type = (ushort)(type >> 1),
                    Index = i
                };
                if (patch)
                {
                    ebxClass.SecondSize = 1;
                }

                mapping.Add(guid, classes.Count);
                classes.Add(ebxClass);
                typeInfoGuids.Add(guid);

                fieldIdx += fieldCount;
            }
        }

        private void ReadRiff(NativeReader reader, bool patch)
        {
            uint fileSize = reader.ReadUInt();

            if (reader.ReadUInt(Endian.Big) != 0x45425854)
                throw new InvalidDataException("Not valid EBXT.");

            if (reader.ReadUInt(Endian.Big) != 0x5245464C)
                throw new InvalidDataException("Not valid REFL chunk.");
            uint reflSize = reader.ReadUInt();

            int classGuidCount = reader.ReadInt();

            for (int i = 0; i < classGuidCount; i++)
            {
                Guid classGuid = reader.ReadGuid();
                reader.Position -= 12;
                Guid typeInfoGuid = reader.ReadGuid();

                mapping.Add(typeInfoGuid, i);
                typeInfoGuids.Add(typeInfoGuid);
            }

            int numClasses = reader.ReadInt();
            for (int i = 0; i < numClasses; i++)
            {
                classes.Add(new EbxClass
                {
                    NameHash = reader.ReadUInt(),
                    FieldIndex = reader.ReadInt(),
                    FieldCount = reader.ReadUShort(),
                    Type = (ushort)(reader.ReadUShort() >> 1),
                    Size = reader.ReadUShort(),
                    Alignment = (byte)reader.ReadUShort(),
                    Index = i,
                    SecondSize = (ushort)(patch ? 1 : 0)
                });
            }

            for (int i = numClasses; i < classGuidCount; i++)
            {
                classes.Add(null);
            }

            int numFields = reader.ReadInt();
            for (int i = 0; i < numFields; i++)
            {
                fields.Add(new EbxField
                {
                    NameHash = reader.ReadUInt(),
                    DataOffset = reader.ReadUInt(),
                    Type = (ushort)(reader.ReadUShort() >> 1),
                    ClassRef = reader.ReadUShort()
                });
            }
        }

        public bool HasClass(Guid guid) => mapping.ContainsKey(guid);

        public EbxClass? GetClass(Guid guid) => !mapping.ContainsKey(guid) ? null : classes[mapping[guid]];

        public EbxClass? GetClass(int index) => index < classes.Count ? classes[index] : null;

        public Guid? GetGuid(EbxClass classType) => classType.Index < typeInfoGuids.Count ? typeInfoGuids[classType.Index] : null;

        public Guid? GetGuid(int index) => index < typeInfoGuids.Count ? typeInfoGuids[index] : null;

        public EbxField? GetField(int index) => index < fields.Count ? fields[index] : null;
    }
}
