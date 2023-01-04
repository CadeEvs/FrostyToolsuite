using Frosty.Hash;
using FrostySdk.Attributes;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using FrostySdk.Ebx;
using FrostySdk.Managers.Entries;

namespace FrostySdk.Attributes
{
    #region -- Attributes --
    public static class GlobalAttributes
    {
        /// <summary>
        /// Wether or not to display the module of the class prepended to the id
        /// </summary>
        public static bool DisplayModuleInClassId;
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ResCustomHandlerAttribute : Attribute
    {
        public ResourceType ResType { get; set; }
        public Type CustomHandler { get; set; }
        public ResCustomHandlerAttribute(ResourceType resType, Type customHandler)
        {
            ResType = resType;
            CustomHandler = customHandler;
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class SdkVersionAttribute : Attribute
    {
        public int Version { get; set; }
        public SdkVersionAttribute(int version)
        {
            Version = version;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class HashAttribute : Attribute
    {
        public int Hash { get; set; }
        public HashAttribute(int inHash) { Hash = inHash; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public class ArrayHashAttribute : Attribute
    {
        public int Hash { get; set; }
        public ArrayHashAttribute(int inHash) { Hash = inHash; }
    }

    /// <summary>
    /// Specifies the guid for the class, used when looking up type refs
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public class GuidAttribute : Attribute
    {
        public Guid Guid { get; set; }
        public GuidAttribute(string inGuid) { Guid = Guid.Parse(inGuid); }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public class ArrayGuidAttribute : Attribute
    {
        public Guid Guid { get; set; }
        public ArrayGuidAttribute(string inGuid) { Guid = Guid.Parse(inGuid); }
    }

    /// <summary>
    /// Used by Anthem to obtain the correct class during ebx reading
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    public class TypeInfoGuidAttribute : Attribute
    {
        public Guid Guid { get; set; }
        public TypeInfoGuidAttribute(string inGuid) { Guid = Guid.Parse(inGuid); }
    }

    /// <summary>
    /// Specifies the signature of the type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    public class TypeInfoSignatureAttribute : Attribute
    {
        public uint Signature { get; set; }
        public TypeInfoSignatureAttribute(int inSignature) { Signature = (uint)inSignature; }
    }

    /// <summary>
    /// Specifies that the class requires a converter to convert to/from some custom type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassConverterAttribute : Attribute
    {
        public Type Type { get; set; }
        public ClassConverterAttribute(string inType) { Type = null; }
        public ClassConverterAttribute(Type inType) { Type = inType; }
    }

    /// <summary>
    /// Specifies that the class can have instances created even if it subclasses from a class that cannot
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsInlineAttribute : Attribute
    {
        public IsInlineAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies that the class can not be created directly
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsAbstractAttribute : Attribute
    {
        public IsAbstractAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies the icon this class should use
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class IconAttribute : Attribute
    {
        public string Icon { get; set; }
        public IconAttribute(string inIcon)
        {
            Icon = inIcon;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Enum)]
    public class RuntimeSizeAttribute : Attribute
    {
        public int Size { get; set; }
        public RuntimeSizeAttribute(int inSize)
        {
            Size = inSize;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ForceAlignAttribute : Attribute
    {
        public ForceAlignAttribute()
        {
        }
    }

    /// <summary>
    /// Sepcifies that this property is dependent on the specified property. The specified property must be a bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        public string Name { get; set; }
        public DependsOnAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Specifies that this property should not show its array items
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HideChildrentAttribute : Attribute
    {
        public HideChildrentAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies the category this property will be displayed under if the class is a top level class of the property grid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CategoryAttribute : Attribute
    {
        public string Name { get; set; }
        public CategoryAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Overrides the display name of the property/class in the Property Grid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DisplayNameAttribute : Attribute
    {
        public string Name { get; set; }
        public DisplayNameAttribute(string name) { Name = name; }
    }

    /// <summary>
    /// Sets the description to display for the property/class in the Property Grid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public DescriptionAttribute(string desc) { Description = desc; }
    }

    /// <summary>
    /// Sets the type of property grid editor to use for the property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EditorAttribute : Attribute
    {
        public Type EditorType { get; set; }
        public EditorAttribute(Type type)
        {
            EditorType = type;
        }
    }

    /// <summary>
    /// Specifies that this property is read only
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsReadOnlyAttribute : Attribute
    {
        public IsReadOnlyAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies that this property is only a reference (does not increment ref-count)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsReferenceAttribute : Attribute
    {
        public IsReferenceAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies that this property (array/struct) is expanded when first loaded
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsExpandedByDefaultAttribute : Attribute
    {
        public IsExpandedByDefaultAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies that the property is a fixed size array
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FixedSizeArrayAttribute : Attribute
    {
    }

    /// <summary>
    /// Sets the default value for new instances of the property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; set; }
        public DefaultValueAttribute(object value)
        {
            DefaultValue = value;
        }
    }

    /// <summary>
    /// Specifies that this property can be used as a property connection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsPropertyAttribute : Attribute
    {
        public IsPropertyAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies that this property should not be saved
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsTransientAttribute : Attribute
    {
        public IsTransientAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies that this property is hidden from the property grid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsHiddenAttribute : Attribute
    {
        public IsHiddenAttribute()
        {
        }
    }

    /// <summary>
    /// Specifies optional meta data specific to an editor
    /// </summary>
    public abstract class EditorMetaDataAttribute : Attribute
    {
        public EditorMetaDataAttribute()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SliderMinMaxAttribute : EditorMetaDataAttribute
    {
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float SmallChange { get; set; }
        public float LargeChange { get; set; }
        public bool  IsSnapToTickEnabled { get; set; }

        public SliderMinMaxAttribute(float min, float max, float small, float large, bool snap)
        {
            MinValue = min;
            MaxValue = max;
            SmallChange = small;
            LargeChange = large;
            IsSnapToTickEnabled = snap;
        }
    }

    /// <summary>
    /// Specifies the fields index, which may differ from its offset
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIndexAttribute : Attribute
    {
        public int Index { get; set; }
        public FieldIndexAttribute(int inIndex)
        {
            Index = inIndex;
        }
    }

    /// <summary>
    /// Mandatory attribute for all Ebx based classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, AllowMultiple = false)]
    public class EbxClassMetaAttribute : Attribute
    {
        public EbxFieldType Type => (EbxFieldType)((Flags >> 4) & 0x1F);

        public ushort Flags { get; set; }
        public byte Alignment { get; set; }
        public ushort Size { get; set; }
        public string Namespace { get; set; }

        public EbxClassMetaAttribute(ushort flags, byte alignment, ushort size, string nameSpace)
        {
            Flags = flags;
            Alignment = alignment;
            Size = size;
            Namespace = nameSpace;
        }

        public EbxClassMetaAttribute(EbxFieldType type)
        {
            Flags = (ushort)((int)type << 4);
        }
    }

    /// <summary>
    /// Mandatory attribute for all Ebx based fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EbxFieldMetaAttribute : Attribute
    {
        public EbxFieldType Type => (EbxFieldType)((Flags >> 4) & 0x1F);
        public EbxFieldType ArrayType => (EbxFieldType)((ArrayFlags >> 4) & 0x1F);

        public ushort Flags { get; set; }
        public uint Offset { get; set; }
        public Type BaseType { get; set; }
        public ushort ArrayFlags { get; set; }
        public bool IsArray { get; set; }

        public EbxFieldMetaAttribute(ushort flags, uint offset, Type baseType, bool isArray, ushort arrayFlags)
        {
            Flags = flags;
            Offset = offset;
            BaseType = baseType;
            IsArray = isArray;
            ArrayFlags = arrayFlags;
        }

        public EbxFieldMetaAttribute(EbxFieldType type, string baseType = "", EbxFieldType arrayType = EbxFieldType.Inherited)
        {
            BaseType = typeof(object);
            if (baseType != "")
                BaseType = TypeLibrary.GetType(baseType);
            Flags = (ushort)((int)type << 4);
            if (arrayType != EbxFieldType.Inherited)
            {
                IsArray = true;
                ArrayFlags = (ushort)((int)arrayType << 4);
            }
        }
    }
    #endregion
}

namespace FrostySdk
{
    public class BaseTypeOverride
    {
        /// <summary>
        /// Used to load the data from Original into custom fields
        /// </summary>
        public virtual void Load()
        {
        }

        /// <summary>
        /// Used to save custom field values back into Original
        /// </summary>
        public virtual void Save(object e)
        {
        }

        /// <summary>
        /// The original object that this type is used to override and add to
        /// </summary>
        public object Original;
    }
    
    public abstract class BaseFieldOverride
    {
    }

    internal struct MetaDataType
    {
        public string DisplayName => meta.GetValue<string>("displayName", "");
        public string Description => meta.GetValue<string>("description", "");
        public string Category => meta.GetValue<string>("category", "");
        public string Editor => meta.GetValue<string>("editor", "");
        public string ValueConverter => meta.GetValue<string>("valueConverter", "");
        
        public bool IsAbstract => meta.GetValue<bool>("abstract", false);
        public bool IsTransient => meta.GetValue<bool>("transient", false);
        public bool IsReadOnly => meta.GetValue<bool>("readOnly", false);
        public bool IsReference => meta.GetValue<bool>("reference", false);
        public bool IsInline => meta.GetValue<bool>("inline", false);
        public bool IsProperty => false;

        private DbObject meta;

        public MetaDataType(DbObject inMeta)
        {
            meta = inMeta;
        }
    }
    internal struct FieldType
    {
        public string Name => name;
        public Type Type => type;
        public Type BaseType => baseType;
        public EbxField? FieldInfo => fieldType;
        public EbxField? ArrayInfo => arrayType;
        public MetaDataType? MetaData => metaData;

        private string name;
        private Type type;
        private Type baseType;
        private EbxField? fieldType;
        private EbxField? arrayType;
        private MetaDataType? metaData;

        internal FieldType(string inName, Type inType, Type inBaseType, EbxField? inFieldType, EbxField? inArrayType = null, MetaDataType? inMetaData = null)
        {
            name = inName;
            type = inType;
            baseType = inBaseType;
            fieldType = inFieldType;
            arrayType = inArrayType;
            metaData = inMetaData;
        }
    }

    public static class TypeLibrary
    {
        public static class Reflection
        {
            private static Dictionary<Guid, string> typeInfos = new Dictionary<Guid, string>();
            private static Dictionary<uint, Type> typeInfosByHash = new Dictionary<uint, Type>();

            private static int cacheVersion = 1;

            private static bool bInitialized = false;

            public static bool ReadCache()
            {
                if (!File.Exists($"Caches/{ProfilesLibrary.CacheName}_typeinfo.cache"))
                    return false;

                using (NativeReader reader = new NativeReader(new FileStream($"Caches/{ProfilesLibrary.CacheName}_typeinfo.cache", FileMode.Open, FileAccess.Read)))
                {
                    uint version = reader.ReadUInt();
                    if (version != cacheVersion)
                        return false;

                    int profileHash = reader.ReadInt();
                    if (profileHash != Fnv1.HashString(ProfilesLibrary.ProfileName))
                        return false;

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Guid guid = reader.ReadGuid();
                        string s = reader.ReadNullTerminatedString();

                        typeInfos.Add(guid, s);
                    }
                }

                return true;
            }

            public static void WriteToCache()
            {
                FileInfo fi = new FileInfo($"Caches/{ProfilesLibrary.CacheName}_typeinfo.cache");
                if (!Directory.Exists(fi.DirectoryName))
                    Directory.CreateDirectory(fi.DirectoryName);

                using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
                {
                    writer.Write(cacheVersion);
                    writer.Write(Fnv1.HashString(ProfilesLibrary.ProfileName));

                    writer.Write(typeInfos.Count);
                    foreach (KeyValuePair<Guid, string> kv in typeInfos)
                    {
                        writer.Write(kv.Key); // Guid
                        writer.WriteNullTerminatedString(kv.Value); // 
                    }
                }
            }

            public static void LoadClassInfoAssets(AssetManager am)
            {
                if (!ReadCache())
                {
                    foreach (EbxAssetEntry entry in am.EnumerateEbx(type: "TypeInfoAsset"))
                    {
                        EbxAsset asset = am.GetEbx(entry);
                        if (!typeInfos.ContainsKey(asset.RootInstanceGuid))
                        {
                            string typeName = ((dynamic)asset.RootObject).TypeName;
                            typeInfos.Add(asset.RootInstanceGuid, typeName);
                        }
                    }
                    foreach (Type type in GetConcreteTypes())
                    {
                        GuidAttribute attr = type.GetCustomAttribute<GuidAttribute>();
                        if (attr != null)
                        {
                            string name = type.Name;
                            if (type.GetCustomAttribute<DisplayNameAttribute>() != null)
                            {
                                name = type.GetCustomAttribute<DisplayNameAttribute>().Name;
                            }
                            typeInfos.Add(attr.Guid, name);
                        }
                        ArrayGuidAttribute arrayAttr = type.GetCustomAttribute<ArrayGuidAttribute>();
                        if (arrayAttr != null)
                        {
                            string name = type.Name;
                            if (type.GetCustomAttribute<DisplayNameAttribute>() != null)
                            {
                                name = type.GetCustomAttribute<DisplayNameAttribute>().Name;
                            }
                            typeInfos.Add(arrayAttr.Guid, $"List<{name}>");
                        }
                    }
                    WriteToCache();
                }

                bInitialized = true;
            }

            public static Guid LookupGuid(string name)
            {
                if (!bInitialized)
                    return Guid.Empty;

                foreach (var guid in typeInfos.Keys)
                {
                    if (name.Equals(typeInfos[guid]))
                        return guid;
                }

                return Guid.Empty;
            }

            public static string LookupType(Guid guid)
            {
                if (!bInitialized)
                    return "";

                if (!typeInfos.ContainsKey(guid))
                    return guid.ToString();

                return typeInfos[guid];
            }

            public static Type LookupType(uint hash)
            {
                if (typeInfosByHash.Count == 0)
                {
                    foreach (Type t in TypeLibrary.GetConcreteTypes())
                    {
                        string name = t.Name;
                        DisplayNameAttribute dispNameAttr = t.GetCustomAttribute<DisplayNameAttribute>();
                        if (dispNameAttr != null)
                        {
                            name = t.GetCustomAttribute<DisplayNameAttribute>().Name;
                        }

                        ArrayGuidAttribute arrayAttr = t.GetCustomAttribute<ArrayGuidAttribute>();
                        if (arrayAttr != null)
                        {
                            name = $"List<{name}>";
                        }

                        typeInfosByHash.Add((uint)Fnv1.HashString(name), t);
                    }
                }

                if (!typeInfosByHash.ContainsKey(hash))
                    return null;
                return typeInfosByHash[hash];
            }
        }

        private const string ModuleName = "EbxClasses";
        private const string Namespace = "FrostySdk.Ebx.";

        private static AssemblyBuilder m_assemblyBuilder;
        private static ModuleBuilder m_moduleBuilder;
        private static Assembly m_existingAssembly;

        private static readonly List<TypeBuilder> m_constructingTypes = new List<TypeBuilder>();
        private static readonly List<ConstructorBuilder> m_constructors = new List<ConstructorBuilder>();
        private static readonly List<Guid> m_constructingGuids = new List<Guid>();

        private static readonly Dictionary<Guid, Type> m_guidTypeMapping = new Dictionary<Guid, Type>();
        
        public static void Initialize(bool loadSdk = true)
        {
            if (loadSdk)
            {
                // move across any newly created SDKs
                if (File.Exists("TmpProfiles/" + ProfilesLibrary.SDKFilename + ".dll"))
                {
                    FileInfo srcFi = new FileInfo("TmpProfiles/" + ProfilesLibrary.SDKFilename + ".dll");
                    FileInfo dstFi = new FileInfo("Profiles/" + ProfilesLibrary.SDKFilename + ".dll");

                    File.Delete(dstFi.FullName);
                    File.Move(srcFi.FullName, dstFi.FullName);

                    File.Delete(srcFi.FullName);
                }

                // now try to load SDK
                if (File.Exists("Profiles/" + ProfilesLibrary.SDKFilename + ".dll"))
                {
                    m_existingAssembly = Assembly.Load(new AssemblyName(ModuleName));
                }
            }

            AssemblyName name = new AssemblyName(ModuleName);
            m_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(ModuleName);
        }

        public static uint GetSdkVersion()
        {
            if (m_existingAssembly == null)
            {
                return 0;
            }
            
            SdkVersionAttribute attr = m_existingAssembly.GetCustomAttribute<SdkVersionAttribute>();
            if (attr == null)
            {
                return 0;
            }
                
            return (uint)attr.Version;
        }

        public static DbObject LoadClassesSdk(Stream sdkStream)
        {
            DbObject classList = null;
            using (NativeReader reader = new NativeReader(sdkStream))
            {
                classList = new DbObject(false);
                while (reader.Position < reader.Length)
                {
                    string line = reader.ReadLine().Trim('\t', ' ');
                    if (!line.StartsWith("BeginClass"))
                    {
                        continue;
                    }
                    
                    string className = line.Split(' ')[1];

                    DbObject classObj = new DbObject();
                    classObj.SetValue("name", className);

                    DbObject fieldList = new DbObject(false);
                    classObj.SetValue("fields", fieldList);

                    while (!line.StartsWith("EndClass"))
                    {
                        line = reader.ReadLine().Trim('\t', ' ');
                        if (line.StartsWith("//"))
                        {
                            continue;
                        }

                        if (line.StartsWith("BeginFields"))
                        {
                            string fieldCount = line.Split(' ')[1];
                            while (!line.StartsWith("EndFields"))
                            {
                                line = reader.ReadLine().Trim('\t', ' ');
                                if (!line.StartsWith("BeginField "))
                                {
                                    continue;
                                }
                                
                                string fieldName = line.Split(' ')[1];

                                DbObject fieldObj = new DbObject();
                                fieldObj.SetValue("name", fieldName);

                                while (!line.EndsWith("EndField"))
                                {
                                    line = reader.ReadLine().Trim('\t', ' ');
                                    string[] arr = SplitOnce(line);

                                    if (arr[0].StartsWith("DisplayName")) fieldObj.SetValue("displayName", arr[1]);
                                    else if (arr[0].StartsWith("Description")) fieldObj.SetValue("description", arr[1]);
                                    else if (arr[0].StartsWith("Category")) fieldObj.SetValue("category", arr[1]);
                                    else if (arr[0].StartsWith("Editor")) fieldObj.SetValue("editor", arr[1]);
                                    else if (arr[0].StartsWith("Transient")) fieldObj.SetValue("transient", bool.Parse(arr[1]));
                                    else if (arr[0].StartsWith("ReadOnly")) fieldObj.SetValue("readOnly", bool.Parse(arr[1]));
                                    else if (arr[0].StartsWith("Reference")) fieldObj.SetValue("reference", bool.Parse(arr[1]));
                                    else if (arr[0].StartsWith("Added")) fieldObj.SetValue("added", bool.Parse(arr[1]));
                                    else if (arr[0].StartsWith("Hidden")) fieldObj.SetValue("hidden", bool.Parse(arr[1]));
                                    else if (arr[0].StartsWith("Index")) fieldObj.SetValue("index", int.Parse(arr[1]));
                                    else if (arr[0].StartsWith("HideChildren")) fieldObj.SetValue("hideChildren", bool.Parse(arr[1]));
                                    else if (arr[0].StartsWith("InterfaceType"))
                                    {
                                        int it = int.Parse(arr[1]);
                                        switch (it)
                                        {
                                            case 0: fieldObj.SetValue("property", 0); break;
                                            case 1: fieldObj.SetValue("property", 1); break;
                                            case 2: fieldObj.SetValue("event", 0); break;
                                            case 3: fieldObj.SetValue("event", 1); break;
                                            case 4: fieldObj.SetValue("link", 0); break;
                                            case 5: fieldObj.SetValue("link", 1); break;
                                        }
                                    }
                                    else if (arr[0].StartsWith("Type"))
                                    {
                                        DbObject typeObj = new DbObject();
                                        string[] arr2 = arr[1].Split(',');

                                        EbxFieldType fieldType = (EbxFieldType)Enum.Parse(typeof(EbxFieldType), arr2[0]);
                                        typeObj.SetValue("flags", (int)fieldType);

                                        if (arr2.Length > 1 && arr2[1] != "None")
                                        {
                                            typeObj.SetValue("baseType", arr2[1]);
                                        }
                                        if (arr2.Length > 2)
                                        {
                                            EbxFieldType arrayType = (EbxFieldType)Enum.Parse(typeof(EbxFieldType), arr2[2]);
                                            typeObj.SetValue("arrayType", (int)arrayType);
                                        }

                                        fieldObj.SetValue("type", typeObj);
                                    }
                                    else if (arr[0].StartsWith("Version"))
                                    {
                                        string[] arr2 = arr[1].Split(',');
                                        DbObject verObj = new DbObject(false);

                                        foreach (string ver in arr2)
                                        {
                                            verObj.Add(int.Parse(ver));
                                        }

                                        fieldObj.SetValue("version", verObj);
                                    }
                                    else if (arr[0].StartsWith("BeginAccessor"))
                                    {
                                        string functionCode = "";
                                        while (!line.StartsWith("EndAccessor"))
                                        {
                                            line = reader.ReadLine().Trim('\t', ' ');
                                            if (line != "EndAccessor")
                                            {
                                                functionCode += line + "\n";
                                            }
                                        }
                                        fieldObj.SetValue("accessor", functionCode);
                                    }
                                }

                                fieldList.Add(fieldObj);
                            }
                        }
                        else if (line.StartsWith("BeginFunctions"))
                        {
                            string functionCode = "";
                            while (!line.StartsWith("EndFunctions"))
                            {
                                line = reader.ReadLine().Trim('\t', ' ');
                                if (line != "EndFunctions")
                                {
                                    functionCode += line + "\n";
                                }
                            }
                            classObj.SetValue("functions", functionCode);
                        }
                        else if (line.StartsWith("BeginAttributes"))
                        {
                            string attributeCode = "";
                            while (!line.StartsWith("EndAttributes"))
                            {
                                line = reader.ReadLine().Trim('\t', ' ');
                                if (line != "EndAttributes")
                                {
                                    attributeCode += line + "\n";
                                }
                            }
                            classObj.SetValue("attributes", attributeCode);
                        }
                        else if (line.StartsWith("BeginConstructor"))
                        {
                            string constructCode = "";
                            while (!line.StartsWith("EndConstructor"))
                            {
                                line = reader.ReadLine().Trim('\t', ' ');
                                if (line != "EndConstructor")
                                {
                                    constructCode += line + "\n";
                                }
                            }
                            classObj.SetValue("constructor", constructCode);
                        }
                        else
                        {
                            string[] arr = SplitOnce(line);
                            if (arr[0].StartsWith("DisplayName")) classObj.SetValue("displayName", arr[1]);
                            else if (arr[0].StartsWith("Description")) classObj.SetValue("description", arr[1]);
                            else if (arr[0].StartsWith("ValueConverter")) classObj.SetValue("valueConverter", arr[1]);
                            else if (arr[0].StartsWith("Alignment")) classObj.SetValue("alignment", int.Parse(arr[1]));
                            else if (arr[0].StartsWith("Abstract")) classObj.SetValue("abstract", bool.Parse(arr[1]));
                            else if (arr[0].StartsWith("Inline")) classObj.SetValue("inline", bool.Parse(arr[1]));
                            else if (arr[0].StartsWith("Icon")) classObj.SetValue("icon", arr[1]);
                            else if (arr[0].StartsWith("Realm")) classObj.SetValue("realm", arr[1]);
                        }
                    }

                    classList.Add(classObj);
                }
            }

            return classList;
        }

        private static string[] SplitOnce(string str)
        {
            int index = str.IndexOf('=');
            if (index == -1)
            {
                return new string[] { str };
            }

            string[] outArray = new string[2];

            outArray[0] = str.Remove(index);
            outArray[1] = str.Remove(0, index + 1);

            return outArray;
        }

        public static void BuildModule(string sdkFilename, DbObject classList)
        {
            AssemblyName name = new AssemblyName(ModuleName);
            m_assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(ModuleName, sdkFilename + ".dll");

            foreach (DbObject classObj in classList)
            {
                EbxFieldType type = (EbxFieldType)classObj.GetValue<int>("type");
                if (type == EbxFieldType.Struct || type == EbxFieldType.Pointer)
                {
                    AddType(classObj.GetValue<string>("name"));
                }
                else if (type == EbxFieldType.Enum)
                {
                    List<Tuple<string, int>> values = new List<Tuple<string, int>>();
                    foreach (DbObject field in classObj.GetValue<DbObject>("fields"))
                    {
                        values.Add(new Tuple<string, int>(field.GetValue<string>("name"), field.GetValue<int>("value")));
                    }

                    AddEnum(classObj.GetValue<string>("name"), values,
                        new EbxClass()
                        {
                            Alignment = (byte)classObj.GetValue<int>("alignment"),
                            Size = (ushort)classObj.GetValue<int>("size"),
                            Type = (ushort)classObj.GetValue<int>("flags"),
                            Namespace = classObj.GetValue<string>("namespace")
                        });
                }
            }

            foreach (DbObject classObj in classList)
            {
                EbxFieldType classType = (EbxFieldType)classObj.GetValue<int>("type");
                if (classType != EbxFieldType.Struct && classType != EbxFieldType.Pointer)
                {
                    continue;
                }
                
                List<FieldType> fieldTypes = new List<FieldType>();
                foreach (DbObject field in classObj.GetValue<DbObject>("fields"))
                {
                    EbxFieldType fieldType = (EbxFieldType)field.GetValue<int>("type");
                    string baseTypeName = field.GetValue<string>("baseType");
                    Type type = GetTypeFromEbxType(fieldType, baseTypeName, field.GetValue<int>("arrayFlags", -1));

                    Type baseType = null;
                    switch (fieldType)
                    {
                        case EbxFieldType.Pointer:
                            baseType = AddType(baseTypeName);
                            break;
                        case EbxFieldType.Array:
                        {
                            EbxFieldType arrayFieldType = (EbxFieldType)((field.GetValue<int>("arrayFlags") >> 4) & 0x1F);
                            if (arrayFieldType == EbxFieldType.Pointer)
                                baseType = AddType(baseTypeName);
                            break;
                        }
                    }

                    MetaDataType? fieldMetaData = null;
                    if (field.GetValue<DbObject>("meta") != null)
                    {
                        DbObject meta = field.GetValue<DbObject>("meta");
                        fieldMetaData = new MetaDataType(meta);
                    }

                    FieldType fieldTypeObj = new FieldType(
                        field.GetValue<string>("name"),
                        type,
                        baseType,
                        new EbxField()
                        {
                            DataOffset = (uint)field.GetValue<int>("offset"),
                            Type = (ushort)field.GetValue<int>("flags")
                        },
                        (fieldType == EbxFieldType.Array) 
                            ? new EbxField() 
                            {
                                DataOffset = 0,
                                Type = (ushort)field.GetValue<int>("arrayFlags")
                            }
                            : (EbxField?)null,
                        fieldMetaData
                    );
                    fieldTypes.Add(fieldTypeObj);
                }

                Type parentType = null;
                switch (classType)
                {
                    case EbxFieldType.Struct:
                        parentType = typeof(object);
                        break;
                    case EbxFieldType.Pointer when classObj.GetValue<string>("parent") != "":
                        parentType = AddType(classObj.GetValue<string>("parent"));
                        break;
                }

                MetaDataType? metaData = null;
                if (classObj.GetValue<DbObject>("meta") != null)
                {
                    DbObject meta = classObj.GetValue<DbObject>("meta");
                    metaData = new MetaDataType(meta);
                }

                FinalizeClass(classObj.GetValue<string>("name"), fieldTypes, parentType,
                    new EbxClass()
                    {
                        Type = (ushort)classObj.GetValue<int>("flags"),
                        Alignment = (byte)classObj.GetValue<int>("alignment"),
                        Size = (ushort)classObj.GetValue<int>("size"),
                        Namespace = classObj.GetValue<string>("namespace")
                    }, metaData);
            }

            m_assemblyBuilder.Save(sdkFilename + ".dll");
        }

        private static Type GetTypeFromEbxType(EbxFieldType inType, string baseType, int arrayType = -1)
        {
            Type type = null;

            switch (inType)
            {
                case EbxFieldType.Struct: type = AddType(baseType); break;
                case EbxFieldType.String: type = typeof(string); break;
                case EbxFieldType.Int8: type = typeof(sbyte); break;
                case EbxFieldType.UInt8: type = typeof(byte); break;
                case EbxFieldType.Boolean: type = typeof(bool); break;
                case EbxFieldType.UInt16: type = typeof(ushort); break;
                case EbxFieldType.Int16: type = typeof(short); break;
                case EbxFieldType.UInt32: type = typeof(uint); break;
                case EbxFieldType.Int32: type = typeof(int); break;
                case EbxFieldType.UInt64: type = typeof(ulong); break;
                case EbxFieldType.Int64: type = typeof(long); break;
                case EbxFieldType.Float32: type = typeof(float); break;
                case EbxFieldType.Float64: type = typeof(double); break;
                case EbxFieldType.Pointer: type = typeof(Ebx.PointerRef); break;
                case EbxFieldType.Guid: type = typeof(Guid); break;
                case EbxFieldType.Sha1: type = typeof(Sha1); break;
                case EbxFieldType.CString: type = typeof(Ebx.CString); break;
                case EbxFieldType.ResourceRef: type = typeof(Ebx.ResourceRef); break;
                case EbxFieldType.FileRef: type = typeof(Ebx.FileRef); break;
                case EbxFieldType.TypeRef: type = typeof(Ebx.TypeRef); break;
                case EbxFieldType.Array: type = typeof(List<>).MakeGenericType(GetTypeFromEbxType((EbxFieldType)((arrayType >> 4) & 0x1F), baseType)); break;
                case EbxFieldType.Enum: type = AddType(baseType); break;
                case EbxFieldType.DbObject: type = null; break;
                case EbxFieldType.BoxedValueRef: type = typeof(Ebx.BoxedValueRef); break;
            }

            return type;
        }

        public static bool IsSubClassOf(object obj, string name)
        {
            Type type = obj.GetType();
            
            return IsSubClassOf(type, name);
        }

        public static bool IsSubClassOf(Type type, string name)
        {
            Type checkType = GetType(name);
            if (checkType == null)
            {
                return false;
            }

            return type.IsSubclassOf(checkType) || (type == checkType);
        }

        public static bool IsSubClassOf(string type, string name)
        {
            Type sourceType = GetType(type);
            
            return sourceType != null && IsSubClassOf(sourceType, name);
        }

        public static Type[] GetTypes(Type type) => GetTypes(type.Name);

        public static Type[] GetTypes(string name)
        {
            List<Type> totalTypes = new List<Type>();
            Type[] types = (m_existingAssembly != null) ? m_existingAssembly.GetTypes() : null;
            if (types != null)
            {
                foreach (Type type in types)
                {
                    if (IsSubClassOf(type, name))
                    {
                        totalTypes.Add(type);
                    }
                }
            }

            foreach (Type type in m_moduleBuilder.Assembly.GetTypes())
            {
                if (IsSubClassOf(type, name))
                {
                    totalTypes.Add(type);
                }
            }

            return totalTypes.ToArray();
        }

        public static Type[] GetConcreteTypes()
        {
            return (m_existingAssembly != null) ? m_existingAssembly.GetTypes() : new Type[] { };
        }

        /// <summary>
        /// creates a new object of the specified type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic CreateObject(string name)
        {
            Type newType = GetType(name);
            
            return newType == null ? null : CreateObject(newType);
        }

        /// <summary>
        /// creates a new object defined by the specified guid
        /// </summary>
        public static dynamic CreateObject(Guid guid)
        {
            Type newType = GetType(guid);
            
            return newType == null ? null : CreateObject(newType);
        }

        /// <summary>
        /// creates a new object of the specified type and assigns it a guid
        /// </summary>
        /// <param name="name"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        //public static dynamic CreateObject(string name, Guid guid)
        //{
        //    Type newType = GetType(name);
        //    if (newType == null)
        //        return null;

        //    dynamic obj = CreateObject(newType);
        //    obj.GetType().GetField("__Guid", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, guid);

        //    return obj;
        //}

        internal static Type AddEnum(string name, List<Tuple<string, int>> values, EbxClass classInfo)
        {
            Type enumType = GetType(name);
            if (enumType != null)
                return enumType;

            EnumBuilder builder = m_moduleBuilder.DefineEnum(Namespace + name, TypeAttributes.Public, typeof(int));
            for (int i = 0; i < values.Count; i++)
                builder.DefineLiteral(values[i].Item1, values[i].Item2);

            AddEnumMeta(builder, classInfo.Type, classInfo.Alignment, classInfo.Size, classInfo.Namespace);
            return builder.CreateType();
        }

        internal static Type AddType(string name, Guid? guid = null)
        {
            return string.IsNullOrEmpty(name) ? null : CreateType(name, guid);
        }

        internal static Type FinalizeStruct(string name, List<FieldType> fields, EbxClass classInfo)
        {
            return FinalizeType(name, fields, typeof(object), classInfo);
        }

        internal static Type FinalizeClass(string name, List<FieldType> fields, Type parentType, EbxClass classInfo, MetaDataType? metaData = null)
        {
            return FinalizeType(name, fields, parentType, classInfo, metaData);
        }

        public static Type[] GetDerivedTypes(Type type)
        {
            List<Type> derivedTypes = new List<Type>();
            foreach (Type subType in m_existingAssembly.GetExportedTypes())
            {
                if (subType.IsSubclassOf(type))
                    derivedTypes.Add(subType);
            }
            return derivedTypes.ToArray();
        }

        public static Type GetType(string name)
        {
            if (m_existingAssembly != null)
            {
                Type type = m_existingAssembly.GetType(Namespace + name);
                if (type != null)
                {
                    return type;
                }

                type = m_existingAssembly.GetType(Namespace + "Reflection." + name);
                if (type != null)
                {
                    return type;
                }
            }

            return m_moduleBuilder.Assembly.GetType(Namespace + name);
        }

        public static Type GetType(Guid guid)
        {
            if (m_existingAssembly != null)
            {
                if (m_guidTypeMapping.Count == 0)
                {
                    foreach (Type subType in m_existingAssembly.GetExportedTypes())
                    {
                        foreach (TypeInfoGuidAttribute guidAttr in subType.GetCustomAttributes<TypeInfoGuidAttribute>())
                        {
                            m_guidTypeMapping.Add(guidAttr.Guid, subType);
                        }
                    }
                }

                if (m_guidTypeMapping.ContainsKey(guid))
                {
                    return m_guidTypeMapping[guid];
                }
            }
            return null;
        }

        public static Type GetType(uint hash) => Reflection.LookupType(hash);

        internal static dynamic CreateObject(Type inType)
        {
            EbxClassMetaAttribute attr = inType.GetCustomAttribute<EbxClassMetaAttribute>();
            if (attr != null)
            {
                switch (attr.Type)
                {
                    case EbxFieldType.Boolean: inType = typeof(bool); break;
                    case EbxFieldType.BoxedValueRef: inType = typeof(BoxedValueRef); break;
                    case EbxFieldType.CString: inType = typeof(CString); break;
                    case EbxFieldType.Float32: inType = typeof(float); break;
                    case EbxFieldType.Float64: inType = typeof(double); break;
                    case EbxFieldType.Guid: inType = typeof(Guid); break;
                    case EbxFieldType.Int16: inType = typeof(short); break;
                    case EbxFieldType.Int32: inType = typeof(int); break;
                    case EbxFieldType.Int64: inType = typeof(long); break;
                    case EbxFieldType.Int8: inType = typeof(sbyte); break;
                    case EbxFieldType.ResourceRef: inType = typeof(ResourceRef); break;
                    case EbxFieldType.Sha1: inType = typeof(Sha1); break;
                    case EbxFieldType.String: inType = typeof(string); break;
                    case EbxFieldType.TypeRef: inType = typeof(TypeRef); break;
                    case EbxFieldType.UInt16: inType = typeof(ushort); break;
                    case EbxFieldType.UInt32: inType = typeof(uint); break;
                    case EbxFieldType.UInt64: inType = typeof(ulong); break;
                    case EbxFieldType.UInt8: inType = typeof(byte); break;
                    case EbxFieldType.FileRef: inType = typeof(FileRef); break;
                }
            }

            object obj = Activator.CreateInstance(inType);
            return obj;
        }

        internal static void InitializeArrays(object obj)
        {
            Type type = obj.GetType();
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.PropertyType.Namespace == "FrostySdk.Ebx" && pi.PropertyType.BaseType != typeof(Enum))
                {
                    object structObj = pi.GetValue(obj);
                    InitializeArrays(structObj);
                    pi.SetValue(obj, structObj);
                }
                else if (pi.PropertyType.Name == "List`1")
                {
                    object value = pi.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    pi.SetValue(obj, value);
                }
            }
        }

        private static Type CreateType(string name, Guid? guid = null)
        {
            Type retType = GetType(name);
            if (retType != null)
            {
                return retType;
            }

            int id = m_constructingTypes.FindIndex((TypeBuilder a) => a.Name == name);
            if (id != -1)
            {
                return m_constructingTypes[id];
            }

            TypeBuilder builder = m_moduleBuilder.DefineType(Namespace + name, TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout);
            ConstructorBuilder cb = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { });

            m_constructingTypes.Add(builder);
            m_constructingGuids.Add(guid ?? Guid.Empty);
            m_constructors.Add(cb);

            return null;
        }

        private static Type FinalizeType(string name, List<FieldType> fields, Type parentType, EbxClass classInfo, MetaDataType? metaData = null)
        {
            int id = m_constructingTypes.FindIndex((TypeBuilder a) => a.Name == name);

            TypeBuilder builder = m_constructingTypes[id];
            ConstructorBuilder cb = m_constructors[id];
            Guid? guid = m_constructingGuids[id];

            m_constructingTypes.RemoveAt(id);
            m_constructors.RemoveAt(id);

            builder.SetParent(parentType);
            if (parentType == null)
            {
                FieldType guidType = new FieldType("_Guid", typeof(Guid), null, null);
                CreateProperty(builder, guidType, false, false);
                parentType = typeof(Object);
            }

            ILGenerator il = cb.GetILGenerator();

            ConstructorInfo parentCb = null;
            if (parentType is TypeBuilder type)
            {
                int index = m_constructingTypes.IndexOf(type);
                parentCb = m_constructors[index];
            }
            else
            {
                parentCb = parentType.GetConstructor(Type.EmptyTypes);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, parentCb);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);

            foreach (FieldType field in fields)
            {
                if (parentType != null)
                {
                    PropertyInfo pi = parentType.GetProperty(field.Name);
                    if (pi != null)
                        continue;
                }

                FieldBuilder fbuilder = CreateProperty(builder, field, true, true);
                if (field.Type.Name == "List`1")
                {
                    Type genericType = field.Type.GenericTypeArguments[0];
                    ConstructorInfo fieldCb = null;

                    fieldCb = genericType is TypeBuilder ? TypeBuilder.GetConstructor(field.Type, typeof(List<>).GetConstructor(Type.EmptyTypes)) : field.Type.GetConstructor(Type.EmptyTypes);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Newobj, fieldCb);
                    il.Emit(OpCodes.Stfld, fbuilder);
                }
                else if (field.Type.Namespace == "FrostySdk.Ebx" && field.Type.BaseType != typeof(Enum) && field.Type.BaseType != typeof(ValueType))
                {
                    ConstructorInfo fieldCb = null;
                    if (field.Type is TypeBuilder fieldType)
                    {
                        int index = m_constructingTypes.IndexOf(fieldType);
                        fieldCb = m_constructors[index];
                    }
                    else
                    {
                        fieldCb = field.Type.GetConstructor(Type.EmptyTypes);
                    }

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Newobj, fieldCb);
                    il.Emit(OpCodes.Stfld, fbuilder);
                }
            }
            il.Emit(OpCodes.Ret);

            // add mandatory attribute
            AddClassMeta(builder, classInfo.Type, classInfo.Alignment, classInfo.Size, classInfo.Namespace, guid);

            if(metaData.HasValue)
            {
                // add custom class attributes
                MetaDataType meta = metaData.Value;
                if (meta.ValueConverter != "")
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(ClassConverterAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { meta.ValueConverter });
                    builder.SetCustomAttribute(attrBuilder);
                }
                if (meta.Description != "")
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { meta.Description });
                    builder.SetCustomAttribute(attrBuilder);
                }
                if (meta.IsInline)
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(IsInlineAttribute).GetConstructor(Type.EmptyTypes), new object[] { });
                    builder.SetCustomAttribute(attrBuilder);
                }
                if(meta.IsAbstract)
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(IsAbstractAttribute).GetConstructor(Type.EmptyTypes), new object[] { });
                    builder.SetCustomAttribute(attrBuilder);
                }
            }

            return builder.CreateType();
        }

        private static FieldBuilder CreateProperty(TypeBuilder tb, FieldType ft, bool createGetter, bool createSetter)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + ft.Name, ft.Type, 0);
            if (!createGetter && !createSetter)
                return fieldBuilder;

            PropertyBuilder propBuilder = tb.DefineProperty(ft.Name, PropertyAttributes.HasDefault, ft.Type, null);

            if (ft.ArrayInfo != null)
                AddFieldMeta(propBuilder, ft.FieldInfo.Value.Type, ft.FieldInfo.Value.DataOffset, ft.BaseType, true, ft.ArrayInfo.Value.Type);

            else if (ft.FieldInfo != null)
                AddFieldMeta(propBuilder, ft.FieldInfo.Value.Type, ft.FieldInfo.Value.DataOffset, ft.BaseType, false, 0);

            if (createGetter)
            {
                MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + ft.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, ft.Type, Type.EmptyTypes);
                ILGenerator getIl = getPropMthdBldr.GetILGenerator();

                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                propBuilder.SetGetMethod(getPropMthdBldr);
            }

            if (createSetter)
            {
                MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + ft.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { ft.Type });
                ILGenerator setIl = setPropMthdBldr.GetILGenerator();

                Label modifyProp = setIl.DefineLabel();
                Label exitSet = setIl.DefineLabel();

                setIl.MarkLabel(modifyProp);
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);

                setIl.Emit(OpCodes.Nop);
                setIl.MarkLabel(exitSet);
                setIl.Emit(OpCodes.Ret);

                propBuilder.SetSetMethod(setPropMthdBldr);
            }

