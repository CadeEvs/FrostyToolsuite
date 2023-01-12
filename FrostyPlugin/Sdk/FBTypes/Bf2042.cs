using Frosty.Core.IO;
using FrostySdk;
using System;
using System.Collections.Generic;

namespace Frosty.Core.Sdk.Bf2042
{
    public class Strings
    {
        public static Dictionary<uint, string> stringHash = new Dictionary<uint, string>();
        public static Dictionary<uint, string> classHash = new Dictionary<uint, string>();
        public static Dictionary<uint, Dictionary<uint, string>> fieldHash = new Dictionary<uint, Dictionary<uint, string>>();
    }

    public class TypeInfo : ClassesSdkCreator.TypeInfo
    {
        private bool m_hasNames = !ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042);

        private uint m_nameHash;
        private uint m_signature;
        public override void Read(MemoryReader reader)
        {
            if (m_hasNames)
            {
                Name = reader.ReadNullTerminatedString();
            }
            m_nameHash = reader.ReadUInt();

            Flags = reader.ReadUShort();
            Flags >>= 1;

            Size = reader.ReadUShort();

            Guid = reader.ReadGuid();

            long nameSpaceOffset = reader.ReadLong();
            ArrayTypeOffset = reader.ReadLong();

            Alignment = reader.ReadUShort();
            FieldCount = reader.ReadUShort();
            m_signature = reader.ReadUInt();

            long[] offsets = new long[7];
            for (int i = 0; i < 7; i++)
            {
                offsets[i] = reader.ReadLong();
            }

            reader.Position = nameSpaceOffset;
            NameSpace = reader.ReadNullTerminatedString();

            if (!m_hasNames)
            {
                if (Strings.classHash.ContainsKey(m_nameHash))
                {
                    Name = Strings.classHash[m_nameHash];
                }
                else if (Strings.stringHash.ContainsKey(m_nameHash))
                {
                    Name = Strings.stringHash[m_nameHash];
                }
                else
                {
                    if (Type == 2)
                    {
                        Name = "Struct_" + m_nameHash.ToString("x8");
                    }
                    else if (Type == 3)
                    {
                        Name = "Class_" + m_nameHash.ToString("x8");
                    }
                    else if (Type == 8)
                    {
                        Name = "Enum_" + m_nameHash.ToString("x8");
                    }
                    else if (Type == 0x1c)
                    {
                        Name = "Delegate_" + m_nameHash.ToString("x8");
                    }
                    else if (Type == 0x18)
                    {
                        Name = "Function_" + m_nameHash.ToString("x8");
                    }
                    else
                    {
                        Name = "Unknown_" + m_nameHash.ToString("x8");
                    }
                }
            }

            bool bReadFields = false;
            ParentClass = offsets[0];
            if (Type == 2 /* Structure */)
            {
                reader.Position = offsets[6];
                bReadFields = true;
            }
            else if (Type == 3 /* Class */)
            {
                reader.Position = offsets[1];
                bReadFields = true;
            }
            else if (Type == 8 /* Enum */)
            {
                ParentClass = 0;
                reader.Position = offsets[0];
                bReadFields = true;
            }
            else if (Type == 0x1c /* Delegate */)
            {
                ParentClass = 0;
                reader.Position = offsets[0];
                for (int i = 0; i < FieldCount; i++)
                {
                    ParameterInfo pi = new ParameterInfo();
                    pi.Read(reader);
                    Parameters.Add(pi);
                }
            }
            else if (Type == 0x18 /* Function */)
            {
                ParentClass = 0;
                reader.Position = offsets[5];
                for (int i = 0; i < FieldCount; i++)
                {
                    ParameterInfo pi = new ParameterInfo();
                    pi.Read(reader);
                    Parameters.Add(pi);
                }
            }

            if (bReadFields)
            {
                for (int i = 0; i < FieldCount; i++)
                {
                    FieldInfo fi = new FieldInfo();
                    fi.Read(reader, m_nameHash);
                    fi.Index = i;

                    Fields.Add(fi);
                }
            }
        }

        public override void Modify(DbObject classObj, Dictionary<long, ClassesSdkCreator.ClassInfo> offsetClassInfoMapping)
        {
            var arrayType = (offsetClassInfoMapping.ContainsKey(ArrayTypeOffset)) ? offsetClassInfoMapping[ArrayTypeOffset] : null;
            classObj.SetValue("nameHash", m_nameHash);

            if (arrayType != null)
            {
                classObj.SetValue("arrayNameHash", arrayType.TypeInfo.As<TypeInfo>().m_nameHash);
            }

            classObj.SetValue("signature", m_signature);
        }
    }

    public class ClassInfo : ClassesSdkCreator.ClassInfo
    {
        public override void Read(MemoryReader reader)
        {
            long thisOffset = reader.Position;

            long typeInfoOffset = reader.ReadLong();

            long prevOffset = reader.ReadLong();

            ClassesSdkCreator.NextOffset = reader.ReadLong();

            Id = reader.ReadUShort();
            IsDataContainer = reader.ReadUShort();
            Padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            ParentClass = reader.ReadLong();

            reader.Position = typeInfoOffset;

            TypeInfo = new TypeInfo();
            TypeInfo.Read(reader);

            if (TypeInfo.ParentClass != 0)
            {
                ParentClass = TypeInfo.ParentClass;
            }

            if (ParentClass == thisOffset)
            {
                ParentClass = 0;
            }
        }
    }

    public class FieldInfo : ClassesSdkCreator.FieldInfo
    {

        private bool m_hasNames = !ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.Battlefield2042);
        private uint m_nameHash;
        public void Read(MemoryReader reader, uint classHash)
        {
            if (m_hasNames)
            {
                Name = reader.ReadNullTerminatedString();
            }
            m_nameHash = reader.ReadUInt();
            if (!m_hasNames)
            {
                Name = "Field_" + m_nameHash.ToString("x8");
                if (Strings.fieldHash.ContainsKey(classHash))
                {
                    if (Strings.fieldHash[classHash].ContainsKey(m_nameHash))
                    {
                        Name = Strings.fieldHash[classHash][m_nameHash];
                    }
                    else if (Strings.stringHash.ContainsKey(m_nameHash))
                    {
                        Name = Strings.stringHash[m_nameHash];
                    }
                }
                else if (Strings.stringHash.ContainsKey(m_nameHash))
                {
                    Name = Strings.stringHash[m_nameHash];
                }
            }
            Flags = reader.ReadUShort();
            Offset = reader.ReadUShort();
            TypeOffset = reader.ReadLong();
        }

        public override void Modify(DbObject fieldObj)
        {
            fieldObj.SetValue("nameHash", m_nameHash);
        }
    }

    public class ParameterInfo : ClassesSdkCreator.ParameterInfo
    {
        public override void Read(MemoryReader reader)
        {
            Name = reader.ReadNullTerminatedString();
            TypeOffset = reader.ReadLong();
            Type = reader.ReadLong();
            long defaultValueOffset = reader.ReadLong();
            if (defaultValueOffset == 0)
            {
                DefaultValue = null;
            }
            else
            {

            }
        }

        public override void Modify(DbObject fieldObj)
        {
        }
    }
}
