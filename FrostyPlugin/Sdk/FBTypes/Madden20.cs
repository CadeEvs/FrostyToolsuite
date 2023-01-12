using Frosty.Core.IO;
using FrostySdk;
using System;
using System.Collections.Generic;

namespace Frosty.Core.Sdk.Madden20
{
    public class TypeInfo : ClassesSdkCreator.TypeInfo
    {
        uint nameHash;
        public override void Read(MemoryReader reader)
        {
            Name = reader.ReadNullTerminatedString();
            nameHash = reader.ReadUInt();

            Flags = reader.ReadUShort();
            Flags >>= 1;

            Size = reader.ReadUInt();
            reader.Position -= 4;
            Size = reader.ReadUShort();

            Guid = reader.ReadGuid();
            //reader.ReadUShort();

            //padding1 = reader.ReadUShort();
            long nameSpaceOffset = reader.ReadLong();
            ArrayTypeOffset = reader.ReadLong();

            Alignment = reader.ReadUShort();
            FieldCount = reader.ReadUShort();
            Padding3 = reader.ReadUInt();

            long[] offsets = new long[7];
            for (int i = 0; i < 7; i++)
                offsets[i] = reader.ReadLong();

            reader.Position = nameSpaceOffset;
            NameSpace = reader.ReadNullTerminatedString();

            bool bReadFields = false;
            
            ParentClass = offsets[0];
            if (Type == 2 /* Structure */) { reader.Position = offsets[6]; bReadFields = true; }
            else if (Type == 3 /* Class */)
            {
                reader.Position = offsets[1];
                bReadFields = true;
            }
            else if (Type == 8 /* Enum */)
            {
                ParentClass = 0;
                reader.Position = offsets[0];

                //if (reader.Position == offsets[0])
                //{
                //    reader.Position = offsets[1];
                //    long newOffset = reader.ReadLong();
                //    reader.Position = newOffset;

                //    uint z = fieldCount;
                //    while (z > 0)
                //    {
                //        newOffset = reader.ReadLong();
                //        long nextOffset = reader.ReadLong();
                //        reader.Position = newOffset;
                //        reader.Position += 8;

                //        long nameOffset = reader.ReadLong();
                //        while (nameOffset != 0)
                //        {
                //            reader.Position -= 8;

                //            FieldInfo f = new FieldInfo();
                //            f.Read(reader);

                //            fields.Add(f);

                //            z--;
                //            if (z <= 0)
                //                break;

                //            nameOffset = reader.ReadLong();
                //        }

                //        if (z <= 0)
                //            break;

                //        if (nextOffset == 0)
                //            break;

                //        reader.Position = nextOffset;
                //    }
                //    bReadFields = false;
                //}
                //else
                {
                    reader.Position = offsets[0];
                    bReadFields = true;
                }
            }

            if (bReadFields)
            {
                for (int i = 0; i < FieldCount; i++)
                {
                    FieldInfo fi = new FieldInfo();
                    fi.Read(reader);
                    fi.Index = i;

                    Fields.Add(fi);
                }
            }
        }

        public override void Modify(DbObject classObj, Dictionary<long, ClassesSdkCreator.ClassInfo> offsetClassInfoMapping)
        {
            var arrayType = (offsetClassInfoMapping.ContainsKey(ArrayTypeOffset)) ? offsetClassInfoMapping[ArrayTypeOffset] : null;
            classObj.SetValue("nameHash", nameHash);

            if (arrayType != null)
            {
                classObj.SetValue("arrayNameHash", arrayType.TypeInfo.As<TypeInfo>().nameHash);
            }
        }
    }

    public class ClassInfo : ClassesSdkCreator.ClassInfo
    {
        public override void Read(MemoryReader reader)
        {
            long thisOffset = reader.Position;

            long typeInfoOffset = reader.ReadLong();

            ClassesSdkCreator.NextOffset = reader.ReadLong();
            Guid guid = Guid.Empty;

            long prevOffset = reader.ReadLong();

            Id = reader.ReadUShort();
            IsDataContainer = reader.ReadUShort();
            Padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            ParentClass = reader.ReadLong();

            reader.Position = typeInfoOffset;

            TypeInfo = new TypeInfo();
            TypeInfo.Read(reader);

            if (TypeInfo.ParentClass != 0)
                ParentClass = TypeInfo.ParentClass;

            if (ParentClass == thisOffset)
                ParentClass = 0;
        }
    }

    public class FieldInfo : ClassesSdkCreator.FieldInfo
    {
        uint nameHash;
        public override void Read(MemoryReader reader)
        {
            Name = reader.ReadNullTerminatedString();
            nameHash = reader.ReadUInt();

            Flags = reader.ReadUShort();
            Offset = reader.ReadUShort();
            TypeOffset = reader.ReadLong();
        }

        public override void Modify(DbObject fieldObj)
        {
            fieldObj.SetValue("nameHash", nameHash);
        }
    }
}
