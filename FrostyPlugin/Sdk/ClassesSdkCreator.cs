﻿using Frosty.Core.IO;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Frosty.Core.Sdk
{
    #region -- EXE Header --
    class IMAGE_DOS_HEADER
    {      // DOS .EXE header
        public ushort e_magic;                     // Magic number
        public ushort e_cblp;                      // Bytes on last page of file
        public ushort e_cp;                        // Pages in file
        public ushort e_crlc;                      // Relocations
        public ushort e_cparhdr;                   // Size of header in paragraphs
        public ushort e_minalloc;                  // Minimum extra paragraphs needed
        public ushort e_maxalloc;                  // Maximum extra paragraphs needed
        public ushort e_ss;                        // Initial (relative) SS value
        public ushort e_sp;                        // Initial SP value
        public ushort e_csum;                      // Checksum
        public ushort e_ip;                        // Initial IP value
        public ushort e_cs;                        // Initial (relative) CS value
        public ushort e_lfarlc;                    // File address of relocation table
        public ushort e_ovno;                      // Overlay number
        public ushort[] e_res = new ushort[4];                    // Reserved words
        public ushort e_oemid;                     // OEM identifier (for e_oeminfo)
        public ushort e_oeminfo;                   // OEM information; e_oemid specific
        public ushort[] e_res2 = new ushort[10];                  // Reserved words
        public int e_lfanew;                    // File address of new exe header

        public void Read(NativeReader reader)
        {
            e_magic = reader.ReadUShort();
            e_cblp = reader.ReadUShort();
            e_cp = reader.ReadUShort();
            e_crlc = reader.ReadUShort();
            e_cparhdr = reader.ReadUShort();
            e_minalloc = reader.ReadUShort();
            e_maxalloc = reader.ReadUShort();
            e_ss = reader.ReadUShort();
            e_sp = reader.ReadUShort();
            e_csum = reader.ReadUShort();
            e_ip = reader.ReadUShort();
            e_cs = reader.ReadUShort();
            e_lfarlc = reader.ReadUShort();
            e_ovno = reader.ReadUShort();
            for (int i = 0; i < 4; i++) { e_res[i] = reader.ReadUShort(); }
            e_oemid = reader.ReadUShort();
            e_oeminfo = reader.ReadUShort();
            for (int i = 0; i < 10; i++) { e_res2[i] = reader.ReadUShort(); }
            e_lfanew = reader.ReadInt();
        }
    }

    class IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;

        public void Read(NativeReader reader)
        {
            Machine = reader.ReadUShort();
            NumberOfSections = reader.ReadUShort();
            TimeDateStamp = reader.ReadUInt();
            PointerToSymbolTable = reader.ReadUInt();
            NumberOfSymbols = reader.ReadUInt();
            SizeOfOptionalHeader = reader.ReadUShort();
            Characteristics = reader.ReadUShort();
        }
    }

    struct IMAGE_SECTION_HEADER
    {
        public uint PhysicalAddress => Address;
        public uint VirtualSize => Address;

        public string Name;
        public uint Address;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;

        public void Read(NativeReader reader)
        {
            Name = reader.ReadSizedString(8);
            Address = reader.ReadUInt();
            VirtualAddress = reader.ReadUInt();
            SizeOfRawData = reader.ReadUInt();
            PointerToRawData = reader.ReadUInt();
            PointerToRelocations = reader.ReadUInt();
            PointerToLinenumbers = reader.ReadUInt();
            NumberOfRelocations = reader.ReadUShort();
            NumberOfLinenumbers = reader.ReadUShort();
            Characteristics = reader.ReadUInt();
        }
    }

    struct IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;
    }

    class IMAGE_OPTIONAL_HEADER
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public ulong ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public ulong SizeOfStackReserve;
        public ulong SizeOfStackCommit;
        public ulong SizeOfHeapReserve;
        public ulong SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;
        IMAGE_DATA_DIRECTORY[] DataDirectory = new IMAGE_DATA_DIRECTORY[16];

        public void Read(NativeReader reader)
        {
            Magic = reader.ReadUShort();
            MajorLinkerVersion = reader.ReadByte();
            MinorLinkerVersion = reader.ReadByte();
            SizeOfCode = reader.ReadUInt();
            SizeOfInitializedData = reader.ReadUInt();
            SizeOfUninitializedData = reader.ReadUInt();
            AddressOfEntryPoint = reader.ReadUInt();
            BaseOfCode = reader.ReadUInt();
            ImageBase = reader.ReadULong();
            SectionAlignment = reader.ReadUInt();
            FileAlignment = reader.ReadUInt();
            MajorOperatingSystemVersion = reader.ReadUShort();
            MinorOperatingSystemVersion = reader.ReadUShort();
            MajorImageVersion = reader.ReadUShort();
            MinorImageVersion = reader.ReadUShort();
            MajorSubsystemVersion = reader.ReadUShort();
            MinorSubsystemVersion = reader.ReadUShort();
            Win32VersionValue = reader.ReadUInt();
            SizeOfImage = reader.ReadUInt();
            SizeOfHeaders = reader.ReadUInt();
            CheckSum = reader.ReadUInt();
            Subsystem = reader.ReadUShort();
            DllCharacteristics = reader.ReadUShort();
            SizeOfStackReserve = reader.ReadULong();
            SizeOfStackCommit = reader.ReadULong();
            SizeOfHeapReserve = reader.ReadULong();
            SizeOfHeapCommit = reader.ReadULong();
            LoaderFlags = reader.ReadUInt();
            NumberOfRvaAndSizes = reader.ReadUInt();

            for (int i = 0; i < 16; i++)
            {
                DataDirectory[i].VirtualAddress = reader.ReadUInt();
                DataDirectory[i].Size = reader.ReadUInt();
            }
        }
    }
    #endregion

    public class ModuleWriter : IDisposable
    {
        private List<string> CreateOldFB3PFs()
            => new List<string>
                {
                    "RenderFormat_BC1_UNORM",
                    "RenderFormat_BC1A_UNORM",
                    "RenderFormat_BC2_UNORM",
                    "RenderFormat_BC3_UNORM",
                    "RenderFormat_BC3A_UNORM",
                    "RenderFormat_DXN",
                    "RenderFormat_BC7_UNORM",
                    "RenderFormat_RGB565",
                    "RenderFormat_RGB888",
                    "RenderFormat_ARGB1555",
                    "RenderFormat_ARGB4444",
                    "RenderFormat_ARGB8888",
                    "RenderFormat_L8",
                    "RenderFormat_L16",
                    "RenderFormat_ABGR16",
                    "RenderFormat_ABGR16F",
                    "RenderFormat_ABGR32F",
                    "RenderFormat_R16F",
                    "RenderFormat_R32F",
                    "RenderFormat_NormalDXN",
                    "RenderFormat_NormalDXT1",
                    "RenderFormat_NormalDXT5",
                    "RenderFormat_NormalDXT5RGA",
                    "RenderFormat_RG8",
                    "RenderFormat_GR16",
                    "RenderFormat_GR16F",
                    "RenderFormat_D16",
                    "RenderFormat_D24",
                    "RenderFormat_D24S8",
                    "RenderFormat_D24FS8",
                    "RenderFormat_D32F",
                    "RenderFormat_D32FS8",
                    "RenderFormat_S8",
                    "RenderFormat_ABGR32",
                    "RenderFormat_GR32F",
                    "RenderFormat_A2R10G10B10",
                    "RenderFormat_R11G11B10F",
                    "RenderFormat_ABGR16_SNORM",
                    "RenderFormat_ABGR16_UINT",
                    "RenderFormat_L16_UINT",
                    "RenderFormat_L32",
                    "RenderFormat_GR16_UINT",
                    "RenderFormat_GR32_UINT",
                    "RenderFormat_ETC1",
                    "RenderFormat_ETC2_RGB",
                    "RenderFormat_ETC2_RGBA",
                    "RenderFormat_ETC2_RGB_A1",
                    "RenderFormat_PVRTC1_4BPP_RGBA",
                    "RenderFormat_PVRTC1_4BPP_RGB",
                    "RenderFormat_PVRTC1_2BPP_RGBA",
                    "RenderFormat_PVRTC1_2BPP_RGB",
                    "RenderFormat_PVRTC2_4BPP",
                    "RenderFormat_PVRTC2_2BPP",
                    "RenderFormat_R8",
                    "RenderFormat_R9G9B9E5F",
                    "RenderFormat_Unknown"
                };

        private DbObject classList;
        private string filename;

        public ModuleWriter(string inFilename, DbObject inList)
        {
            filename = inFilename;
            classList = inList;
        }

        public void Write(uint version)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FrostySdk.Attributes;");
            sb.AppendLine("using FrostySdk.Managers;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("using FrostySdk;");
            sb.AppendLine();
            sb.AppendLine("[assembly: SdkVersion(" + (int)version + ")]");
            sb.AppendLine();
            sb.AppendLine("namespace FrostySdk.Ebx");
            sb.AppendLine("{");
            {
                foreach (DbObject classObj in classList)
                {
                    EbxFieldType type = (EbxFieldType)classObj.GetValue<int>("type");
                    if (type == EbxFieldType.Enum)
                        sb.Append(WriteEnum(classObj));
                    else if (type == EbxFieldType.Struct || type == EbxFieldType.Pointer)
                        sb.Append(WriteClass(classObj));
                    else if (type != EbxFieldType.Array && type != EbxFieldType.Delegate && (uint)type != 0x1c && classObj.HasValue("basic"))
                    {
                        sb.AppendLine("namespace Reflection\r\n{");
                        sb.Append(WriteClass(classObj));
                        sb.AppendLine("}");
                    }
                    else if (type == EbxFieldType.Delegate || (uint)type == 0x1c)
                    {
                        sb.AppendLine("namespace Reflection\r\n{");
                        sb.AppendLine("[" + typeof(DisplayNameAttribute).Name + "(\"" + classObj.GetValue<string>("name") + "\")]");
                        sb.AppendLine("[" + typeof(GuidAttribute).Name + "(\"" + classObj.GetValue<Guid>("guid") + "\")]");
                        sb.AppendLine($"[{typeof(HashAttribute).Name}({classObj.GetValue<int>("nameHash")})]");
                        sb.AppendLine("public class Delegate_" + classObj.GetValue<Guid>("guid").ToString().Replace('-', '_') + " { }\r\n}");
                    }
                }

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare)
                {
                    DbObject newEnum = DbObject.CreateObject();
                    newEnum.SetValue("name", "RenderFormat");

                    List<string> formats = CreateOldFB3PFs();
                    DbObject fields = DbObject.CreateList();

                    int value = 0;
                    foreach (string format in formats)
                    {
                        DbObject field = DbObject.CreateObject();
                        field.SetValue("name", format);
                        field.SetValue("value", value++);
                        fields.Add(field);
                    }

                    newEnum.SetValue("fields", fields);
                    sb.Append(WriteEnum(newEnum));
                }
            }
            sb.AppendLine("}");

            using (NativeWriter writer = new NativeWriter(new FileStream("temp.cs", FileMode.Create)))
                writer.WriteLine(sb.ToString());

            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerParameters compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = filename,
                CompilerOptions = "-define:DV_" + (int)ProfilesLibrary.DataVersion
            };

            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("FrostySdk.dll");

            CompilerResults results = provider.CompileAssemblyFromFile(compilerParams, "temp.cs");
            File.Delete("temp.cs");

