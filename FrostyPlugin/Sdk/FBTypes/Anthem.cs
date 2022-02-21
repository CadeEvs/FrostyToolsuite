using System.Collections.Generic;
using Frosty.Core.IO;
using FrostySdk;

namespace Frosty.Core.Sdk.Anthem
{
    public class ClassInfo : ClassesSdkCreator.ClassInfo
    {
        public override void Read(MemoryReader reader)
        {
            long thisOffset = reader.Position;

            long typeInfoOffset = reader.ReadLong();
            ClassesSdkCreator.offset = reader.ReadLong();
            id = reader.ReadUShort();
            isDataContainer = reader.ReadUShort();
            padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            parentClass = reader.ReadLong();

            reader.Position = typeInfoOffset;
            //if (typeInfoOffset == 0x145029EB0)
            //    Console.WriteLine();

            typeInfo = new TypeInfo();
            typeInfo.Read(reader);

            if (typeInfo.parentClass != 0)
                parentClass = typeInfo.parentClass;

            reader.Position = parentClass;
            if (reader.Position == thisOffset)
                parentClass = 0;
        }
    }
    public class TypeInfo : ClassesSdkCreator.TypeInfo
    {
        public uint nameHash;

        public override void Read(MemoryReader reader)
        {
            nameHash = reader.ReadUInt();

            flags = reader.ReadUShort();
            flags >>= 1;

            size = reader.ReadUShort();
            guid = reader.ReadGuid();

            long nameSpaceOffset = reader.ReadLong();
            long arrayTypeOffset = arrayTypeOffset = reader.ReadLong();

            alignment = reader.ReadUShort();
            fieldCount = reader.ReadUShort();
            padding3 = reader.ReadUInt();

            long[] offsets = new long[7];
            for (int i = 0; i < 7; i++)
                offsets[i] = reader.ReadLong();

            reader.Position = nameSpaceOffset;
            nameSpace = reader.ReadNullTerminatedString();

            name = "Class_" + nameHash.ToString("x8");
            if (AnthemDemo.Strings.classHash.ContainsKey(nameHash))
                name = AnthemDemo.Strings.classHash[nameHash];
            else if (AnthemDemo.Strings.stringHash.ContainsKey(nameHash))
                name = AnthemDemo.Strings.stringHash[nameHash];
            else
            {
                if (Type == 2) name = "Struct_" + nameHash.ToString("x8");
                else if (Type == 3) name = "Class_" + nameHash.ToString("x8");
                else if (Type == 8) name = "Enum_" + nameHash.ToString("x8");
                else name = "Unknown_" + nameHash.ToString("x8");
            }

            bool bReadFields = false;
            parentClass = offsets[0];
            if (Type == 2 /* Structure */) { reader.Position = offsets[6]; bReadFields = true; }
            else if (Type == 3 /* Class */) { reader.Position = offsets[1]; bReadFields = true; }
            else if (Type == 8 /* Enum */)
            {
                parentClass = 0;
                reader.Position = offsets[0];
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
                    fi.Read(reader, this.nameHash);
                    fi.index = i;

                    fields.Add(fi);
                }
            }
        }

        public override void Modify(DbObject classObj, Dictionary<long, ClassesSdkCreator.ClassInfo> offsetClassInfoMapping)
        {
            classObj.SetValue("nameHash", nameHash);
        }
    }
    public class FieldInfo : ClassesSdkCreator.FieldInfo
    {
        public uint nameHash;

        public void Read(MemoryReader reader, uint classHash)
        {
            nameHash = reader.ReadUInt();
            name = "Field_" + nameHash.ToString("x8");
            if (AnthemDemo.Strings.fieldHash.ContainsKey(classHash))
            {
                if (AnthemDemo.Strings.fieldHash[classHash].ContainsKey(nameHash))
                    name = AnthemDemo.Strings.fieldHash[classHash][nameHash];
                else if (AnthemDemo.Strings.stringHash.ContainsKey(nameHash))
                    name = AnthemDemo.Strings.stringHash[nameHash];
            }
            else if (AnthemDemo.Strings.stringHash.ContainsKey(nameHash))
                name = AnthemDemo.Strings.stringHash[nameHash];

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