            if (ft.MetaData.HasValue)
            {
                MetaDataType meta = ft.MetaData.Value;
                if (meta.DisplayName != "")
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(DisplayNameAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { meta.DisplayName });
                    propBuilder.SetCustomAttribute(attrBuilder);
                }
                if (meta.Description != "")
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { meta.Description });
                    propBuilder.SetCustomAttribute(attrBuilder);
                }
                if (meta.Editor != "")
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(EditorAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { meta.Editor });
                    propBuilder.SetCustomAttribute(attrBuilder);
                }
                if (meta.IsReadOnly)
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(IsReadOnlyAttribute).GetConstructor(Type.EmptyTypes), new object[] { });
                    propBuilder.SetCustomAttribute(attrBuilder);
                }
                if (meta.IsReference)
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(IsReferenceAttribute).GetConstructor(Type.EmptyTypes), new object[] { });
                    propBuilder.SetCustomAttribute(attrBuilder);
                }
                if (meta.IsProperty)
                {
                    CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(IsPropertyAttribute).GetConstructor(Type.EmptyTypes), new object[] { });
                    propBuilder.SetCustomAttribute(attrBuilder);
                }
            }

            return fieldBuilder;
        }

        private static void AddClassMeta(TypeBuilder tb, ushort flags, byte alignment, ushort size, string nameSpace, Guid? guid = null)
        {
            CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(EbxClassMetaAttribute).GetConstructor(new Type[] { typeof(ushort), typeof(byte), typeof(ushort), typeof(string) }), new object[] { flags, alignment, size, nameSpace });
            tb.SetCustomAttribute(attrBuilder);

            if (guid.HasValue && guid != Guid.Empty)
            {
                attrBuilder = new CustomAttributeBuilder(typeof(TypeInfoGuidAttribute).GetConstructor(new Type[] { typeof(Guid) }), new object[] { guid });
                tb.SetCustomAttribute(attrBuilder);
            }
        }

        private static void AddEnumMeta(EnumBuilder eb, ushort flags, byte alignment, ushort size, string nameSpace)
        {
            CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(typeof(EbxClassMetaAttribute).GetConstructor(new Type[] { typeof(ushort), typeof(byte), typeof(ushort), typeof(string) }), new object[] { flags, alignment, size, nameSpace });
            eb.SetCustomAttribute(attrBuilder);
        }

        private static void AddFieldMeta(PropertyBuilder pb, ushort flags, uint offset, Type baseType, bool isArray, ushort arrayFlags)
        {
            CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(
                typeof(EbxFieldMetaAttribute).GetConstructor(new Type[] { typeof(ushort), typeof(uint), typeof(Type), typeof(bool), typeof(ushort) }),
                new object[] { flags, offset, baseType, isArray, arrayFlags });
            pb.SetCustomAttribute(attrBuilder);
        }
    }
}