#if FROSTY_ALPHA || FROSTY_DEVELOPER
            if (results.Errors.Count > 0)
            {
                using (NativeWriter writer = new NativeWriter(new FileStream("Errors.txt", FileMode.Create)))
                {
                    foreach (CompilerError error in results.Errors)
                    {
                        writer.WriteLine("[Line: " + error.Line + "]: " + error.ErrorText);
                    }
                }
            }
#endif
        }

        private string WriteEnum(DbObject enumObj)
        {
            StringBuilder sb = new StringBuilder();
            {
                sb.Append(WriteClassAttributes(enumObj));
                sb.AppendLine("public enum " + enumObj.GetValue<string>("name"));
                sb.AppendLine("{");
                foreach (DbObject fieldObj in enumObj.GetValue<DbObject>("fields"))
                {
                    sb.AppendLine(fieldObj.GetValue<string>("name") + " = " + fieldObj.GetValue<int>("value") + ",");
                }
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        private string WriteClass(DbObject classObj)
        {
            if (classObj.GetValue<string>("name") == "char")
                return "";

            StringBuilder sb = new StringBuilder();
            {
                string parent = classObj.GetValue<string>("parent", "").Replace(':', '_');
                EbxFieldType type = (EbxFieldType)classObj.GetValue<int>("type");
                DbObject meta = classObj.GetValue<DbObject>("meta");

                string className = classObj.GetValue<string>("name").Replace(':', '_');

                sb.Append(WriteClassAttributes(classObj));
                sb.AppendLine("public class " + className + (parent != "" ? " : " + parent : ""));
                sb.AppendLine("{");

                if (/*parent == "" &&*/ type == EbxFieldType.Pointer)
                {
                    if (parent == "DataContainer")
                    {
                        // add Id field to non asset types
                        sb.AppendLine("[" + typeof(IsTransientAttribute).Name + "]");
                        if (!classObj.HasValue("isData"))
                            sb.AppendLine("[" + typeof(IsHiddenAttribute).Name + "]");
                        sb.AppendLine("[" + typeof(DisplayNameAttribute).Name + "(\"Id\")]");
                        sb.AppendLine("[" + typeof(CategoryAttribute).Name + "(\"Annotations\")]");
                        sb.AppendLine("[" + typeof(EbxFieldMetaAttribute).Name + "(8310, 8u, null, false, 0)]");
                        sb.AppendLine("[" + typeof(FieldIndexAttribute).Name + "(-2)]");
                        sb.AppendLine("public CString __Id\r\n{\r\nget\r\n{\r\nreturn GetId();\r\n}\r\nset { __id = value; }\r\n}\r\n");
                        sb.AppendLine("protected CString __id = new CString();");
                    }

                    if (parent == "")
                    {
                        // add guid field
                        sb.AppendLine("[" + typeof(IsTransientAttribute).Name + "]");
                        sb.AppendLine("[" + typeof(IsReadOnlyAttribute).Name + "]");
                        sb.AppendLine("[" + typeof(DisplayNameAttribute).Name + "(\"Guid\")]");
                        sb.AppendLine("[" + typeof(CategoryAttribute).Name + "(\"Annotations\")]");
                        //sb.AppendLine("[" + typeof(EditorAttribute).Name + "(\"Struct\")]");
                        sb.AppendLine("[" + typeof(EbxFieldMetaAttribute).Name + "(24918, 1, null, false, 0)]");
                        sb.AppendLine("[" + typeof(FieldIndexAttribute).Name + "(-1)]");
                        sb.AppendLine("public AssetClassGuid __InstanceGuid { get { return __Guid; } }");

                        sb.AppendLine("protected AssetClassGuid __Guid;");
                        sb.AppendLine("public AssetClassGuid GetInstanceGuid() { return __Guid; }");
                        sb.AppendLine("public void SetInstanceGuid(AssetClassGuid newGuid) { __Guid = newGuid; }");
                    }
                }

                // write out fields/properties
                bool isAssetClass = classObj.GetValue<string>("name").Equals("Asset");
                bool addedGetId = false;

                foreach (DbObject fieldObj in classObj.GetValue<DbObject>("fields"))
                {
                    sb.Append(WriteField(fieldObj));
                    if (!isAssetClass && fieldObj.GetValue<string>("name").Equals("Name", StringComparison.OrdinalIgnoreCase) && type == EbxFieldType.Pointer)
                    {
                        EbxFieldType fieldType = (EbxFieldType)fieldObj.GetValue<int>("type");
                        if (fieldType == EbxFieldType.CString)
                        {
                            Type tmpType = typeof(EbxClassMetaAttribute);
                            string namespaceName = tmpType.GetProperties()[4].Name;
                            tmpType = typeof(GlobalAttributes);
                            string displayModuleName = tmpType.GetFields()[0].Name;
                            tmpType = typeof(CString);
                            string funcName1 = tmpType.GetMethods()[0].Name;
                            string funcName2 = tmpType.GetMethods()[3].Name;

                            if (classObj.HasValue("isData"))
                            {
                                string fieldName = fieldObj.GetValue<string>("name");
                                sb.AppendLine("protected virtual CString GetId()\r\n{");
                                sb.AppendLine("if (__id != \"\") return __id;");
                                sb.AppendLine("if (_" + fieldName + " != \"\") return _" + fieldName + "." + funcName1 + "();");
                                sb.AppendLine("if (" + typeof(GlobalAttributes).Name + "." + displayModuleName + ")\r\n{\r\n" + typeof(EbxClassMetaAttribute).Name + " attr = GetType().GetCustomAttribute<" + typeof(EbxClassMetaAttribute).Name + ">();\r\nif (attr != null && attr." + namespaceName + " != \"\")\r\nreturn attr." + namespaceName + " + \".\" + GetType().Name;\r\n}\r\nreturn GetType().Name;");
                                sb.AppendLine("}");
                                addedGetId = true;
                            }
                            else
                            { 
                                string fieldName = fieldObj.GetValue<string>("name");
                                sb.AppendLine("protected override CString GetId()\r\n{");
                                sb.AppendLine("if (__id != \"\") return __id;");
                                sb.AppendLine("if (_" + fieldName + " != \"\") return _" + fieldName + "." + funcName1 + "();\r\nreturn base.GetId();");
                                sb.AppendLine("}");
                            }
                        }
                    }
                }

                if (parent == "DataContainer" && !addedGetId)
                {
                    Type tmpType = typeof(EbxClassMetaAttribute);
                    string namespaceName = tmpType.GetProperties()[4].Name;
                    tmpType = typeof(GlobalAttributes);
                    string displayModuleName = tmpType.GetFields()[0].Name;

                    sb.AppendLine("protected virtual CString GetId()\r\n{");
                    sb.AppendLine("if (__id == \"\")\r\n{\r\nif (" + typeof(GlobalAttributes).Name + "." + displayModuleName + ")\r\n{\r\n" + typeof(EbxClassMetaAttribute).Name + " attr = GetType().GetCustomAttribute<" + typeof(EbxClassMetaAttribute).Name + ">();\r\nif (attr != null && attr." + namespaceName + " != \"\")\r\nreturn attr." + namespaceName + " + \".\" + GetType().Name;\r\n}\r\nreturn GetType().Name;\r\n}\r\nreturn __id;");
                    sb.AppendLine("}");
                }

                // write out custom functions
                if (meta != null)
                {
                    // constructor
                    if (meta.HasValue("constructor"))
                    {
                        sb.AppendLine("public " + classObj.GetValue<string>("name").Replace(':', '_') + "()");
                        sb.AppendLine("{");
                        sb.AppendLine(meta.GetValue<string>("constructor"));
                        sb.AppendLine("}");
                    }

                    sb.AppendLine(meta.GetValue<string>("functions", ""));
                }

                if (type == EbxFieldType.Struct && classObj.GetValue<DbObject>("fields").Count > 0)
                {
                    // Equals override
                    sb.AppendLine("public override bool Equals(object obj)\r\n{");
                    sb.AppendLine("if (obj == null || !(obj is " + className + "))\r\nreturn false;");
                    sb.AppendLine(className + " b = (" + className + ")obj;");
                    sb.Append("return ");

                    int z = 0;
                    foreach (DbObject fieldObj in classObj.GetValue<DbObject>("fields"))
                    {
                        DbObject fieldMeta = fieldObj.GetValue<DbObject>("meta");
                        if (fieldMeta != null && fieldMeta.HasValue("hidden"))
                            continue;

                        string fieldName = fieldObj.GetValue<string>("name");
                        sb.AppendLine(((z++ != 0) ? "&& " : "") + fieldName + " == b." + fieldName);
                    }
                    sb.AppendLine(";\r\n}");

                    // GetHashCode override
                    sb.AppendLine("public override int GetHashCode()\r\n{\r\nunchecked {\r\nint hash = (int)2166136261;");
                    foreach (DbObject fieldObj in classObj.GetValue<DbObject>("fields"))
                    {
                        DbObject fieldMeta = fieldObj.GetValue<DbObject>("meta");
                        if (fieldMeta != null && fieldMeta.HasValue("hidden"))
                            continue;

                        string fieldName = fieldObj.GetValue<string>("name");
                        sb.AppendLine("hash = (hash * 16777619) ^ " + fieldName + ".GetHashCode();");
                    }
                    sb.AppendLine("return hash;\r\n}\r\n}");
                }

                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        private string WriteField(DbObject fieldObj)
        {
            StringBuilder sb = new StringBuilder();
            {
                string fieldName = fieldObj.GetValue<string>("name");
                EbxFieldType type = (EbxFieldType)fieldObj.GetValue<int>("type");
                string baseType = fieldObj.GetValue<string>("baseType", "");

                DbObject meta = fieldObj.GetValue<DbObject>("meta");
                DbObject typeObj = meta?.GetValue<DbObject>("type");

                if (meta != null && meta.HasValue("version"))
                {
                    bool bFound = false;
                    foreach (int ver in meta.GetValue<DbObject>("version"))
                    {
                        if (ver == (int)ProfilesLibrary.DataVersion)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                        return "";
                }

                if (typeObj != null)
                {
                    type = (EbxFieldType)typeObj.GetValue<int>("flags");
                    if (typeObj.HasValue("baseType"))
                        baseType = typeObj.GetValue<string>("baseType");
                }

                string fieldType = "";
                bool requiresDeclaration = false;

                sb.Append(WriteFieldAttributes(fieldObj));
                if (type == EbxFieldType.Array)
                {
                    EbxFieldType arrayType = (EbxFieldType)((fieldObj.GetValue<int>("arrayFlags") >> 4) & 0x1F);
                    if (typeObj != null)
                        arrayType = (EbxFieldType)typeObj.GetValue<int>("arrayType");

                    fieldType = "List<" + GetFieldType(arrayType, baseType) + ">";
                    requiresDeclaration = true;
                }
                else
                {
                    fieldType = GetFieldType(type, baseType);
                    requiresDeclaration = (type == EbxFieldType.ResourceRef || type == EbxFieldType.BoxedValueRef || type == EbxFieldType.CString || type == EbxFieldType.FileRef || type == EbxFieldType.TypeRef || type == EbxFieldType.Struct);
                }

                if (meta != null && meta.HasValue("accessor"))
                {
                    // custom getter/setter
                    sb.AppendLine("public " + fieldType + " " + fieldName + " { " + meta.GetValue<string>("accessor") + " }");
                }
                else
                {
                    // default getter/setter
                    sb.AppendLine("public " + fieldType + " " + fieldName + " { get { return _" + fieldName + "; } set { _" + fieldName + " = value; } }");
                }
                sb.AppendLine("protected " + fieldType + " _" + fieldName + ((requiresDeclaration) ? " = new " + fieldType + "()" : "") + ";");
            }
            return sb.ToString();
        }

        private string WriteClassAttributes(DbObject classObj)
        {
            StringBuilder sb = new StringBuilder();
            {
                DbObject meta = classObj.GetValue<DbObject>("meta");
                int alignment = classObj.GetValue<int>("alignment");

                if (meta != null && meta.HasValue("alignment"))
                    alignment = meta.GetValue<int>("alignment");

                // mandatory ebx meta
                sb.AppendLine("[" + typeof(EbxClassMetaAttribute).Name + "(" + classObj.GetValue<int>("flags") + ", " + alignment + ", " + classObj.GetValue<int>("size") + ", \"" + classObj.GetValue<string>("namespace") + "\")]");

                if (classObj.HasValue("guid"))
                    sb.AppendLine("[" + typeof(GuidAttribute).Name + "(\"" + classObj.GetValue<Guid>("guid") + "\")]");
                if (classObj.HasValue("typeInfoGuid"))
                {
                    foreach (Guid typeInfoGuid in classObj.GetValue<DbObject>("typeInfoGuid"))
                        sb.AppendLine("[" + typeof(TypeInfoGuidAttribute).Name + "(\"" + typeInfoGuid + "\")]");
                }
                if (classObj.HasValue("arrayNameHash"))
                    sb.AppendLine($"[{typeof(ArrayHashAttribute).Name}({classObj.GetValue<int>("arrayNameHash")})]");

                //if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
                //{
                //    sb.AppendLine("[" + typeof(RuntimeSizeAttribute).Name + "(" + classObj.GetValue<int>("runtimeSize") + ")]");
                //}

                if (classObj.HasValue("forceAlign"))
                {
                    sb.AppendLine("[" + typeof(ForceAlignAttribute).Name + "]");
                }

                if (classObj.HasValue("arrayGuid"))
                {
                    sb.AppendLine($"[{typeof(ArrayGuidAttribute).Name}(\"{classObj.GetValue<Guid>("arrayGuid")}\")]");
                }

                if (meta != null)
                {
                    // all other attributes
                    if (meta.HasValue("valueConverter")) sb.AppendLine("[" + typeof(ClassConverterAttribute).Name + "(\"" + meta.GetValue<string>("valueConverter") + "\")]");
                    if (meta.HasValue("description")) sb.AppendLine("[" + typeof(DescriptionAttribute).Name + "(\"" + meta.GetValue<string>("description") + "\")]");
                    if (meta.HasValue("inline")) sb.AppendLine("[" + typeof(IsInlineAttribute).Name + "]");
                    if (meta.HasValue("abstract")) sb.AppendLine("[" + typeof(IsAbstractAttribute).Name + "]");
                    if (meta.HasValue("icon")) sb.AppendLine("[" + typeof(IconAttribute).Name + "(\"" + meta.GetValue<string>("icon") + "\")]");
                    //if (meta.HasValue("realm")) sb.AppendLine("[DefaultRealm(\"" + meta.GetValue<string>("realm") + "\")]");
                    //if (meta.HasValue("attributes")) sb.Append(meta.GetValue<string>("attributes"));
                }
            }
            return sb.ToString();
        }

        private string WriteFieldAttributes(DbObject fieldObj)
        {
            StringBuilder sb = new StringBuilder();
            {
                DbObject meta = fieldObj.GetValue<DbObject>("meta");
                EbxFieldType type = (EbxFieldType)fieldObj.GetValue<int>("type");
                int arrayFlags = fieldObj.GetValue<int>("arrayFlags", 0);
                string baseType = (type == EbxFieldType.Pointer || type == EbxFieldType.Array) ? fieldObj.GetValue<string>("baseType", "null") : "null";
                int flags = fieldObj.GetValue<int>("flags");

                if (type == EbxFieldType.Array)
                {
                    if (fieldObj.HasValue("guid"))
                        sb.AppendLine("[" + typeof(GuidAttribute) + "(\"" + fieldObj.GetValue<Guid>("guid").ToString() + "\")]");
                }

                if (meta != null)
                {
                    DbObject typeObj = meta.GetValue<DbObject>("type");
                    if (typeObj != null)
                    {
                        flags = (typeObj.GetValue<int>("flags") << 4);
                        if (typeObj.HasValue("baseType"))
                            baseType = typeObj.GetValue<string>("baseType");
                        if (typeObj.HasValue("arrayType"))
                            arrayFlags = typeObj.GetValue<int>("arrayType") << 4;
                    }
                    else if (meta.HasValue("transient"))
                    {
                        // defaults to int
                        flags = (0x0F << 4);
                    }
                }

                if (baseType != "null")
                    baseType = "typeof(" + baseType + ")";

                // field index
                int index = fieldObj.GetValue<int>("index");

                if (fieldObj.HasValue("nameHash"))
                    sb.AppendLine("[" + typeof(HashAttribute).Name + "(" + fieldObj.GetValue<int>("nameHash") + ")]");

                // mandatory ebx meta
                sb.AppendLine("[" + typeof(EbxFieldMetaAttribute).Name + "(" + flags + ", " + fieldObj.GetValue<int>("offset") + ", " + baseType + ", " + (type == EbxFieldType.Array).ToString().ToLower() + ", " + arrayFlags + ")]");

                if (meta != null)
                {
                    // all other attributes
                    if (meta.HasValue("displayName")) sb.AppendLine("[" + typeof(DisplayNameAttribute) + "(\"" + meta.GetValue<string>("displayName") + "\")]");
                    if (meta.HasValue("description")) sb.AppendLine("[" + typeof(DescriptionAttribute) + "(\"" + meta.GetValue<string>("description") + "\")]");
                    if (meta.HasValue("editor")) sb.AppendLine("[" + typeof(EditorAttribute).Name + "(\"" + meta.GetValue<string>("editor") + "\")]");
                    if (meta.HasValue("readOnly")) sb.AppendLine("[" + typeof(IsReadOnlyAttribute).Name + "]");
                    if (meta.HasValue("hidden")) sb.AppendLine("[" + typeof(IsHiddenAttribute).Name + "]");
                    if (meta.HasValue("reference")) sb.AppendLine("[" + typeof(IsReferenceAttribute).Name + "]");
                    if (meta.HasValue("transient")) sb.AppendLine("[" + typeof(IsTransientAttribute).Name + "]");
                    if (meta.HasValue("category")) sb.AppendLine("[" + typeof(CategoryAttribute).Name + "(\"" + meta.GetValue<string>("category") + "\")]");
                    if (meta.HasValue("index")) index = meta.GetValue<int>("index");
                    if (meta.HasValue("hideChildren")) sb.AppendLine("[" + typeof(HideChildrentAttribute).Name + "]");
                    //if (meta.HasValue("property")) sb.AppendLine("[ExposedToSchematics(SchematicsInterfaceType.Property, (SchematicAccessType)" + meta.GetValue<int>("property") + ")]");
                    //if (meta.HasValue("event")) sb.AppendLine("[ExposedToSchematics(SchematicsInterfaceType.Event, (SchematicAccessType)" + meta.GetValue<int>("event") + ")]");
                    //if (meta.HasValue("link")) sb.AppendLine("[ExposedToSchematics(SchematicsInterfaceType.Link, (SchematicAccessType)" + meta.GetValue<int>("link") + ")]");
                }

                sb.AppendLine("[" + typeof(FieldIndexAttribute).Name + "(" + index + ")]");
            }
            return sb.ToString();
        }

        private string GetFieldType(EbxFieldType type, string baseType)
        {
            switch (type)
            {
                case EbxFieldType.Boolean: return "bool";
                case EbxFieldType.BoxedValueRef: return "BoxedValueRef";
                case EbxFieldType.CString: return "CString";
                case EbxFieldType.FileRef: return "FileRef";
                case EbxFieldType.Float32: return "float";
                case EbxFieldType.Float64: return "double";
                case EbxFieldType.Guid: return "Guid";
                case EbxFieldType.Int16: return "short";
                case EbxFieldType.Int32: return "int";
                case EbxFieldType.Int64: return "long";
                case EbxFieldType.Int8: return "sbyte";
                case EbxFieldType.ResourceRef: return "ResourceRef";
                case EbxFieldType.Sha1: return "Sha1";
                case EbxFieldType.String: return "string";
                case EbxFieldType.TypeRef: return "TypeRef";
                case EbxFieldType.UInt16: return "ushort";
                case EbxFieldType.UInt32: return "uint";
                case EbxFieldType.UInt64: return "ulong";
                case EbxFieldType.UInt8: return "byte";
                case EbxFieldType.Pointer: return "PointerRef";
                case EbxFieldType.Enum: return baseType;
                case EbxFieldType.Struct: return baseType;
            }

            return "";
        }

        public void Dispose()
        {
        }
    }

    public class ClassesSdkCreator
    {
        #region -- FB Types --
        public class TypeInfo
        {
            public int Type => ((flags >> 4) & 0x1F);

            public string name;
            public ushort flags;
            public uint size;
            public Guid guid;
            public ushort padding1;
            public string nameSpace;
            public ushort alignment;
            public uint fieldCount;
            public uint padding3;

            public long parentClass;
            public long arrayTypeOffset;
            public List<FieldInfo> fields = new List<FieldInfo>();

            public virtual void Read(MemoryReader reader)
            {
                bool byteAlignFieldCount = (ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals || ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare);

                name = reader.ReadNullTerminatedString();
                flags = reader.ReadUShort();
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    flags >>= 1;
                size = reader.ReadUInt();
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20)
                {
                    reader.Position -= 4;
                    size = reader.ReadUShort();

                    guid = reader.ReadGuid();
                    reader.ReadUShort();
                }
                padding1 = reader.ReadUShort();
                long nameSpaceOffset = reader.ReadLong();

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    arrayTypeOffset = reader.ReadLong();

                alignment = (byteAlignFieldCount) ? reader.ReadByte() : reader.ReadUShort();
                fieldCount = (byteAlignFieldCount) ? reader.ReadByte() : reader.ReadUShort();
                if (byteAlignFieldCount)
                    padding3 = reader.ReadUShort();
                padding3 = reader.ReadUInt();

                long[] offsets = new long[7];
                for (int i = 0; i < 7; i++)
                    offsets[i] = reader.ReadLong();

                reader.Position = nameSpaceOffset;
                nameSpace = reader.ReadNullTerminatedString();

                bool bReadFields = false;
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20)
                {
                    // FIFA19
                    parentClass = offsets[0];
                    if (Type == 2 /* Structure */) { reader.Position = offsets[6]; bReadFields = true; }
                    else if (Type == 3 /* Class */) { reader.Position = offsets[1]; bReadFields = true; }
                    else if (Type == 8 /* Enum */)
                    {
                        parentClass = 0;
                        reader.Position = offsets[0];
                        if (reader.Position == offsets[0])
                        {
                            reader.Position = offsets[1];
                            long newOffset = reader.ReadLong();
                            reader.Position = newOffset;

                            uint z = fieldCount;
                            while (z > 0)
                            {
                                newOffset = reader.ReadLong();
                                long oldPos = reader.Position;
                                reader.Position = newOffset;

                                while (true)
                                {
                                    if (z == 0)
                                        break;

                                    long value = reader.ReadLong(); // 0xFFFFFFFF
                                    long nameOffset = reader.ReadLong(); // 0x0
                                    if (value == 0 && nameOffset == 0)
                                        break;
                                    z--;

                                    FieldInfo f = new FieldInfo {typeOffset = value};

                                    reader.Position -= 8;
                                    f.name = reader.ReadNullTerminatedString();

                                    fields.Add(f);
                                }

                                reader.Position = oldPos;

                                if (z > 0)
                                {
                                    newOffset = reader.ReadLong();
                                    reader.Position = newOffset;
                                }
                            }
                            bReadFields = false;
                        }
                        else
                        {
                            bReadFields = true;
                        }
                    }
                }
                else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    // FIFA18
                    parentClass = offsets[0];
                    if (Type == 2 /* Structure */) { reader.Position = offsets[5]; bReadFields = true; }
                    else if (Type == 3 /* Class */) { reader.Position = offsets[1]; bReadFields = true; }
                    else if (Type == 8 /* Enum */) { reader.Position = offsets[0]; bReadFields = true; parentClass = 0; }
                }
                else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17)
                {
                    // MEA
                    parentClass = offsets[0];
                    if (Type == 2 /* Structure */) { reader.Position = offsets[3]; bReadFields = true; }
                    else if (Type == 3 /* Class */) { reader.Position = offsets[1]; bReadFields = true; }
                    else if (Type == 8 /* Enum */) { reader.Position = offsets[0]; bReadFields = true; parentClass = 0; }
                }
                else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals || ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4)
                {
                    // DAI
                    parentClass = offsets[0];
                    if (Type == 2 /* Structure */) { reader.Position = offsets[1]; bReadFields = true; }
                    else if (Type == 3 /* Class */) { reader.Position = offsets[2]; bReadFields = true; }
                    else if (Type == 8 /* Enum */) { reader.Position = offsets[0]; bReadFields = true; parentClass = 0; }
                }
                else
                {
                    // Everything else
                    if (Type == 2 /* Structure */) { reader.Position = offsets[1]; bReadFields = true; }
                    else if (Type == 3 /* Class */) { reader.Position = offsets[2]; bReadFields = true; }
                    else if (Type == 8 /* Enum */) { reader.Position = offsets[0]; bReadFields = true; parentClass = 0; }
                    else if (Type == 4 /* Array */) { parentClass = offsets[0]; }
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

            public virtual void Modify(DbObject classObj, Dictionary<long, ClassInfo> offsetClassInfoMapping)
            {
            }

            public T As<T>() where T : TypeInfo
            {
                return this as T;
            }
        }

        public class ClassInfo
        {
            public TypeInfo typeInfo;
            public ushort id;
            public ushort isDataContainer;
            public byte[] padding;
            public long parentClass;

            public virtual void Read(MemoryReader reader)
            {
                long thisOffset = reader.Position;

                long typeInfoOffset = reader.ReadLong();
                offset = reader.ReadLong();
                Guid guid = Guid.Empty;
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    guid = reader.ReadGuid();
                id = reader.ReadUShort();
                isDataContainer = reader.ReadUShort();
                padding = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
                parentClass = reader.ReadLong();

                reader.Position = typeInfoOffset;

                typeInfo = new TypeInfo();
                typeInfo.Read(reader);

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    typeInfo.guid = guid;

                if (typeInfo.parentClass != 0)
                    parentClass = typeInfo.parentClass;

                reader.Position = parentClass;
                if (reader.Position == thisOffset)
                    parentClass = 0;
            }
        }

        public class FieldInfo
        {
            public string name;
            public ushort flags;
            public uint offset;
            public ushort padding1;
            public long typeOffset;
            public int index;

            public virtual void Read(MemoryReader reader)
            {
                name = reader.ReadNullTerminatedString();
                flags = reader.ReadUShort();
                offset = reader.ReadUInt();
                padding1 = reader.ReadUShort();
                typeOffset = reader.ReadLong();
            }

            public virtual void Modify(DbObject fieldObj)
            {
            }
        }
        #endregion

        public static long offset;

        private List<ClassInfo> classInfos = new List<ClassInfo>();
        private List<string> alreadyProcessedClasses = new List<string>();
        private Dictionary<long, ClassInfo> offsetClassInfoMapping = new Dictionary<long, ClassInfo>();

        private List<EbxClass> processed = new List<EbxClass>();
        private Dictionary<string, List<EbxField>> fieldMapping;
        Dictionary<string, Tuple<EbxClass, DbObject>> mapping;
        private List<Tuple<EbxClass, DbObject>> values = null;
        private DbObject classList = null;
        private DbObject classMetaList = null;
        private SdkUpdateState state;

        public ClassesSdkCreator(SdkUpdateState inState)
        {
            state = inState;
        }

        public bool GatherTypeInfos(SdkUpdateTask task)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Frosty.Core.Sdk.Classes.txt"))
            {
                if (stream != null)
                    classMetaList = TypeLibrary.LoadClassesSDK(stream);
            }

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
            {
                // read in strings from the AnthemDemo (since Anthem has stripped all strings)
                using (NativeReader reader = new NativeReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Frosty.Core.Sdk.AnthemDemo-Strings.txt")))
                {
                    int count = int.Parse(reader.ReadLine());
                    for (int i = 0; i < count; i++)
                    {
                        string line = reader.ReadLine();
                        string[] arr = line.Split(',');

                        AnthemDemo.Strings.stringHash.Add(uint.Parse(arr[0]), arr[1]);
                    }

                    count = int.Parse(reader.ReadLine());
                    for (int i = 0; i < count; i++)
                    {
                        string line = reader.ReadLine();
                        string[] arr = line.Split(',');

                        AnthemDemo.Strings.classHash.Add(uint.Parse(arr[0]), arr[1]);
                    }

                    count = int.Parse(reader.ReadLine());
                    for (int i = 0; i < count; i++)
                    {
                        int hash = (int)uint.Parse(reader.ReadLine());
                        int fieldCount = int.Parse(reader.ReadLine());

                        AnthemDemo.Strings.fieldHash.Add((uint)hash, new Dictionary<uint, string>());
                        for (int j = 0; j < fieldCount; j++)
                        {
                            string line = reader.ReadLine();
                            string[] arr = line.Split(',');

                            AnthemDemo.Strings.fieldHash[(uint)hash].Add(uint.Parse(arr[0]), arr[1]);
                        }
                    }
                }
            }

            classList = DumpClasses(task);

            return classList != null && classList.Count > 0;
        }

        public bool CrossReferenceAssets(SdkUpdateTask task)
        {
            mapping = new Dictionary<string, Tuple<EbxClass, DbObject>>();
            fieldMapping = new Dictionary<string, List<EbxField>>();

            if (App.FileSystem.HasFileInMemoryFs("SharedTypeDescriptors.ebx"))
            {
                List<Guid> guids = new List<Guid>();
                LoadSharedTypeDescriptors("SharedTypeDescriptors.ebx", mapping, guids);
                LoadSharedTypeDescriptors("SharedTypeDescriptors_patch.ebx", mapping, guids);
            }
            else
            {
                uint count = App.AssetManager.GetEbxCount();
                uint index = 0;

                foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx())
                {
                    Stream ebxStream = App.AssetManager.GetEbxStream(entry);
                    if (ebxStream == null)
                        continue;

                    task.StatusMessage = string.Format("{0:0}%", (++index / (float)count) * 100);
                    using (EbxReader reader = EbxReader.CreateReader(ebxStream))
                    {
                        List<EbxClass> classes = reader.ClassTypes;
                        List<EbxField> fields = reader.FieldTypes;

                        foreach (EbxClass cl in classes)
                        {
                            if (cl.Name != "array")
                            {
                                if (!mapping.ContainsKey(cl.Name))
                                {
                                    DbObject foundObj = null;
                                    int idx = 0;
                                    foreach (DbObject classObj in classList)
                                    {
                                        if (classObj.GetValue<string>("name") == cl.Name)
                                        {
                                            foundObj = classObj;
                                            classList.RemoveAt(idx);
                                            break;
                                        }
                                        idx++;
                                    }

                                    mapping.Add(cl.Name, new Tuple<EbxClass, DbObject>(cl, foundObj));
                                    fieldMapping.Add(cl.Name, new List<EbxField>());

                                    for (int fieldId = 0; fieldId < cl.FieldCount; fieldId++)
                                    {
                                        EbxField field = fields[cl.FieldIndex + fieldId];
                                        fieldMapping[cl.Name].Add(field);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool CreateSDK()
        {
            DbObject finalList = new DbObject(false);

            values = mapping.Values.ToList();
            values.Sort((Tuple<EbxClass, DbObject> a, Tuple<EbxClass, DbObject> b) => { return a.Item1.Name.CompareTo(b.Item1.Name); });

            Console.WriteLine("Creating SDK");
            for (int z = 0; z < values.Count; z++)
            {
                Tuple<EbxClass, DbObject> ab = values[z];

                EbxClass cl = ab.Item1;
                DbObject obj = ab.Item2;

                if (obj == null)
                {
                    continue;
                }

                int offset = (cl.DebugType == EbxFieldType.Pointer) ? 8 : 0;
                int fieldIndex = 0;

                ProcessClass(cl, obj, fieldMapping[cl.Name], finalList, ref offset, ref fieldIndex);
            }

            List<DbObject> supportedClasses = new List<DbObject>();
            foreach (DbObject classObj in classList)
            {
                if (fieldMapping.ContainsKey(classObj.GetValue<string>("name")))
                    continue;

                EbxFieldType type = (EbxFieldType)((classObj.GetValue<int>("flags") >> 4) & 0x1F);
                if (type == EbxFieldType.Pointer)
                {
                    //if(classObj.GetValue<bool>("isRuntimeOnly"))
                    //{
                    //    Console.WriteLine("Class {0} is a runtime class", classObj.GetValue<string>("name"));
                    //    continue;
                    //}

                    if (classObj.GetValue<int>("alignment") == 0)
                    {
                        //Console.WriteLine("Class {0} has 0 byte alignment", classObj.GetValue<string>("name"));
                        classObj.SetValue("alignment", 4);
                    }
                }

                EbxClass tmpClass = new EbxClass()
                {
                    Name = classObj.GetValue<string>("name"),
                    Type = (ushort)classObj.GetValue<int>("flags"),
                    Alignment = (byte)classObj.GetValue<int>("alignment"),
                    FieldCount = (byte)classObj.GetValue<DbObject>("fields").Count
                };

                List<EbxField> tmpFields = new List<EbxField>();
                foreach (DbObject field in classObj.GetValue<DbObject>("fields"))
                {
                    EbxField tmpField = new EbxField()
                    {
                        Name = field.GetValue<string>("name"),
                        Type = (ushort)field.GetValue<int>("flags")
                    };
                    tmpFields.Add(tmpField);
                }

                values.Add(new Tuple<EbxClass, DbObject>(tmpClass, classObj));
                fieldMapping.Add(tmpClass.Name, tmpFields);
                supportedClasses.Add(classObj);
            }

            foreach (DbObject classObj in supportedClasses)
            {
                if (classObj.HasValue("basic"))
                {
                    finalList.Add(classObj);
                    continue;
                }

                Tuple<EbxClass, DbObject> p = values.Find((Tuple<EbxClass, DbObject> a) => { return a.Item2 == classObj; });

                int offset = 0;
                int fieldIndex = 0;

                ProcessClass(p.Item1, p.Item2, fieldMapping[p.Item1.Name], finalList, ref offset, ref fieldIndex);

                //if(p.Item1.DebugType == EbxFieldType.Pointer)
                //    Console.WriteLine(p.Item1.Name);
            }

            //TypeLibrary.BuildModule(ProfilesLibrary.SDKFilename, finalList);
            using (ModuleWriter writer = new ModuleWriter("EbxClasses.dll", finalList))
                writer.Write(App.FileSystem.Head);

            if (File.Exists("EbxClasses.dll"))
            {
                FileInfo fi = new FileInfo(".\\TmpProfiles\\" + ProfilesLibrary.SDKFilename + ".dll");
                if (!fi.Directory.Exists)
                    Directory.CreateDirectory(fi.Directory.FullName);
                if (fi.Exists)
                    File.Delete(fi.FullName);

                File.Move("EbxClasses.dll", fi.FullName);
            }
            else
            {
                Console.WriteLine("Failed to produce SDK");
                return false;
            }

            // delete type info cache if there is one
            if (File.Exists($"{App.FileSystem.CacheName}_typeinfo.cache"))
                File.Delete($"{App.FileSystem.CacheName}_typeinfo.cache");

            return true;
        }

        private void LoadSharedTypeDescriptors(string name, Dictionary<string, Tuple<EbxClass, DbObject>> mapping, List<Guid> existingClasses)
        {
            byte[] typeDescData = App.FileSystem.GetFileFromMemoryFs(name);
            if (typeDescData == null)
                return;

            Dictionary<uint, DbObject> classMapping = new Dictionary<uint, DbObject>();
            Dictionary<uint, string> hashToFieldMapping = new Dictionary<uint, string>();
            foreach (DbObject classObj in classList)
            {
                if (classObj.HasValue("basic"))
                    continue;

                classMapping.Add((uint)classObj.GetValue<int>("nameHash"), classObj);
                foreach (DbObject fieldObj in classObj.GetValue<DbObject>("fields"))
                {
                    if (!hashToFieldMapping.ContainsKey((uint)fieldObj.GetValue<int>("nameHash")))
                        hashToFieldMapping.Add((uint)fieldObj.GetValue<int>("nameHash"), fieldObj.GetValue("name", ""));
                }
            }

            using (NativeReader reader = new NativeReader(new MemoryStream(typeDescData)))
            {
                uint magic = reader.ReadUInt();
                ushort numClasses = reader.ReadUShort();
                ushort numFields = reader.ReadUShort();

                List<EbxField> fields = new List<EbxField>();
                for (int i = 0; i < numFields; i++)
                {
                    uint hash = reader.ReadUInt();

                    EbxField field = new EbxField
                    {
                        Name = (hashToFieldMapping.ContainsKey(hash)) ? hashToFieldMapping[hash] : "",
                        NameHash = hash,
                        Type = (ushort)(reader.ReadUShort() >> 1),
                        ClassRef = reader.ReadUShort(),
                        DataOffset = reader.ReadUInt(),
                        SecondOffset = reader.ReadUInt()
                    };
                    fields.Add(field);
                }

                int fieldIdx = 0;
                List<EbxClass?> classes = new List<EbxClass?>();
                List<Guid> guids = new List<Guid>();

                for (int i = 0; i < numClasses; i++)
                {
                    long classOffset = reader.Position;

                    Guid guid = reader.ReadGuid();
                    Guid guid2 = reader.ReadGuid();

                    
                    if (existingClasses.Contains(guid) && guid == guid2)
                    {
                        guids.Add(guid);
                        classes.Add(null);
                        continue;
                    }
                    existingClasses.Add(guid);

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

                    EbxClass theClass = new EbxClass
                    {
                        NameHash = hash,
                        FieldCount = (byte)fieldCount,
                        FieldIndex = (int)((classOffset - (fieldOffset - 0x08)) / 0x10),
                        Alignment = alignment,
                        Type = type,
                        Size = (ushort)size,
                        Index = i
                    };
                    classes.Add(theClass);
                    guids.Add(guid);
                }

                for (int i = 0; i < classes.Count; i++)
                {
                    if (!classes[i].HasValue)
                        continue;

                    EbxClass theClass = classes[i].Value;
                    Guid guid = guids[i];

                    if (classMapping.ContainsKey(theClass.NameHash))
                    {
                        DbObject classObj = classMapping[theClass.NameHash];

                        if (mapping.ContainsKey(classObj.GetValue("name", "")))
                        {
                            mapping.Remove(classObj.GetValue("name", ""));
                            fieldMapping.Remove(classObj.GetValue("name", ""));
                        }

                        if (!classObj.HasValue("typeInfoGuid"))
                            classObj.SetValue("typeInfoGuid", DbObject.CreateList());
                        if (classObj.GetValue<DbObject>("typeInfoGuid").FindIndex((object a) => (Guid)a == guid) == -1)
                            classObj.GetValue<DbObject>("typeInfoGuid").Add(guid);

                        EbxClass ebxClass = new EbxClass
                        {
                            Name = classObj.GetValue("name", ""),
                            FieldCount = (byte)theClass.FieldCount,
                            Alignment = theClass.Alignment,
                            Size = (ushort)(theClass.Size),
                            Type = (ushort)(theClass.Type >> 1),
                            SecondSize = (ushort)classObj.GetValue<int>("size")
                        };

                        //int idx = 0;
                        //foreach (DbObject clobj in classList)
                        //{
                        //    if (clobj.GetValue<int>("nameHash") == hash)
                        //    {
                        //        classList.RemoveAt(idx);
                        //        break;
                        //    }
                        //    idx++;
                        //}                     

                        mapping.Add(ebxClass.Name, new Tuple<EbxClass, DbObject>(ebxClass, classObj));
                        fieldMapping.Add(ebxClass.Name, new List<EbxField>());

                        DbObject fieldObjs = classObj.GetValue<DbObject>("fields");
                        DbObject newFieldObjs = DbObject.CreateList();
                        classObj.RemoveValue("fields");

                        for (int j = 0; j < theClass.FieldCount; j++)
                        {
                            EbxField field = fields[theClass.FieldIndex + j];
                            bool bFound = false;

                            foreach (DbObject fieldObj in fieldObjs)
                            {
                                uint nameHash = (uint)fieldObj.GetValue<int>("nameHash");
                                if (nameHash == field.NameHash)
                                {
                                    fieldObj.SetValue("type", field.Type);
                                    fieldObj.SetValue("offset", field.DataOffset);
                                    fieldObj.SetValue("value", (int)field.DataOffset);
                                    if (field.DebugType == EbxFieldType.Array)
                                    {
                                        Guid arrayGuid = guids[theClass.Index + (short)field.ClassRef];
                                        fieldObj.SetValue("guid", arrayGuid);
                                    }
                                    newFieldObjs.Add(fieldObj);
                                    bFound = true;
                                    break;
                                }
                            }

                            if (!bFound)
                            {
                                // not the inherited variable
                                uint inheritedHash = (ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville) ? 0xc4cfb854 : 0xb95a6ae7;
                                if (field.NameHash != inheritedHash)
                                {
                                    field.Name = (field.Name != "") ? field.Name : "Unknown_" + field.NameHash.ToString("x8");

                                    DbObject newField = DbObject.CreateObject();
                                    newField.SetValue("name", field.Name);
                                    newField.SetValue("nameHash", (int)field.NameHash);
                                    newField.SetValue("type", field.Type);
                                    newField.SetValue("flags", (ushort)0);
                                    newField.SetValue("offset", field.DataOffset);
                                    newField.SetValue("value", (int)field.DataOffset);
                                    newFieldObjs.Add(newField);
                                }
                            }

                            fieldMapping[ebxClass.Name].Add(field);
                            fieldIdx++;
                        }

                        classObj.SetValue("fields", newFieldObjs);
                    }
                    else
                    {
                        fieldIdx += theClass.FieldCount;
                    }
                }
            }
        }

        private int ProcessClass(EbxClass pclass, DbObject pobj, List<EbxField> fields, DbObject outList, ref int offset, ref int fieldIndex)
        {
            string parent = pobj.GetValue<string>("parent");
            if (parent != "")
            {
                Tuple<EbxClass, DbObject> p = values.Find((Tuple<EbxClass, DbObject> a) => { return a.Item1.Name == parent; });
                offset = ProcessClass(p.Item1, p.Item2, fieldMapping[p.Item1.Name], outList, ref offset, ref fieldIndex);

                if (p.Item1.Name == "DataContainer" && pclass.Name != "Asset")
                    pobj.SetValue("isData", true);
            }

            if (processed.Contains(pclass))
            {
                foreach (DbObject t in outList)
                {
                    if (t.GetValue<string>("name") == pclass.Name)
                    {
                        fieldIndex += t.GetValue<DbObject>("fields").Count;
                        return t.GetValue<int>("size");
                    }
                }
                return 0;
            }
            processed.Add(pclass);

            int index = classMetaList.FindIndex((object o) => { return ((DbObject)o).GetValue<string>("name") == pclass.Name; });
            DbObject classMeta = null;
            if (index != -1)
                classMeta = classMetaList[index] as DbObject;

            DbObject origFieldList = pobj.GetValue<DbObject>("fields");
            DbObject fieldList = new DbObject(false);

            if (pclass.DebugType == EbxFieldType.Enum)
            {
                foreach (DbObject field in origFieldList)
                {
                    DbObject fieldObj = new DbObject();
                    fieldObj.AddValue("name", field.GetValue<string>("name"));
                    fieldObj.AddValue("value", field.GetValue<int>("value"));
                    fieldList.Add(fieldObj);
                }
            }
            else
            {
                List<EbxField> newFields = new List<EbxField>();
                foreach (DbObject field in origFieldList)
                {
                    EbxField nf = new EbxField
                    {
                        Name = field.GetValue<string>("name"),
                        Type = (ushort)field.GetValue<int>("flags"),
                        DataOffset = (uint)field.GetValue<int>("offset"),
                        NameHash = (uint)field.GetValue<int>("nameHash")
                    };
                    newFields.Add(nf);
                }
                newFields.Sort((EbxField a, EbxField b) => a.DataOffset.CompareTo(b.DataOffset));

                foreach (EbxField field in newFields)
                {
                    if (field.DebugType == EbxFieldType.Inherited)
                        continue;

                    DbObject origField = null;
                    foreach (DbObject a in origFieldList)
                    {
                        if (a.GetValue<string>("name") == field.Name)
                        {
                            origField = a;
                            break;
                        }
                    }
                    if (origField == null)
                    {
                        Console.WriteLine(pclass.Name + "." + field.Name + " missing from executable definition");
                        continue;
                    }

                    DbObject fieldObj = new DbObject();
                    if (classMeta != null)
                    {
                        DbObject fieldMetaList = classMeta.GetValue<DbObject>("fields");
                        index = fieldMetaList.FindIndex((object o) => ((DbObject)o).GetValue<string>("name") == field.Name);
                        if (index != -1)
                        {
                            DbObject fieldMeta = fieldMetaList[index] as DbObject;
                            fieldObj.AddValue("meta", fieldMeta);
                        }
                    }

                    fieldObj.AddValue("name", field.Name);
                    fieldObj.AddValue("type", (int)field.DebugType);
                    fieldObj.AddValue("flags", (int)field.Type);
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20)
                    {
                        fieldObj.AddValue("offset", (int)field.DataOffset);
                        fieldObj.AddValue("nameHash", field.NameHash);
                    }

                    if (field.DebugType == EbxFieldType.Pointer || field.DebugType == EbxFieldType.Struct || field.DebugType == EbxFieldType.Enum)
                    {
                        string baseTypeName = origField.GetValue<string>("baseType");
                        int idx = values.FindIndex((Tuple<EbxClass, DbObject> a) => a.Item1.Name == baseTypeName && !a.Item2.HasValue("basic"));
                        if (idx != -1)
                            fieldObj.AddValue("baseType", values[idx].Item1.Name);
                        else if (field.DebugType == EbxFieldType.Enum)
                            throw new InvalidDataException();

                        if (field.DebugType == EbxFieldType.Struct)
                        {
                            foreach (EbxField ebxField in fields)
                            {
                                if (ebxField.DebugType == EbxFieldType.Inherited && ebxField.DataOffset == 16)
                                {
                                    pobj.SetValue("forceAlign", true);
                                    Console.WriteLine(pobj.GetValue<string>("name"));
                                }
                                if (ebxField.Name.Equals(field.Name))
                                {
                                    if (field.Type != ebxField.Type)
                                        fieldObj.SetValue("flags", (int)ebxField.Type);
                                    break;
                                }
                            }

                            while (offset % values[idx].Item1.Alignment != 0)
                                offset++;
                        }
                    }
                    else if (field.DebugType == EbxFieldType.Array)
                    {
                        string baseTypeName = origField.GetValue<string>("baseType");
                        int idx = values.FindIndex((Tuple<EbxClass, DbObject> a) => a.Item1.Name == baseTypeName && !a.Item2.HasValue("basic"));

                        if (idx != -1)
                        {
                            fieldObj.AddValue("baseType", values[idx].Item1.Name);
                            fieldObj.AddValue("arrayFlags", (int)values[idx].Item1.Type);
                        }
                        else
                        {
                            EbxFieldType arrayType = (EbxFieldType)((origField.GetValue<int>("arrayFlags") >> 4) & 0x1F);
                            if (arrayType == EbxFieldType.Pointer || arrayType == EbxFieldType.Struct || arrayType == EbxFieldType.Enum)
                                fieldObj.AddValue("baseType", baseTypeName);
                            fieldObj.AddValue("arrayFlags", origField.GetValue<int>("arrayFlags"));
                        }

                        if (origField.HasValue("guid"))
                            fieldObj.SetValue("guid", origField.GetValue<Guid>("guid"));
                    }

                    // Align refs to 8 byte boundaries
                    if (field.DebugType == EbxFieldType.ResourceRef
                        || field.DebugType == EbxFieldType.TypeRef
                        || field.DebugType == EbxFieldType.FileRef
                        || field.DebugType == EbxFieldType.BoxedValueRef)
                    {
                        while (offset % 8 != 0)
                            offset++;
                    }

                    // Align arrays and pointers to 4 byte boundaries
                    else if (field.DebugType == EbxFieldType.Array
                        || field.DebugType == EbxFieldType.Pointer)
                    {
                        while (offset % 4 != 0)
                            offset++;
                    }

                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem)
                        fieldObj.AddValue("offset", offset);

                    fieldObj.SetValue("index", origField.GetValue<int>("index") + fieldIndex);
                    fieldList.Add(fieldObj);

                    switch (field.DebugType)
                    {
                        case EbxFieldType.Struct:
                            {
                                Tuple<EbxClass, DbObject> s = values.Find((Tuple<EbxClass, DbObject> a) => a.Item1.Name == fieldObj.GetValue<string>("baseType"));

                                int structOffset = 0;
                                int structFieldIndex = 0;

                                offset += ProcessClass(s.Item1, s.Item2, fieldMapping[s.Item1.Name], outList, ref structOffset, ref structFieldIndex);
                            }
                            break;
                        case EbxFieldType.Pointer: offset += 4; break;
                        case EbxFieldType.Array: offset += 4; break;
                        case EbxFieldType.String: offset += 32; break;
                        case EbxFieldType.CString: offset += 4; break;
                        case EbxFieldType.Enum: offset += 4; break;
                        case EbxFieldType.FileRef: offset += 8; break;
                        case EbxFieldType.Boolean: offset += 1; break;
                        case EbxFieldType.Int8: offset += 1; break;
                        case EbxFieldType.UInt8: offset += 1; break;
                        case EbxFieldType.Int16: offset += 2; break;
                        case EbxFieldType.UInt16: offset += 2; break;
                        case EbxFieldType.Int32: offset += 4; break;
                        case EbxFieldType.UInt32: offset += 4; break;
                        case EbxFieldType.Int64: offset += 8; break;
                        case EbxFieldType.UInt64: offset += 8; break;
                        case EbxFieldType.Float32: offset += 4; break;
                        case EbxFieldType.Float64: offset += 8; break;
                        case EbxFieldType.Guid: offset += 16; break;
                        case EbxFieldType.Sha1: offset += 20; break;
                        case EbxFieldType.ResourceRef: offset += 8; break;
                        case EbxFieldType.TypeRef: offset += 8; break;
                        case EbxFieldType.BoxedValueRef: offset += 16; break;
                    }
                }
            }

            while (offset % pclass.Alignment != 0)
                offset++;

            pobj.SetValue("flags", (int)pclass.Type);

            pobj.SetValue("size", offset);
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
                pobj.SetValue("size", pclass.Size);

            if (pclass.DebugType == EbxFieldType.Enum)
                pobj.SetValue("size", 4);
            pobj.SetValue("alignment", (int)pclass.Alignment);
            pobj.SetValue("fields", fieldList);
            fieldIndex += fieldList.Count;

            if (classMeta != null)
            {
                pobj.AddValue("meta", classMeta);
                foreach (DbObject fieldMeta in classMeta.GetValue<DbObject>("fields"))
                {
                    if (fieldMeta.GetValue<bool>("added", false))
                    {
                        DbObject fieldObj = new DbObject();
                        fieldObj.AddValue("name", fieldMeta.GetValue<string>("name"));
                        fieldObj.AddValue("type", (int)EbxFieldType.Int32);
                        fieldObj.AddValue("meta", fieldMeta);

                        pobj.GetValue<DbObject>("fields").Add(fieldObj);
                    }
                }
            }

            outList.Add(pobj);
            return offset;
        }

        private DbObject DumpClasses(SdkUpdateTask task)
        {
            MemoryReader reader = null;
            string Namespace = "Frosty.Core.Sdk.ClassesSdkCreator+";

            // Anthem
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem) { Namespace = "Frosty.Core.Sdk.Anthem."; }

            // All others
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20) { Namespace = "Frosty.Core.Sdk.Madden20."; }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville) { Namespace = "Frosty.Core.Sdk.Madden20."; }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20) { Namespace = "Frosty.Core.Sdk.Madden20."; }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat) { Namespace = "Frosty.Core.Sdk.Madden20."; }

            long origOffset = state.TypeInfoOffset;
            reader = new MemoryReader(state.Process, origOffset);

            // cleanup
            offsetClassInfoMapping.Clear();
            classInfos.Clear();
            alreadyProcessedClasses.Clear();
            processed.Clear();
            fieldMapping?.Clear();

            offset = origOffset;
            int count = 0;

            while (offset != 0)
            {
                task.StatusMessage = $"Found {++count} type(s)";
                reader.Position = offset;

                ClassInfo info = (ClassInfo)Activator.CreateInstance(Type.GetType(Namespace + "ClassInfo"));
                info.Read(reader);

                classInfos.Add(info);
                offsetClassInfoMapping.Add(origOffset, info);

                if (offset != 0)
                {
                    origOffset = offset;
                    //offset = AdjustOffset(offset);
                }
            }

            reader.Dispose();

            DbObject classList = new DbObject(false);
            classInfos.Sort((ClassInfo a, ClassInfo b) => a.typeInfo.name.CompareTo(b.typeInfo.name));

            foreach (ClassInfo classInfo in classInfos)
            {
                if (classInfo.typeInfo.Type == 2 || classInfo.typeInfo.Type == 3 || classInfo.typeInfo.Type == 8 || classInfo.typeInfo.Type == 0x1B)
                {
                    if (classInfo.typeInfo.Type == 0x1B)
                    {
                        //Console.WriteLine(classInfo.typeInfo.name);
                        classInfo.typeInfo.flags = (3 << 4);
                    }
                    CreateClassObject(classInfo, ref classList);
                }
                else if (classInfo.typeInfo.Type != 4)
                {
                    CreateBasicClassObject(classInfo, ref classList);
                }
            }

            return classList;
        }

        private void CreateBasicClassObject(ClassInfo classInfo, ref DbObject classList)
        {
            int alignment = classInfo.typeInfo.alignment;
            int size = (int)classInfo.typeInfo.size;

            ClassInfo arrayType = (offsetClassInfoMapping.ContainsKey(classInfo.typeInfo.arrayTypeOffset)) ? offsetClassInfoMapping[classInfo.typeInfo.arrayTypeOffset] : null;

            DbObject classObj = DbObject.CreateObject();
            classObj.SetValue("name", classInfo.typeInfo.name);
            classObj.SetValue("type", classInfo.typeInfo.Type);
            classObj.SetValue("flags", (int)classInfo.typeInfo.flags);
            classObj.SetValue("alignment", alignment);
            classObj.SetValue("size", size);
            classObj.SetValue("runtimeSize", size);
            if (classInfo.typeInfo.guid != Guid.Empty)
                classObj.SetValue("guid", classInfo.typeInfo.guid);
            if (arrayType != null && arrayType.typeInfo.guid != Guid.Empty)
            {
                classObj.AddValue("arrayGuid", arrayType.typeInfo.guid);
            }
            classObj.SetValue("namespace", classInfo.typeInfo.nameSpace);
            classObj.SetValue("fields", DbObject.CreateList());
            classObj.SetValue("parent", "");
            classObj.SetValue("basic", true);

            classInfo.typeInfo.Modify(classObj, offsetClassInfoMapping);
            classList.Add(classObj);
        }

        private void CreateClassObject(ClassInfo classInfo, ref DbObject classList)
        {
            if (alreadyProcessedClasses.Contains(classInfo.typeInfo.name))
                return;

            ClassInfo parent = (offsetClassInfoMapping.ContainsKey(classInfo.parentClass)) ? offsetClassInfoMapping[classInfo.parentClass] : null;
            ClassInfo arrayType = (offsetClassInfoMapping.ContainsKey(classInfo.typeInfo.arrayTypeOffset)) ? offsetClassInfoMapping[classInfo.typeInfo.arrayTypeOffset] : null;
            if (parent != null)
                CreateClassObject(parent, ref classList);

            int alignment = classInfo.typeInfo.alignment;
            int size = (int)classInfo.typeInfo.size;

            DbObject classObj = new DbObject();
            classObj.AddValue("name", classInfo.typeInfo.name);
            classObj.AddValue("parent", (parent != null) ? parent.typeInfo.name : "");
            classObj.AddValue("type", classInfo.typeInfo.Type);
            classObj.AddValue("flags", (int)classInfo.typeInfo.flags);
            classObj.AddValue("alignment", alignment);
            classObj.AddValue("size", size);
            classObj.AddValue("runtimeSize", size);
            classObj.AddValue("additional", (int)classInfo.isDataContainer);
            classObj.AddValue("namespace", classInfo.typeInfo.nameSpace);
            if (classInfo.typeInfo.guid != Guid.Empty)
                classObj.AddValue("guid", classInfo.typeInfo.guid);
            if (arrayType != null && arrayType.typeInfo.guid != Guid.Empty)
                classObj.AddValue("arrayGuid", arrayType.typeInfo.guid);

            classInfo.typeInfo.Modify(classObj, offsetClassInfoMapping);

            DbObject fieldList = new DbObject(false);
            foreach (FieldInfo field in classInfo.typeInfo.fields)
            {
                DbObject fieldObj = new DbObject();
                if (classInfo.typeInfo.Type == 8)
                {
                    fieldObj.AddValue("name", field.name);
                    fieldObj.AddValue("value", (int)field.typeOffset);
                }
                else
                {
                    ClassInfo fieldType = offsetClassInfoMapping[field.typeOffset];
                    fieldObj.AddValue("name", field.name);
                    fieldObj.AddValue("type", fieldType.typeInfo.Type);
                    fieldObj.AddValue("flags", (int)fieldType.typeInfo.flags);
                    fieldObj.AddValue("offset", (int)field.offset);
                    fieldObj.AddValue("index", (int)field.index);
                    if (fieldType.typeInfo.Type == 3 || fieldType.typeInfo.Type == 2 || fieldType.typeInfo.Type == 8)
                    {
                        fieldObj.AddValue("baseType", fieldType.typeInfo.name);
                    }
                    else if (fieldType.typeInfo.Type == 4)
                    {
                        fieldType = offsetClassInfoMapping[fieldType.parentClass];
                        fieldObj.AddValue("baseType", fieldType.typeInfo.name);
                        fieldObj.AddValue("arrayFlags", (int)fieldType.typeInfo.flags);
                    }
                }
                field.Modify(fieldObj);
                fieldList.Add(fieldObj);
            }

            classObj.AddValue("fields", fieldList);
            classList.Add(classObj);

            alreadyProcessedClasses.Add(classInfo.typeInfo.name);
        }

        //public static long AdjustOffset(long inOffset)
        //{
        //    for (int i = 0; i < sections.Length; i++)
        //    {
        //        if ((ulong)inOffset >= (optHeader.ImageBase + sections[i].VirtualAddress) && (ulong)inOffset < (optHeader.ImageBase + sections[i].VirtualAddress + sections[i].VirtualSize))
        //        {
        //            inOffset -= (sections[i].VirtualAddress - sections[i].PointerToRawData);
        //            inOffset -= (long)optHeader.ImageBase;
        //            break;
        //        }
        //    }

        //    return inOffset;
        //}
    }
}
