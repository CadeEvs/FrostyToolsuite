using System.Collections.Generic;

namespace Frosty.Core.Sdk.AnthemDemo
{
    public class Strings
    {
        public static Dictionary<uint, string> stringHash = new Dictionary<uint, string>();
        public static Dictionary<uint, string> classHash = new Dictionary<uint, string>();
        public static Dictionary<uint, Dictionary<uint, string>> fieldHash = new Dictionary<uint, Dictionary<uint, string>>();
    }
    //public class ClassInfo : ClassesSdkCreator.ClassInfo
    //{
    //    public override void Read(MemoryReader reader)
    //    {
    //        long thisOffset = reader.Position;

    //        long typeInfoOffset = reader.ReadLong();
    //        ClassesSdkCreator.offset = reader.ReadLong();
    //        id = reader.ReadUShort();
    //        isDataContainer = reader.ReadUShort();
    //        padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
    //        parentClass = reader.ReadLong();

    //        reader.Position = typeInfoOffset;
    //        //if (typeInfoOffset == 0x145029EB0)
    //        //    Console.WriteLine();

    //        typeInfo = new TypeInfo();
    //        typeInfo.Read(reader);

    //        if (typeInfo.parentClass != 0)
    //            parentClass = typeInfo.parentClass;

    //        reader.Position = parentClass;
    //        if (reader.Position == thisOffset)
    //            parentClass = 0;
    //    }
    //}
    //public class TypeInfo : ClassesSdkCreator.TypeInfo
    //{
    //    public uint nameHash;

    //    public override void Read(MemoryReader reader)
    //    {
    //        name = reader.ReadNullTerminatedString();
    //        nameHash = reader.ReadUInt();

    //        flags = reader.ReadUShort();
    //        flags >>= 1;

    //        size = reader.ReadUShort();
    //        guid = reader.ReadGuid();


    //        long nameSpaceOffset = reader.ReadLong();
    //        long arrayTypeOffset = reader.ReadLong();

    //        alignment = reader.ReadUShort();
    //        fieldCount = reader.ReadUShort();
    //        padding3 = reader.ReadUInt();

    //        long[] offsets = new long[7];
    //        for (int i = 0; i < 7; i++)
    //            offsets[i] = reader.ReadLong();

    //        if (!Strings.classHash.ContainsKey(nameHash))
    //            Strings.classHash.Add(nameHash, name);
    //        if (!Strings.stringHash.ContainsKey(nameHash))
    //            Strings.stringHash.Add(nameHash, name);

    //        reader.Position = nameSpaceOffset;
    //        nameSpace = reader.ReadNullTerminatedString();

    //        bool bReadFields = false;
    //        parentClass = offsets[0];
    //        if (Type == 2 /* Structure */) { reader.Position = offsets[6]; bReadFields = true; }
    //        else if (Type == 3 /* Class */) { reader.Position = offsets[1]; bReadFields = true; }
    //        else if (Type == 8 /* Enum */)
    //        {
    //            parentClass = 0;
    //            reader.Position = offsets[0];
    //            if (reader.Position == offsets[0])
    //            {
    //                reader.Position = offsets[1];
    //                long newOffset = reader.ReadLong();
    //                reader.Position = newOffset;

    //                uint z = fieldCount;
    //                while (z > 0)
    //                {
    //                    newOffset = reader.ReadLong();
    //                    long nextOffset = reader.ReadLong();
    //                    reader.Position = newOffset;
    //                    reader.Position += 8;

    //                    long nameOffset = reader.ReadLong();
    //                    while (nameOffset != 0)
    //                    {
    //                        reader.Position -= 8;

    //                        FieldInfo f = new FieldInfo();
    //                        f.Read(reader, flags);

    //                        fields.Add(f);

    //                        z--;
    //                        if (z <= 0)
    //                            break;

    //                        nameOffset = reader.ReadLong();
    //                    }

    //                    if (z <= 0)
    //                        break;

    //                    if (nextOffset == 0)
    //                        break;

    //                    reader.Position = nextOffset;
    //                }
    //                bReadFields = false;
    //            }
    //            else
    //            {
    //                bReadFields = true;
    //            }
    //        }

    //        if (bReadFields)
    //        {
    //            for (int i = 0; i < fieldCount; i++)
    //            {
    //                FieldInfo fi = new FieldInfo();
    //                fi.Read(reader, this.nameHash);

    //                fields.Add(fi);
    //            }
    //        }
    //    }
    //}
    //public class FieldInfo : ClassesSdkCreator.FieldInfo
    //{
    //    public uint nameHash;

    //    public void Read(MemoryReader reader, uint classHash)
    //    {
    //        name = reader.ReadNullTerminatedString();
    //        nameHash = reader.ReadUInt();

    //        flags = reader.ReadUShort();
    //        offset = reader.ReadUShort();
    //        typeOffset = reader.ReadLong();

    //        if (!Strings.fieldHash.ContainsKey(classHash))
    //            Strings.fieldHash.Add(classHash, new Dictionary<uint, string>());
    //        if (!Strings.fieldHash[classHash].ContainsKey(nameHash))
    //            Strings.fieldHash[classHash].Add(nameHash, name);
    //        if (!Strings.stringHash.ContainsKey(nameHash))
    //            Strings.stringHash.Add(nameHash, name);
    //    }
    //}
}
