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
            name = reader.ReadNullTerminatedString();
            nameHash = reader.ReadUInt();

            flags = reader.ReadUShort();
            flags >>= 1;

            size = reader.ReadUInt();
            reader.Position -= 4;
            size = reader.ReadUShort();

            guid = reader.ReadGuid();
            //reader.ReadUShort();

            //padding1 = reader.ReadUShort();
            long nameSpaceOffset = reader.ReadLong();
            long arrayTypeOffset = reader.ReadLong();

            alignment = reader.ReadUShort();
            fieldCount = reader.ReadUShort();
            padding3 = reader.ReadUInt();

            long[] offsets = new long[7];
            for (int i = 0; i < 7; i++)
                offsets[i] = reader.ReadLong();

            reader.Position = nameSpaceOffset;
            nameSpace = reader.ReadNullTerminatedString();

            bool bReadFields = false;
            
            parentClass = offsets[0];
            if (Type == 2 /* Structure */) { reader.Position = offsets[6]; bReadFields = true; }
            else if (Type == 3 /* Class */)
            {
                reader.Position = offsets[1];
                bReadFields = true;
            }
            else if (Type == 8 /* Enum */)
            {
                parentClass = 0;
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
                for (int i = 0; i < fieldCount; i++)
                {
                    FieldInfo fi = new FieldInfo();
                    fi.Read(reader);
                    fi.index = i;

                    fields.Add(fi);
                }
            }
        }

        public override void Modify(DbObject classObj, Dictionary<long, ClassesSdkCreator.ClassInfo> offsetClassInfoMapping)
        {
            var arrayType = (offsetClassInfoMapping.ContainsKey(arrayTypeOffset)) ? offsetClassInfoMapping[arrayTypeOffset] : null;
            classObj.SetValue("nameHash", nameHash);

            if (arrayType != null)
            {
                classObj.SetValue("arrayNameHash", arrayType.typeInfo.As<TypeInfo>().nameHash);
            }
        }
    }

    public class ClassInfo : ClassesSdkCreator.ClassInfo
    {
        public override void Read(MemoryReader reader)
        {
            long thisOffset = reader.Position;

            long typeInfoOffset = reader.ReadLong();
            ClassesSdkCreator.offset = reader.ReadLong();
            Guid guid = Guid.Empty;

            id = reader.ReadUShort();
            isDataContainer = reader.ReadUShort();
            padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            parentClass = reader.ReadLong();

            reader.Position = typeInfoOffset;

            typeInfo = new TypeInfo();
            typeInfo.Read(reader);

            if (typeInfo.parentClass != 0)
                parentClass = typeInfo.parentClass;

            if (parentClass == thisOffset)
                parentClass = 0;
        }
    }

    public class FieldInfo : ClassesSdkCreator.FieldInfo
    {
        uint nameHash;
        public override void Read(MemoryReader reader)
        {
            name = reader.ReadNullTerminatedString();
            nameHash = reader.ReadUInt();

            flags = reader.ReadUShort();
            offset = reader.ReadUShort();
            typeOffset = reader.ReadLong();
        }

        public override void Modify(DbObject fieldObj)
        {
            fieldObj.SetValue("nameHash", nameHash);
        }
    }
}
