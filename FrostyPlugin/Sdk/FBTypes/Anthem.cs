using System;
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
            ClassesSdkCreator.NextOffset = reader.ReadLong();
            Guid guid = reader.ReadGuid();
            Id = reader.ReadUShort();
            IsDataContainer = reader.ReadUShort();
            Padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            ParentClass = reader.ReadLong();

            reader.Position = typeInfoOffset;

            TypeInfo = new TypeInfo();
            TypeInfo.Read(reader);

            if (TypeInfo.ParentClass != 0)
                ParentClass = TypeInfo.ParentClass;

            reader.Position = ParentClass;
            if (reader.Position == thisOffset)
                ParentClass = 0;
        }
    }
    public class TypeInfo : ClassesSdkCreator.TypeInfo
    {
        public uint nameHash;

        public override void Read(MemoryReader reader)
        {
            nameHash = reader.ReadUInt();

            Flags = reader.ReadUShort();
            Flags >>= 1;

            Size = reader.ReadUShort();
            Guid = reader.ReadGuid();

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

            Name = "Class_" + nameHash.ToString("x8");
            if (AnthemDemo.Strings.classHash.ContainsKey(nameHash))
                Name = AnthemDemo.Strings.classHash[nameHash];
            else if (AnthemDemo.Strings.stringHash.ContainsKey(nameHash))
                Name = AnthemDemo.Strings.stringHash[nameHash];
            else
            {
                if (Type == 2) Name = "Struct_" + nameHash.ToString("x8");
                else if (Type == 3) Name = "Class_" + nameHash.ToString("x8");
                else if (Type == 8) Name = "Enum_" + nameHash.ToString("x8");
                else Name = "Unknown_" + nameHash.ToString("x8");
            }

            bool bReadFields = false;
            ParentClass = offsets[0];
            if (Type == 2 /* Structure */) { reader.Position = offsets[6]; bReadFields = true; }
            else if (Type == 3 /* Class */) { reader.Position = offsets[1]; bReadFields = true; }
            else if (Type == 8 /* Enum */)
            {
                ParentClass = 0;
                reader.Position = offsets[0];
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
                    fi.Read(reader, this.nameHash);
                    fi.Index = i;

                    Fields.Add(fi);
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
            Name = "Field_" + nameHash.ToString("x8");
            if (AnthemDemo.Strings.fieldHash.ContainsKey(classHash))
            {
                if (AnthemDemo.Strings.fieldHash[classHash].ContainsKey(nameHash))
                    Name = AnthemDemo.Strings.fieldHash[classHash][nameHash];
                else if (AnthemDemo.Strings.stringHash.ContainsKey(nameHash))
                    Name = AnthemDemo.Strings.stringHash[nameHash];
            }
            else if (AnthemDemo.Strings.stringHash.ContainsKey(nameHash))
                Name = AnthemDemo.Strings.stringHash[nameHash];

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
