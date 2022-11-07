using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MeshSetPlugin.Fbx
{
    internal static class Native
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpszLib);
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        internal static extern IntPtr FreeLibrary(IntPtr hModule);
    }

    internal static class FbxUtils
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?FbxMalloc@fbxsdk@@YAPEAX_K@Z")]
        private static extern IntPtr FbxMallocInternal(ulong Size);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?FbxFree@fbxsdk@@YAXPEAX@Z")]
        private static extern void FbxFreeInternal(IntPtr Ptr);

        public static IntPtr FbxMalloc(ulong Size)
        {
            return FbxMallocInternal(Size);
        }

        public static void FbxFree(IntPtr Ptr)
        {
            FbxFreeInternal(Ptr);
        }

        public static unsafe string IntPtrToString(IntPtr InPtr)
        {
            string Str = "";
            byte* b = (byte*)InPtr;

            while ((*b) != 0x00)
            {
                Str += (char)(*b);
                b++;
            }

            return Str;
        }
    }

    internal enum EFbxType
    {
        eFbxUndefined,          //!< Unidentified.
        eFbxChar,               //!< 8 bit signed integer.
        eFbxUChar,              //!< 8 bit unsigned integer.
        eFbxShort,              //!< 16 bit signed integer.
        eFbxUShort,             //!< 16 bit unsigned integer.
        eFbxUInt,               //!< 32 bit unsigned integer.
        eFbxLongLong,           //!< 64 bit signed integer.
        eFbxULongLong,          //!< 64 bit unsigned integer.
        eFbxHalfFloat,          //!< 16 bit floating point.
        eFbxBool,               //!< Boolean.
        eFbxInt,                //!< 32 bit signed integer.
        eFbxFloat,              //!< Floating point value.
        eFbxDouble,             //!< Double width floating point value.
        eFbxDouble2,            //!< Vector of two double values.
        eFbxDouble3,            //!< Vector of three double values.
        eFbxDouble4,            //!< Vector of four double values.
        eFbxDouble4x4,          //!< Four vectors of four double values.
        eFbxEnum = 17,  //!< Enumeration.
        eFbxEnumM = -17,    //!< Enumeration allowing duplicated items.
        eFbxString = 18,    //!< String.
        eFbxTime,               //!< Time value.
        eFbxReference,          //!< Reference to object or property.
        eFbxBlob,               //!< Binary data block type.
        eFbxDistance,           //!< Distance.
        eFbxDateTime,           //!< Date and time.
        eFbxTypeCount = 24  //!< Indicates the number of type identifiers constants.
    };

    internal enum EStatusCode
    {
        eSuccess = 0,                           //!< Operation was successful
        eFailure,                               //!< Operation failed
        eInsufficientMemory,                    //!< Operation failed due to insufficient memory
        eInvalidParameter,                      //!< An invalid parameter was provided
        eIndexOutOfRange,                       //!< Index value outside the valid range
        ePasswordError,                         //!< Operation on FBX file password failed
        eInvalidFileVersion,                    //!< File version not supported (anymore or yet)
        eInvalidFile                            //!< Operation on the file access failed
    };

    internal class FbxNative
    {
        protected IntPtr pHandle;
        protected IntPtr vTable;

        internal IntPtr Handle => pHandle;
        protected bool bNeedsFreeing;

        public FbxNative() { }
        public FbxNative(IntPtr InHandle)
        {
            pHandle = InHandle;
            vTable = Marshal.ReadIntPtr(pHandle, 0);
        }
        ~FbxNative()
        {
            if (bNeedsFreeing)
            {
                FbxUtils.FbxFree(pHandle);
            }
        }
    }

    internal class FbxManager : FbxNative, IDisposable
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxManager@fbxsdk@@SAPEAV12@XZ")]
        private static extern IntPtr CreateInternal();

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Destroy@FbxManager@fbxsdk@@UEAAXXZ")]
        private static extern IntPtr DestroyInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetIOSettings@FbxManager@fbxsdk@@UEAAXPEAVFbxIOSettings@2@@Z")]
        private static extern void SetIOSettingsInternal(IntPtr handle, IntPtr pIOSettings);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIOSettings@FbxManager@fbxsdk@@UEBAPEAVFbxIOSettings@2@XZ")]
        private static extern IntPtr GetIOSettingsInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetVersion@FbxManager@fbxsdk@@SAPEBD_N@Z", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetVersionInternal(IntPtr InHandle, bool pFull);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetFileFormatVersion@FbxManager@fbxsdk@@SAXAEAH00@Z")]
        private static extern void GetFileFormatVersionInternal(ref int pMajor, ref int pMinor, ref int pRevision);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIOPluginRegistry@FbxManager@fbxsdk@@QEBAPEAVFbxIOPluginRegistry@2@XZ")]
        private static extern IntPtr GetIOPluginRegistryInternal(IntPtr InHandle);

        public static void GetFileFormatVersion(out int pMajor, out int pMinor, out int pRevision)
        {
            pMajor = 0; pMinor = 0; pRevision = 0;
            GetFileFormatVersionInternal(ref pMajor, ref pMinor, ref pRevision);
        }

        public FbxManager()
        {
            pHandle = CreateInternal();
        }

        ~FbxManager()
        {
            Dispose(false);
        }

        public FbxIOPluginRegistry IOPluginRegistry => new FbxIOPluginRegistry(GetIOPluginRegistryInternal(pHandle));

        public void SetIOSettings(FbxIOSettings settings)
        {
            SetIOSettingsInternal(pHandle, settings.Handle);
        }

        public FbxIOSettings GetIOSettings()
        {
            return new FbxIOSettings(GetIOSettingsInternal(pHandle));
        }

        public string GetVersion(bool pFull = true)
        {
            IntPtr StrPtr = GetVersionInternal(pHandle, pFull);
            return FbxUtils.IntPtrToString(StrPtr);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (pHandle != IntPtr.Zero)
            {
                DestroyInternal(pHandle);
                pHandle = IntPtr.Zero;
            }

            if (bDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }

    internal class FbxIOPluginRegistry : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetWriterFormatCount@FbxIOPluginRegistry@fbxsdk@@QEBAHXZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern int GetWriterFormatCountInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?WriterIsFBX@FbxIOPluginRegistry@fbxsdk@@QEBA_NH@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool WriterIsFBXInternal(IntPtr InHandle, int pFileFormat);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetWriterFormatDescription@FbxIOPluginRegistry@fbxsdk@@QEBAPEBDH@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetWriterFormatDescriptionInternal(IntPtr InHandle, int pFileFormat);

        public FbxIOPluginRegistry(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public int WriterFormatCount => GetWriterFormatCountInternal(pHandle);
        public bool WriterIsFBX(int pFileFormat) { return WriterIsFBXInternal(pHandle, pFileFormat); }

        public string GetWriterFormatDescription(int pFileFormat)
        {
            IntPtr Ptr = GetWriterFormatDescriptionInternal(pHandle, pFileFormat);
            if (Ptr == IntPtr.Zero)
            {
                return "";
            }

            return FbxUtils.IntPtrToString(Ptr);
        }
    }

    internal class FbxObject : FbxNative, IDisposable
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Destroy@FbxObject@fbxsdk@@QEAAX_N@Z", CallingConvention = CallingConvention.ThisCall)]
        protected static extern IntPtr DestroyInternal(IntPtr InHandle, bool pRecursive);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetName@FbxObject@fbxsdk@@QEBAPEBDXZ", CallingConvention = CallingConvention.ThisCall)]
        protected static extern IntPtr GetNameInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetName@FbxObject@fbxsdk@@QEAAXPEBD@Z", CallingConvention = CallingConvention.ThisCall)]
        protected static extern void SetNameInternal(IntPtr InHandle, [MarshalAs(UnmanagedType.LPStr)] string pName);

        internal FbxObject()
        {
        }

        public FbxObject(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public string Name
        {
            get
            {
                IntPtr StrPtr = GetNameInternal(Handle);
                return FbxUtils.IntPtrToString(StrPtr);
            }
            set => SetNameInternal(Handle, value);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (pHandle != IntPtr.Zero)
            {
                DestroyInternal(pHandle, false);
                pHandle = IntPtr.Zero;
            }

            if (bDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public override int GetHashCode()
        {
            return pHandle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is FbxObject b)
            {
                return pHandle == b.Handle;
            }
            return false;
        }

        public static bool operator ==(FbxObject a, FbxObject b)
        {
            return a?.Equals(b) ?? b is null;
        }

        public static bool operator !=(FbxObject a, FbxObject b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a.Equals(b);
        }
    }

    internal class FbxIOSettings : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxIOSettings@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateInternal(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetBoolProp@FbxIOSettings@fbxsdk@@QEAAXPEBD_N@Z")]
        private static extern void SetBoolPropInternal(IntPtr InHandle, [MarshalAs(UnmanagedType.LPStr)] string pName, bool pValue);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetBoolProp@FbxIOSettings@fbxsdk@@QEBA_NPEBD_N@Z")]
        private static extern bool GetBoolPropInternal(IntPtr InHandle, [MarshalAs(UnmanagedType.LPStr)] string pName);

        #region -- Strings --

        public const string IOSROOT = "IOSRoot";
        public const string IOSN_EXPORT = "Export";
        public const string IOSN_ADV_OPT_GRP = "AdvOptGrp";
        public const string IOSN_FBX = "Fbx";
        public const string IOSN_MATERIAL = "Material";
        public const string IOSN_TEXTURE = "Texture";
        public const string IOSN_GLOBAL_SETTINGS = "Global_Settings";
        public const string IOSN_SHAPE = "Shape";
        public const string EXP_ADV_OPT_GRP = IOSN_EXPORT + "|" + IOSN_ADV_OPT_GRP;
        public const string EXP_FBX = EXP_ADV_OPT_GRP + "|" + IOSN_FBX;
        public const string EXP_FBX_MATERIAL = EXP_FBX + "|" + IOSN_MATERIAL;
        public const string EXP_FBX_TEXTURE = EXP_FBX + "|" + IOSN_TEXTURE;
        public const string EXP_FBX_GLOBAL_SETTINGS = EXP_FBX + "|" + IOSN_GLOBAL_SETTINGS;
        public const string EXP_FBX_SHAPE = EXP_FBX + "|" + IOSN_SHAPE;

        #endregion

        public FbxIOSettings() { }
        public FbxIOSettings(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public FbxIOSettings(FbxManager Manager, string Name)
        {
            pHandle = CreateInternal(Manager.Handle, Name);
        }

        public void SetBoolProp(string pName, bool pValue)
        {
            SetBoolPropInternal(pHandle, pName, pValue);
        }

        public bool GetBoolProp(string pName)
        {
            return GetBoolPropInternal(pHandle, pName);
        }
    }

    internal class FbxSystemUnit : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetScaleFactor@FbxSystemUnit@fbxsdk@@QEBANXZ")]
        private static extern double GetScaleFactorInternal(IntPtr InHandle);

        private static FbxSystemUnit mMillimeters;
        private static FbxSystemUnit mCentimeters;
        private static FbxSystemUnit mMeters;
        private static FbxSystemUnit mKilometers;

        public FbxSystemUnit() { }
        public FbxSystemUnit(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public double ScaleFactor => GetScaleFactorInternal(pHandle);

        public static FbxSystemUnit Millimeters => GetStaticValue("?mm@FbxSystemUnit@fbxsdk@@2V12@B", ref mMillimeters);
        public static FbxSystemUnit Centimeters => GetStaticValue("?cm@FbxSystemUnit@fbxsdk@@2V12@B", ref mCentimeters);
        public static FbxSystemUnit Meters => GetStaticValue("?m@FbxSystemUnit@fbxsdk@@2V12@B", ref mMeters);
        public static FbxSystemUnit Kilometers => GetStaticValue("?km@FbxSystemUnit@fbxsdk@@2V12@B", ref mKilometers);

        private static FbxSystemUnit GetStaticValue(string Sig, ref FbxSystemUnit OutUnit)
        {
            if (OutUnit == null)
            {
                IntPtr Module = Native.LoadLibrary("libfbxsdk.dll");
                OutUnit = new FbxSystemUnit(Native.GetProcAddress(Module, Sig));
                Native.FreeLibrary(Module);
            }
            return OutUnit;
        }
    }

    internal class FbxGlobalSettings : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetSystemUnit@FbxGlobalSettings@fbxsdk@@QEAAXAEBVFbxSystemUnit@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetSystemUnitInternal(IntPtr InHandle, IntPtr pOther);

        public FbxGlobalSettings() { }
        public FbxGlobalSettings(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public void SetSystemUnit(FbxSystemUnit pOther)
        {
            IntPtr Ptr = pOther.Handle;
            SetSystemUnitInternal(pHandle, Ptr);
        }
    }

    internal class FbxCollection : FbxObject
    {
        public FbxCollection() { }
        public FbxCollection(IntPtr InHandle)
            : base(InHandle)
        {
        }
    }

    internal class FbxDocument : FbxCollection
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDocumentInfo@FbxDocument@fbxsdk@@QEBAPEAVFbxDocumentInfo@2@XZ")]
        protected static extern IntPtr GetDocumentInfoInternal(IntPtr handle);

        public FbxDocument() { }
        public FbxDocument(IntPtr InHandle)
            : base(InHandle)
        {
        }
    }

    internal class FbxScene : FbxDocument
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxScene@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxScene@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetSceneInfo@FbxScene@fbxsdk@@QEAAXPEAVFbxDocumentInfo@2@@Z")]
        private static extern void SetSceneInfoInternal(IntPtr InHandle, IntPtr pSceneInfo);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetGlobalSettings@FbxScene@fbxsdk@@QEBAAEBVFbxGlobalSettings@2@XZ")]
        private static extern IntPtr GetGlobalSettingsInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetRootNode@FbxScene@fbxsdk@@QEBAPEAVFbxNode@2@XZ")]
        private static extern IntPtr GetRootNodeInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetSceneInfo@FbxScene@fbxsdk@@QEAAPEAVFbxDocumentInfo@2@XZ")]
        private static extern IntPtr GetSceneInfoInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?AddPose@FbxScene@fbxsdk@@QEAA_NPEAVFbxPose@2@@Z")]
        private static extern bool AddPoseInternal(IntPtr pHandle, IntPtr pPose);

        public FbxGlobalSettings GlobalSettings => new FbxGlobalSettings(GetGlobalSettingsInternal(pHandle));
        public FbxNode RootNode => new FbxNode(GetRootNodeInternal(pHandle));

        public FbxDocumentInfo SceneInfo
        {
            get
            {
                IntPtr p = GetSceneInfoInternal(pHandle);
                return (p != IntPtr.Zero) ? new FbxDocumentInfo(GetSceneInfoInternal(pHandle)) : null;
            }
            set => SetSceneInfoInternal(pHandle, value.Handle);
        }

        public FbxScene(FbxManager Manager, string Name)
        {
            pHandle = CreateFromManager(Manager.Handle, Name);
        }

        public FbxScene(FbxObject Object, string Name)
        {
            pHandle = CreateFromObject(Object.Handle, Name);
        }

        public bool AddPose(FbxPose pose)
        {
            return AddPoseInternal(pHandle, pose.Handle);
        }
    }

    internal class FbxStatus : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetErrorString@FbxStatus@fbxsdk@@QEBAPEBDXZ")]
        private static extern IntPtr GetErrorStringInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetCode@FbxStatus@fbxsdk@@QEBA?AW4EStatusCode@12@XZ")]
        private static extern EStatusCode GetCodeInternal(IntPtr handle);

        public string ErrorString => FbxUtils.IntPtrToString(GetErrorStringInternal(pHandle));
        public EStatusCode Code => GetCodeInternal(pHandle);

        public FbxStatus(IntPtr handle)
        {
            pHandle = handle;
        }
    }

    internal class FbxIOBase : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetStatus@FbxIOBase@fbxsdk@@QEAAAEAVFbxStatus@2@XZ")]
        private static extern IntPtr GetStatusInternal(IntPtr handle);

        public FbxStatus Status => new FbxStatus(GetStatusInternal(pHandle));
    }

    internal class FbxImporter : FbxIOBase
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxImporter@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Initialize@FbxImporter@fbxsdk@@UEAA_NPEBDHPEAVFbxIOSettings@2@@Z")]
        private static extern bool InitializeInternal(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string pFilename, int pFileFormat, IntPtr pIOSettings);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetFileVersion@FbxImporter@fbxsdk@@QEAAXAEAH00@Z")]
        private static extern void GetFileVersionInternal(IntPtr handle, out int pMajor, out int pMinor, out int pRevision);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?IsFBX@FbxImporter@fbxsdk@@QEAA_NXZ")]
        private static extern bool IsFBXInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Import@FbxImporter@fbxsdk@@QEAA_NPEAVFbxDocument@2@_N@Z")]
        private static extern bool ImportInternal(IntPtr handle, IntPtr pDocument, bool pNonBlocking);

        public FbxImporter(FbxManager Manager, string Name)
        {
            pHandle = CreateFromManager(Manager.Handle, Name);
        }

        public bool Initialize(string filename, int fileFormat = -1, FbxIOSettings ioSettings = null)
        {
            return InitializeInternal(pHandle, filename, fileFormat, ioSettings.Handle);
        }

        public void GetFileVersion(out int major, out int minor, out int revision)
        {
            GetFileVersionInternal(pHandle, out major, out minor, out revision);
        }

        public bool IsFBX()
        {
            return IsFBXInternal(pHandle);
        }

        public bool Import(FbxDocument document, bool nonBlocking = false)
        {
            return ImportInternal(pHandle, document.Handle, nonBlocking);
        }
    }

    internal class FbxExporter : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxExporter@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Initialize@FbxExporter@fbxsdk@@UEAA_NPEBDHPEAVFbxIOSettings@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool InitializeInternal(IntPtr InHandle, [MarshalAs(UnmanagedType.LPStr)] string pFileName, int pFileFormat, IntPtr pIOSettings);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Export@FbxExporter@fbxsdk@@QEAA_NPEAVFbxDocument@2@_N@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool ExportInternal(IntPtr InHandle, IntPtr pDocument, bool pNonBlocking);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetFileExportVersion@FbxExporter@fbxsdk@@QEAA_NVFbxString@2@W4ERenamingMode@FbxSceneRenamer@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool SetFileExportVersionInternal(IntPtr InHandle, IntPtr pVersion, int pRenamingMode);

        public FbxExporter(FbxManager Manager, string Name)
        {
            pHandle = CreateFromManager(Manager.Handle, Name);
        }

        public bool Initialize(string pFileName, int pFileFormat = -1, FbxIOSettings pIOSettings = null)
        {
            IntPtr Ptr = (pIOSettings != null) ? pIOSettings.Handle : IntPtr.Zero;
            return InitializeInternal(pHandle, pFileName, pFileFormat, Ptr);
        }

        public bool Export(FbxDocument pDocument, bool pNonBlocking = false)
        {
            return ExportInternal(pHandle, pDocument.Handle, pNonBlocking);
        }

        public bool SetFileExportVersion(string pVersion)
        {
            IntPtr Ptr = FbxString.Construct(pVersion);
            return SetFileExportVersionInternal(pHandle, Ptr, 0);
        }
    }

    internal class FbxDocumentInfo : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxDocumentInfo@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        private IntPtr mOriginal_ApplicationVendor; // FbxPropertyT<FbxString>
        private IntPtr mOriginal_ApplicationName; // FbxPropertyT<FbxString>
        private IntPtr mOriginal_ApplicationVersion; // FbxPropertyT<FbxString>

        private IntPtr mLastSaved_ApplicationVendor; // FbxPropertyT<FbxString>
        private IntPtr mLastSaved_ApplicationName; // FbxPropertyT<FbxString>
        private IntPtr mLastSaved_ApplicationVersion; // FbxPropertyT<FbxString>

        private IntPtr mTitle; // FbxString
        private IntPtr mSubject; // FbxString

        public FbxDocumentInfo(IntPtr handle)
            : base(handle)
        {
            mOriginal_ApplicationVendor = pHandle + 0xA8;
            mOriginal_ApplicationName = pHandle + 0xB8;
            mOriginal_ApplicationVersion = pHandle + 0xC8;

            mLastSaved_ApplicationVendor = pHandle + 0x108;
            mLastSaved_ApplicationName = pHandle + 0x118;
            mLastSaved_ApplicationVersion = pHandle + 0x128;

            mTitle = pHandle + 0x158;
            mSubject = pHandle + 0x160;
        }

        public FbxDocumentInfo(FbxManager Manager, string Name)
            : this(CreateFromManager(Manager.Handle, Name))
        { 
        }

        public string OriginalApplicationVendor { get => FbxProperty.GetString(mOriginal_ApplicationVendor); set => FbxProperty.Set(mOriginal_ApplicationVendor, value); }
        public string OriginalApplicationName { get => FbxProperty.GetString(mOriginal_ApplicationName); set => FbxProperty.Set(mOriginal_ApplicationName, value); }
        public string OriginalApplicationVersion { get => FbxProperty.GetString(mOriginal_ApplicationVersion); set => FbxProperty.Set(mOriginal_ApplicationVersion, value); }

        public string LastSavedApplicationVendor { get => FbxProperty.GetString(mLastSaved_ApplicationVendor); set => FbxProperty.Set(mLastSaved_ApplicationVendor, value); }
        public string LastSavedApplicationName { get => FbxProperty.GetString(mLastSaved_ApplicationName); set => FbxProperty.Set(mLastSaved_ApplicationName, value); }
        public string LastSavedApplicationVersion { get => FbxProperty.GetString(mLastSaved_ApplicationVersion); set => FbxProperty.Set(mLastSaved_ApplicationVersion, value); }

        public string Title { get => FbxString.Get(mTitle); set => FbxString.Assign(mTitle, value); }
        public string Subject { get => FbxString.Get(mSubject); set => FbxString.Assign(mSubject, value); }
    }

    internal class FbxLayer : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetNormals@FbxLayer@fbxsdk@@QEAAXPEAVFbxLayerElementNormal@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetNormalsInternal(IntPtr InHandle, IntPtr pNormals);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetTangents@FbxLayer@fbxsdk@@QEAAXPEAVFbxLayerElementTangent@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetTangentsInternal(IntPtr InHandle, IntPtr pTangents);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetBinormals@FbxLayer@fbxsdk@@QEAAXPEAVFbxLayerElementBinormal@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetBinormalsInternal(IntPtr InHandle, IntPtr pBinormals);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetUVs@FbxLayer@fbxsdk@@QEAAXPEAVFbxLayerElementUV@2@W4EType@FbxLayerElement@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetUVsInternal(IntPtr InHandle, IntPtr pUVs, FbxLayerElement.EType pTypeIdentifier);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetVertexColors@FbxLayer@fbxsdk@@QEAAXPEAVFbxLayerElementVertexColor@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetVertexColorsInternal(IntPtr InHandle, IntPtr pVertexColors);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetUserData@FbxLayer@fbxsdk@@QEAAXPEAVFbxLayerElementUserData@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetUserDataInternal(IntPtr InHandle, IntPtr pUserData);

        public FbxLayer(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public void SetNormals(FbxLayerElementNormal pNormals)
        {
            SetNormalsInternal(pHandle, pNormals.Handle);
        }

        public void SetTangents(FbxLayerElementTangent pTangents)
        {
            SetTangentsInternal(pHandle, pTangents.Handle);
        }

        public void SetBinormals(FbxLayerElementBinormal pBinormals)
        {
            SetBinormalsInternal(pHandle, pBinormals.Handle);
        }

        public void SetUVs(FbxLayerElementUV pUVs)
        {
            SetUVsInternal(pHandle, pUVs.Handle, FbxLayerElement.EType.eTextureDiffuse);
        }

        public void SetVertexColors(FbxLayerElementVertexColor pVertexColors)
        {
            SetVertexColorsInternal(pHandle, pVertexColors.Handle);
        }

        public void SetUserData(FbxLayerElementUserData pUserData)
        {
            SetUserDataInternal(pHandle, pUserData.Handle);
        }
    }

    internal class FbxNode : FbxObject
    {
        public enum EPivotSet
        {
            eSourcePivot,
            eDestinationPivot 
        };

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxNode@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxNode@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetNodeAttribute@FbxNode@fbxsdk@@QEAAPEAVFbxNodeAttribute@2@PEAV32@@Z")]
        private static extern IntPtr SetNodeAttributeInternal(IntPtr InHandle, IntPtr pNodeAttribute);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?AddChild@FbxNode@fbxsdk@@QEAA_NPEAV12@@Z")]
        private static extern bool AddChildInternal(IntPtr InHandle, IntPtr pNode);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetNodeAttribute@FbxNode@fbxsdk@@QEAAPEAVFbxNodeAttribute@2@XZ")]
        private static extern IntPtr GetNodeAttributeInternal(IntPtr inHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetNodeAttributeCount@FbxNode@fbxsdk@@QEBAHXZ")]
        private static extern int GetNodeAttributeCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetNodeAttributeByIndex@FbxNode@fbxsdk@@QEAAPEAVFbxNodeAttribute@2@H@Z")]
        private static extern IntPtr GetNodeAttributeByIndexInternal(IntPtr handle, int pIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?EvaluateGlobalTransform@FbxNode@fbxsdk@@QEAAAEAVFbxAMatrix@2@VFbxTime@2@W4EPivotSet@12@_N2@Z")]
        private static extern IntPtr EvaluateGlobalTransformInternal(IntPtr inHandle, IntPtr pTime, EPivotSet pPivotSet, bool pApplyTarget, bool pForceEval);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetParent@FbxNode@fbxsdk@@QEAAPEAV12@XZ")]
        private static extern IntPtr GetParentInternal(IntPtr pHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetChildCount@FbxNode@fbxsdk@@QEBAH_N@Z")]
        private static extern int GetChildCountInternal(IntPtr handle, bool pRecursive);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetChild@FbxNode@fbxsdk@@QEAAPEAV12@H@Z")]
        private static extern IntPtr GetChildInternal(IntPtr handle, int pIndex);

        private IntPtr lclTranslation;
        private IntPtr lclRotation;
        private IntPtr lclScaling;
        private IntPtr visibility;

        public SharpDX.Vector3 LclTranslation { get => FbxProperty.GetDouble3(lclTranslation); set => FbxProperty.Set(lclTranslation, value); }
        public SharpDX.Vector3 LclRotation { get => FbxProperty.GetDouble3(lclRotation); set => FbxProperty.Set(lclRotation, value); }
        public SharpDX.Vector3 LclScaling { get => FbxProperty.GetDouble3(lclScaling); set => FbxProperty.Set(lclScaling, value); }
        public double Visibility { get => FbxProperty.GetDouble(visibility); set => FbxProperty.Set(visibility, value); }
        public int ChildCount => GetChildCountInternal(pHandle, false);
        public int NodeAttributeCount => GetNodeAttributeCountInternal(pHandle);

        public IEnumerable<FbxNode> Children
        {
            get
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    yield return GetChild(i);
                }
            }
        }

        public FbxNode(FbxManager Manager, string pName)
        {
            pHandle = CreateFromManager(Manager.Handle, pName);

            lclTranslation = pHandle + 0x78;
            lclRotation = pHandle + 0x88;
            lclScaling = pHandle + 0x98;
            visibility = pHandle + 0xA8;
        }

        public FbxNode(IntPtr InHandle)
            : base(InHandle)
        {
            lclTranslation = pHandle + 0x78;
            lclRotation = pHandle + 0x88;
            lclScaling = pHandle + 0x98;
            visibility = pHandle + 0xA8;
        }

        public FbxNode(FbxObject Object, string pName)
        {
            pHandle = CreateFromObject(Object.Handle, pName);

            lclTranslation = pHandle + 0x78;
            lclRotation = pHandle + 0x88;
            lclScaling = pHandle + 0x98;
            visibility = pHandle + 0xA8;
        }

        public FbxNodeAttribute SetNodeAttribute(FbxNodeAttribute pNodeAttribute)
        {
            IntPtr Ptr = SetNodeAttributeInternal(pHandle, pNodeAttribute.Handle);
            return Ptr == IntPtr.Zero ? null : new FbxNodeAttribute(Ptr);
        }

        public FbxNodeAttribute GetNodeAttribute(FbxNodeAttribute.EType type)
        {
            for (int i = 0; i < NodeAttributeCount; i++)
            {
                IntPtr ptr = GetNodeAttributeByIndexInternal(pHandle, i);
                if (ptr != IntPtr.Zero)
                {
                    FbxNodeAttribute attr = new FbxNodeAttribute(ptr);
                    if (attr.AttributeType == type)
                    {
                        return attr;
                    }
                }
            }
            return null;
        }

        public bool AddChild(FbxNode pNode)
        {
            return AddChildInternal(pHandle, pNode.Handle);
        }

        public FbxAMatrix EvaluateGlobalTransform(FbxTime time = null, EPivotSet pivotSet = EPivotSet.eSourcePivot, bool applyTarget = false, bool forceEval = false)
        {
            if (time == null)
            {
                time = FbxTime.FBXSDK_TIME_INFINITE;
            }

            IntPtr ptr = EvaluateGlobalTransformInternal(pHandle, time.Handle, pivotSet, applyTarget, forceEval);
            return new FbxAMatrix(ptr);
        }

        public FbxNode GetParent()
        {
            IntPtr ptr = GetParentInternal(pHandle);
            return ptr == IntPtr.Zero ? null : new FbxNode(ptr);
        }

        public FbxNode GetChild(int index)
        {
            IntPtr ptr = GetChildInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxNode(ptr);
        }
    }

    internal class FbxDeformer : FbxObject
    {
        public enum EDeformerType
        {
            eUnknown,
            eSkin,
            eBlendShape,
            eVertexCache
        };

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxDeformer@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxDeformer@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        private delegate EDeformerType GetDeformerTypeDelegate(IntPtr handle);
        private GetDeformerTypeDelegate GetDeformerTypeInternal;

        public EDeformerType DeformerType => GetDeformerTypeInternal(pHandle);

        public FbxDeformer(IntPtr InHandle)
            : base(InHandle)
        {
            GetDeformerTypeInternal = Marshal.GetDelegateForFunctionPointer<GetDeformerTypeDelegate>(Marshal.ReadIntPtr(vTable + 0xB8));
        }

        public FbxDeformer(FbxManager manager, string name)
            : this(CreateFromManager(manager.Handle, name))
        {
        }

        public FbxDeformer(FbxObject obj, string name)
            : this(CreateFromObject(obj.Handle, name))
        {
        }
    }

    internal class FbxSkin : FbxDeformer
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxSkin@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxSkin@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?AddCluster@FbxSkin@fbxsdk@@QEAA_NPEAVFbxCluster@2@@Z")]
        private static extern bool AddClusterInternal(IntPtr pHandle, IntPtr pCluster);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetClusterCount@FbxSkin@fbxsdk@@QEBAHXZ")]
        private static extern int GetClusterCountInternal(IntPtr pHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetCluster@FbxSkin@fbxsdk@@QEAAPEAVFbxCluster@2@H@Z")]
        private static extern IntPtr GetClusterInternal(IntPtr pHandle, int pIndex);

        public int ClusterCount => GetClusterCountInternal(pHandle);

        public IEnumerable<FbxCluster> Clusters
        {
            get
            {
                for (int i = 0; i < ClusterCount; i++)
                {
                    yield return GetCluster(i);
                }
            }
        }

        public FbxSkin(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public FbxSkin(FbxDeformer deformer)
            : this(deformer.Handle)
        {
        }

        public FbxSkin(FbxManager manager, string name)
            : this(CreateFromManager(manager.Handle, name))
        {
        }

        public FbxSkin(FbxObject obj, string name)
            : this(CreateFromObject(obj.Handle, name))
        {
        }

        public bool AddCluster(FbxCluster cluster)
        {
            return AddClusterInternal(pHandle, cluster.Handle);
        }

        public FbxCluster GetCluster(int index)
        {
            IntPtr ptr = GetClusterInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxCluster(ptr);
        }
    }

    internal class FbxSubDeformer : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxSubDeformer@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxSubDeformer@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        internal FbxSubDeformer()
        {
        }

        public FbxSubDeformer(FbxManager manager, string name)
        {
            pHandle = CreateFromManager(manager.Handle, name);
        }

        public FbxSubDeformer(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public FbxSubDeformer(FbxObject obj, string name)
        {
            pHandle = CreateFromObject(obj.Handle, name);
        }
    }

    internal class FbxCluster : FbxSubDeformer
    {
        public enum ELinkMode
        {
            eNormalize,
            eAdditive,
            eTotalOne
        };

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxCluster@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxCluster@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetLink@FbxCluster@fbxsdk@@QEAAXPEBVFbxNode@2@@Z")]
        private static extern void SetLinkInternal(IntPtr pHandle, IntPtr pNode);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetLinkMode@FbxCluster@fbxsdk@@QEBA?AW4ELinkMode@12@XZ")]
        private static extern ELinkMode GetLinkModeInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetLinkMode@FbxCluster@fbxsdk@@QEAAXW4ELinkMode@12@@Z")]
        private static extern void SetLinkModeInternal(IntPtr pHandle, ELinkMode pMode);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?AddControlPointIndex@FbxCluster@fbxsdk@@QEAAXHN@Z")]
        private static extern void AddControlPointIndexInternal(IntPtr pHandle, int pIndex, double pWeight);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetControlPointIndicesCount@FbxCluster@fbxsdk@@QEBAHXZ")]
        private static extern int GetControlPointIndicesCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetControlPointIndices@FbxCluster@fbxsdk@@QEBAPEAHXZ")]
        private static extern IntPtr GetControlPointIndicesInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetControlPointWeights@FbxCluster@fbxsdk@@QEBAPEANXZ")]
        private static extern IntPtr GetControlPointWeightsInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetTransformLinkMatrix@FbxCluster@fbxsdk@@QEAAXAEBVFbxAMatrix@2@@Z")]
        private static extern void SetTransformLinkMatrixInternal(IntPtr pHandle, IntPtr pMatrix);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetLink@FbxCluster@fbxsdk@@QEAAPEAVFbxNode@2@XZ")]
        private static extern IntPtr GetLinkInternal(IntPtr pHandle);

        public int ControlPointIndicesCount => GetControlPointIndicesCountInternal(pHandle);
        public ELinkMode LinkMode { get => GetLinkModeInternal(pHandle); set => SetLinkModeInternal(pHandle, value); }

        public FbxCluster(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public FbxCluster(FbxManager manager, string name)
            : this(CreateFromManager(manager.Handle, name))
        {
        }

        public FbxCluster(FbxObject obj, string name)
            : this(CreateFromObject(obj.Handle, name))
        {
        }

        public void SetLink(FbxNode node)
        {
            SetLinkInternal(pHandle, node.Handle);
        }

        public FbxNode GetLink()
        {
            IntPtr ptr = GetLinkInternal(pHandle);
            return new FbxNode(ptr);
        }

        public void SetLinkMode(ELinkMode mode)
        {
            SetLinkModeInternal(pHandle, mode);
        }

        public void AddControlPointIndex(int index, double weight)
        {
            AddControlPointIndexInternal(pHandle, index, weight);
        }

        public void SetTransformLinkMatrix(FbxAMatrix matrix)
        {
            SetTransformLinkMatrixInternal(pHandle, matrix.Handle);
        }

        public int[] GetControlPointIndices()
        {
            IntPtr ptr = GetControlPointIndicesInternal(pHandle);
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            int[] outBuf = new int[ControlPointIndicesCount];
            Marshal.Copy(ptr, outBuf, 0, ControlPointIndicesCount);

            return outBuf;
        }

        public double[] GetControlPointWeights()
        {
            IntPtr ptr = GetControlPointWeightsInternal(pHandle);
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            double[] outBuf = new double[ControlPointIndicesCount];
            Marshal.Copy(ptr, outBuf, 0, ControlPointIndicesCount);

            return outBuf;
        }
    }

    internal class FbxPose : FbxObject
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxPose@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxPose@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetIsBindPose@FbxPose@fbxsdk@@QEAAX_N@Z")]
        private static extern void SetIsBindPoseInternal(IntPtr pHandle, bool pIsBindPose);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Add@FbxPose@fbxsdk@@QEAAHPEAVFbxNode@2@AEBVFbxMatrix@2@_N2@Z")]
        private static extern int AddInternal(IntPtr pHandle, IntPtr pNode, IntPtr pMatrix, bool pLocalMatrix, bool pMultipleBindPose);

        public bool IsBindPose { get { unsafe { return *((char*)mType) == 'b'; } } set => SetIsBindPoseInternal(pHandle, value);
        }
        private IntPtr mType;

        public FbxPose(FbxManager manager, string name)
        {
            pHandle = CreateFromManager(manager.Handle, name);
            mType = pHandle + 0x78;
        }

        public FbxPose(IntPtr InHandle)
            : base(InHandle)
        {
            mType = pHandle + 0x78;
        }

        public FbxPose(FbxObject obj, string name)
        {
            pHandle = CreateFromObject(obj.Handle, name);
            mType = pHandle + 0x78;
        }

        public int Add(FbxNode node, FbxMatrix matrix, bool localMatrix = false, bool multipleBindPose = false)
        {
            return AddInternal(pHandle, node.Handle, matrix.Handle, localMatrix, multipleBindPose);
        }
    }

    internal class FbxSkeleton : FbxNodeAttribute
    {
        new public enum EType
        {
            eRoot,          /*!< First element of a chain. */
            eLimb,          /*!< Chain element. */
            eLimbNode,      /*!< Chain element. */
            eEffector       /*!< Last element of a chain. */
        };

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxSkeleton@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxSkeleton@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetSkeletonType@FbxSkeleton@fbxsdk@@QEAAXW4EType@12@@Z")]
        private static extern void SetSkeletonTypeInternal(IntPtr inHandle, EType pSkeletonType);

        private IntPtr size;
        public double Size { get => FbxProperty.GetDouble(size); set => FbxProperty.Set(size, value); }

        public FbxSkeleton(FbxManager Manager, string pName)
        {
            pHandle = CreateFromManager(Manager.Handle, pName);
            size = pHandle + 0x88;
        }

        public FbxSkeleton(IntPtr InHandle)
            : base(InHandle)
        {
            size = pHandle + 0x88;
        }

        public FbxSkeleton(FbxObject Object, string pName)
        {
            pHandle = CreateFromObject(Object.Handle, pName);
            size = pHandle + 0x88;
        }

        public void SetSkeletonType(EType skeletonType)
        {
            SetSkeletonTypeInternal(pHandle, skeletonType);
        }
    }

    internal class FbxNodeAttribute : FbxObject
    {
        public enum EType
        {
            eUnknown,
            eNull,
            eMarker,
            eSkeleton,
            eMesh,
            eNurbs,
            ePatch,
            eCamera,
            eCameraStereo,
            eCameraSwitcher,
            eLight,
            eOpticalReference,
            eOpticalMarker,
            eNurbsCurve,
            eTrimNurbsSurface,
            eBoundary,
            eNurbsSurface,
            eShape,
            eLODGroup,
            eSubDiv,
            eCachedEffect,
            eLine
        };

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetNodeCount@FbxNodeAttribute@fbxsdk@@QEBAHXZ")]
        private static extern int GetNodeCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetNode@FbxNodeAttribute@fbxsdk@@QEBAPEAVFbxNode@2@H@Z")]
        private static extern IntPtr GetNodeInternal(IntPtr handle, int pIndex);

        private delegate EType GetAttributeTypeDelegate(IntPtr handle);
        private GetAttributeTypeDelegate GetAttributeTypeInternal;
        private IntPtr color;

        public EType AttributeType => GetAttributeTypeInternal(pHandle);
        public int NodeCount => GetNodeCountInternal(pHandle);

        public SharpDX.Color4 Color
        {
            get
            {
                SharpDX.Vector3 v = FbxProperty.GetDouble3(color);
                return new SharpDX.Color4(v.X, v.Y, v.Z, 1.0f);
            }
        }

        public FbxNodeAttribute() { }
        public FbxNodeAttribute(IntPtr InHandle)
            : base(InHandle)
        {
            GetAttributeTypeInternal = Marshal.GetDelegateForFunctionPointer<GetAttributeTypeDelegate>(Marshal.ReadIntPtr(vTable + 0xB8));
            color = pHandle + 0x78;
        }

        public FbxNode GetNode(int index)
        {
            if (index >= NodeCount)
            {
                return null;
            }

            IntPtr ptr = GetNodeInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxNode(ptr);
        }
    }

    internal class FbxLayerContainer : FbxNodeAttribute
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetLayer@FbxLayerContainer@fbxsdk@@QEBAPEBVFbxLayer@2@H@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetLayerInternal(IntPtr InHandle, int pIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?CreateLayer@FbxLayerContainer@fbxsdk@@QEAAHXZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern int CreateLayerInternal(IntPtr InHandle);

        public FbxLayerContainer() { }
        public FbxLayerContainer(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public FbxLayer GetLayer(int pIndex)
        {
            IntPtr Ptr = GetLayerInternal(pHandle, pIndex);
            return Ptr == IntPtr.Zero ? null : new FbxLayer(Ptr);
        }

        public int CreateLayer()
        {
            return CreateLayerInternal(pHandle);
        }
    }

    internal class FbxGeometryBase : FbxLayerContainer
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?InitControlPoints@FbxGeometryBase@fbxsdk@@UEAAXH@Z")]
        private static extern void InitControlPointsInternal(IntPtr InHandle, int pCount);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetControlPoints@FbxGeometryBase@fbxsdk@@UEBAPEAVFbxVector4@2@PEAVFbxStatus@2@@Z")]
        private static extern IntPtr GetControlPointsInternal(IntPtr InHandle, IntPtr pStatus);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetControlPointsCount@FbxGeometryBase@fbxsdk@@UEBAHXZ")]
        private static extern int GetControlsPointsCount(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementNormalCount@FbxGeometryBase@fbxsdk@@QEBAHXZ")]
        private static extern int GetElementNormalCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementTangentCount@FbxGeometryBase@fbxsdk@@QEBAHXZ")]
        private static extern int GetElementTangentCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementBinormalCount@FbxGeometryBase@fbxsdk@@QEBAHXZ")]
        private static extern int GetElementBinormalCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementUVCount@FbxGeometryBase@fbxsdk@@QEBAHW4EType@FbxLayerElement@2@@Z")]
        private static extern int GetElementUVCountInternal(IntPtr handle, FbxLayerElement.EType type);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementVertexColorCount@FbxGeometryBase@fbxsdk@@QEBAHXZ")]
        private static extern int GetElementVertexColorCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementNormal@FbxGeometryBase@fbxsdk@@QEAAPEAVFbxLayerElementNormal@2@H@Z")]
        private static extern IntPtr GetElementNormalInternal(IntPtr handle, int pIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementTangent@FbxGeometryBase@fbxsdk@@QEAAPEAVFbxLayerElementTangent@2@H@Z")]
        private static extern IntPtr GetElementTangentInternal(IntPtr handle, int pIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementBinormal@FbxGeometryBase@fbxsdk@@QEAAPEAVFbxLayerElementBinormal@2@H@Z")]
        private static extern IntPtr GetElementBinormalInternal(IntPtr handle, int pIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementUV@FbxGeometryBase@fbxsdk@@QEAAPEAVFbxLayerElementUV@2@HW4EType@FbxLayerElement@2@@Z")]
        private static extern IntPtr GetElementUVInternal(IntPtr handle, int pIndex, FbxLayerElement.EType pType);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetElementVertexColor@FbxGeometryBase@fbxsdk@@QEAAPEAVFbxLayerElementVertexColor@2@H@Z")]
        private static extern IntPtr GetElementVertexColorInternal(IntPtr handle, int pIndex);

        public int ControlPointsCount => GetControlsPointsCount(pHandle);
        public int ElementNormalCount => GetElementNormalCountInternal(pHandle);
        public int ElementTangentCount => GetElementTangentCountInternal(pHandle);
        public int ElementBinormalCount => GetElementBinormalCountInternal(pHandle);
        public int ElementUVCount => GetElementUVCountInternal(pHandle, FbxLayerElement.EType.eUnknown);
        public int ElementVertexColorCount => GetElementVertexColorCountInternal(pHandle);

        public FbxGeometryBase() { }
        public FbxGeometryBase(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public void InitControlPoints(int pCount)
        {
            InitControlPointsInternal(pHandle, pCount);
        }

        public IntPtr GetControlPoints()
        {
            IntPtr Ptr = GetControlPointsInternal(pHandle, IntPtr.Zero);
            return Ptr;
        }

        public FbxLayerElementTangent GetElementTangent(int index)
        {
            IntPtr ptr = GetElementTangentInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxLayerElementTangent(ptr);
        }

        public FbxLayerElementBinormal GetElementBinormal(int index)
        {
            IntPtr ptr = GetElementBinormalInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxLayerElementBinormal(ptr);
        }

        public FbxLayerElementNormal GetElementNormal(int index)
        {
            IntPtr ptr = GetElementNormalInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxLayerElementNormal(ptr);
        }

        public FbxLayerElementUV GetElementUV(int index, FbxLayerElement.EType type)
        {
            IntPtr ptr = GetElementUVInternal(pHandle, index, type);
            return ptr == IntPtr.Zero ? null : new FbxLayerElementUV(ptr);
        }

        public FbxLayerElementUV GetElementUV(string name)
        {
            for (int i = 0; i < ElementUVCount; i++)
            {
                FbxLayerElementUV layer = GetElementUV(i, FbxLayerElement.EType.eUnknown);
                if (layer.Name == name)
                {
                    return layer;
                }
            }
            return null;
        }

        public FbxLayerElementVertexColor GetElementVertexColor(int index)
        {
            IntPtr ptr = GetElementVertexColorInternal(pHandle, index);
            return ptr == IntPtr.Zero ? null : new FbxLayerElementVertexColor(ptr);
        }

        public FbxLayerElementVertexColor GetElementVertexColor(string name)
        {
            for (int i = 0; i < ElementVertexColorCount; i++)
            {
                FbxLayerElementVertexColor layer = GetElementVertexColor(i);
                if (layer.Name == name)
                {
                    return layer;
                }
            }
            return null;
        }
    }

    internal class FbxGeometry : FbxGeometryBase
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?AddDeformer@FbxGeometry@fbxsdk@@QEAAHPEAVFbxDeformer@2@@Z")]
        private static extern int AddDeformerInternal(IntPtr pHandle, IntPtr pDeformer);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDeformerCount@FbxGeometry@fbxsdk@@QEBAHW4EDeformerType@FbxDeformer@2@@Z")]
        private static extern int GetDeformerCountInternal(IntPtr pHandle, FbxDeformer.EDeformerType pType);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDeformer@FbxGeometry@fbxsdk@@QEBAPEAVFbxDeformer@2@HW4EDeformerType@32@PEAVFbxStatus@2@@Z")]
        private static extern IntPtr GetDeformerInternal(IntPtr pHandle, int pIndex, FbxDeformer.EDeformerType pType, IntPtr pStatus);

        public FbxGeometry(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public int AddDeformer(FbxDeformer deformer)
        {
            return AddDeformerInternal(pHandle, deformer.Handle);
        }

        public int GetDeformerCount(FbxDeformer.EDeformerType type)
        {
            return GetDeformerCountInternal(pHandle, type);
        }

        public FbxDeformer GetDeformer(int index, FbxDeformer.EDeformerType type)
        {
            IntPtr ptr = GetDeformerInternal(pHandle, index, type, IntPtr.Zero);
            return ptr == IntPtr.Zero ? null : new FbxDeformer(ptr);
        }
    }

    internal class FbxGeometryConverter : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??0FbxGeometryConverter@fbxsdk@@QEAA@PEAVFbxManager@1@@Z")]
        private static extern void CreateFromManager(IntPtr handle, IntPtr manager);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??1FbxGeometryConverter@fbxsdk@@QEAA@XZ")]
        private static extern void DisposeInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?ComputeEdgeSmoothingFromNormals@FbxGeometryConverter@fbxsdk@@QEBA_NPEAVFbxMesh@2@@Z")]
        private static extern bool ComputeEdgeSmoothingFromNormalsInternal(IntPtr handle, IntPtr mesh);

        public FbxGeometryConverter(FbxManager mgr)
        {
            pHandle = FbxUtils.FbxMalloc(16);
            CreateFromManager(pHandle, mgr.Handle);
        }

        public bool ComputeEdgeSmoothingFromNormals(FbxMesh pMesh)
        {
            return ComputeEdgeSmoothingFromNormalsInternal(pHandle, pMesh.Handle);
        }

        public void Dispose()
        {
            DisposeInternal(pHandle);
            FbxUtils.FbxFree(pHandle);
            pHandle = IntPtr.Zero;
        }
    }

    internal class FbxMesh : FbxGeometry
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxMesh@fbxsdk@@SAPEAV12@PEAVFbxManager@2@PEBD@Z")]
        private static extern IntPtr CreateFromManager(IntPtr pManager, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxMesh@fbxsdk@@SAPEAV12@PEAVFbxObject@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pObject, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?BeginPolygon@FbxMesh@fbxsdk@@QEAAXHHH_N@Z")]
        private static extern void BeginPolygonInternal(IntPtr InHandle, int pMaterial, int pTexture, int pGroup, bool bLegacy);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?AddPolygon@FbxMesh@fbxsdk@@QEAAXHH@Z")]
        private static extern void AddPolygonInternal(IntPtr InHandle, int pIndex, int pTextureUVIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?EndPolygon@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void EndPolygonInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetPolygonCount@FbxMesh@fbxsdk@@QEBAHXZ")]
        private static extern int GetPolygonCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetPolygonSize@FbxMesh@fbxsdk@@QEBAHH@Z")]
        private static extern int GetPolygonSizeInternal(IntPtr handle, int pIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetPolygonVertex@FbxMesh@fbxsdk@@QEBAHHH@Z")]
        private static extern int GetPolygonIndexInternal(IntPtr handle, int pPolygonIndex, int pPositionInPolygon);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?IsTriangleMesh@FbxMesh@fbxsdk@@QEBA_NXZ")]
        private static extern bool IsTriangleMeshInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?RemoveBadPolygons@FbxMesh@fbxsdk@@QEAAHXZ")]
        private static extern int RemoveBadPolygonsInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetMeshEdgeCount@FbxMesh@fbxsdk@@QEBAHXZ")]
        private static extern int GetMeshEdgeCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?BuildMeshEdgeArray@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void BuildMeshEdgeArrayInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?BeginGetMeshEdgeVertices@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void BeginGetMeshEdgeVerticesInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?EndGetMeshEdgeVertices@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void EndGetMeshEdgeVerticesInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?EndGetMeshEdgeVertices@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void GetMeshEdgeVerticesInternal(IntPtr handle, int pEdgeIndex, ref int pStartVertexIndex, ref int pEndVertexIndex);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?BeginGetMeshEdgeIndexForPolygon@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void BeginGetMeshEdgeIndexForPolygonInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?EndGetMeshEdgeIndexForPolygon@FbxMesh@fbxsdk@@QEAAXXZ")]
        private static extern void EndGetMeshEdgeIndexForPolygonInternal(IntPtr handle);

        public int PolygonCount => GetPolygonCountInternal(pHandle);
        public int MeshEdgeCount => GetMeshEdgeCountInternal(pHandle);

        public FbxMesh(FbxManager mgr, string name)
            : base(CreateFromManager(mgr.Handle, name))
        {
        }

        public FbxMesh(FbxObject obj, string name)
            : base(CreateFromObject(obj.Handle, name))
        {
        }

        public FbxMesh(FbxNodeAttribute attr)
            : base(attr.Handle)
        {
        }

        public void BeginPolygon(int materialIndex = -1)
        {
            BeginPolygonInternal(pHandle, materialIndex, -1, -1, true);
        }

        public void AddPolygon(int index)
        {
            AddPolygonInternal(pHandle, index, -1);
        }

        public void EndPolygon()
        {
            EndPolygonInternal(pHandle);
        }

        public int GetPolygonSize(int index)
        {
            return GetPolygonSizeInternal(pHandle, index);
        }

        public int GetPolygonIndex(int index, int position)
        {
            return GetPolygonIndexInternal(pHandle, index, position);
        }

        public bool IsTriangleMesh()
        {
            return IsTriangleMeshInternal(pHandle);
        }

        public int RemoveBadPolygons()
        {
            return RemoveBadPolygonsInternal(pHandle);
        }

        public void BuildMeshEdgeArray()
        {
            BuildMeshEdgeArrayInternal(pHandle);
        }

        public void BeginGetMeshEdgeVertices()
        {
            BeginGetMeshEdgeVerticesInternal(pHandle);
        }

        public void EndGetMeshEdgeVertices()
        {
            EndGetMeshEdgeVerticesInternal(pHandle);
        }

        public void GetMeshEdgeVertices(int pEdgeIndex, ref int pStartVertexIndex, ref int pEndVertexIndex)
        {
            GetMeshEdgeVerticesInternal(pHandle, pEdgeIndex, ref pStartVertexIndex, ref pEndVertexIndex);
        }

        public void BeginGetMeshEdgeIndexForPolygon()
        {
            BeginGetMeshEdgeIndexForPolygonInternal(pHandle);
        }

        public void EndGetMeshEdgeIndexForPolygon()
        {
            EndGetMeshEdgeIndexForPolygonInternal(pHandle);
        }
    }

    internal enum EMappingMode
    {
        eNone,
        eByControlPoint,
        eByPolygonVertex,
        eByPolygon,
        eByEdge,
        eAllSame
    };

    internal enum EReferenceMode
    {
        eDirect,
        eIndex,
        eIndexToDirect
    };

    internal class FbxLayerElement : FbxNative
    {
        internal enum EType
        {
            eUnknown,
            eNormal,
            eBiNormal,
            eTangent,
            eMaterial,
            ePolygonGroup,
            eUV,
            eVertexColor,
            eSmoothing,
            eVertexCrease,
            eEdgeCrease,
            eHole,
            eUserData,
            eVisibility,
            eTextureDiffuse,
            eTextureDiffuseFactor,
            eTextureEmissive,
            eTextureEmissiveFactor,
            eTextureAmbient,
            eTextureAmbientFactor,
            eTextureSpecular,
            eTextureSpecularFactor,
            eTextureShininess,
            eTextureNormalMap,
            eTextureBump,
            eTextureTransparency,
            eTextureTransparencyFactor,
            eTextureReflection,
            eTextureReflectionFactor,
            eTextureDisplacement,
            eTextureDisplacementVector,
            eTypeCount
        };

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetMappingMode@FbxLayerElement@fbxsdk@@QEAAXW4EMappingMode@12@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetMappingModeInternal(IntPtr InHandle, EMappingMode pMappingMode);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?SetReferenceMode@FbxLayerElement@fbxsdk@@QEAAXW4EReferenceMode@12@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern void SetReferenceModeInternal(IntPtr InHandle, EReferenceMode pReferenceMode);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetMappingMode@FbxLayerElement@fbxsdk@@QEBA?AW4EMappingMode@12@XZ")]
        private static extern EMappingMode GetMappingModeInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetReferenceMode@FbxLayerElement@fbxsdk@@QEBA?AW4EReferenceMode@12@XZ")]
        private static extern EReferenceMode GetReferenceModeInternal(IntPtr handle);

        public EMappingMode MappingMode { get => GetMappingModeInternal(pHandle); set => SetMappingModeInternal(pHandle, value); }
        public EReferenceMode ReferenceMode { get => GetReferenceModeInternal(pHandle); set => SetReferenceModeInternal(pHandle, value); }
        public string Name { get => FbxString.Get(mName); set => mName = FbxString.Construct(value); }

        private IntPtr mName;

        private FbxLayerElement()
        {
        }

        public FbxLayerElement(IntPtr handle)
            : this()
        {
            pHandle = handle;
            mName = pHandle + 0x10;
        }
    }

    internal class FbxLayerElementNormal : FbxLayerElement
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxLayerElementNormal@fbxsdk@@SAPEAV12@PEAVFbxLayerContainer@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pOwner, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDirectArray@?$FbxLayerElementTemplate@VFbxVector4@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@VFbxVector4@fbxsdk@@@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetDirectArrayInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIndexArray@?$FbxLayerElementTemplate@VFbxVector4@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@H@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetIndexArrayInternal(IntPtr InHandle);

        public FbxLayerElementNormal(IntPtr handle)
            : base(handle)
        {
        }

        public FbxLayerElementNormal(FbxLayerContainer pOwner, string pName)
            : this(CreateFromObject(pOwner.Handle, pName))
        {
        }

        public FbxLayerElementArray DirectArray
        {
            get
            {
                IntPtr Ptr = GetDirectArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }

        public FbxLayerElementArray IndexArray
        {
            get
            {
                IntPtr Ptr = GetIndexArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }
    }

    internal class FbxLayerElementTangent : FbxLayerElement
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxLayerElementTangent@fbxsdk@@SAPEAV12@PEAVFbxLayerContainer@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pOwner, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDirectArray@?$FbxLayerElementTemplate@VFbxVector4@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@VFbxVector4@fbxsdk@@@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetDirectArrayInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIndexArray@?$FbxLayerElementTemplate@VFbxVector4@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@H@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetIndexArrayInternal(IntPtr InHandle);

        public FbxLayerElementTangent(IntPtr handle)
            : base(handle)
        {
        }

        public FbxLayerElementTangent(FbxLayerContainer pOwner, string pName)
            : this(CreateFromObject(pOwner.Handle, pName))
        {
        }

        public FbxLayerElementArray DirectArray
        {
            get
            {
                IntPtr Ptr = GetDirectArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }

        public FbxLayerElementArray IndexArray
        {
            get
            {
                IntPtr Ptr = GetIndexArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }
    }

    internal class FbxLayerElementBinormal : FbxLayerElement
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxLayerElementBinormal@fbxsdk@@SAPEAV12@PEAVFbxLayerContainer@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pOwner, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDirectArray@?$FbxLayerElementTemplate@VFbxVector4@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@VFbxVector4@fbxsdk@@@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetDirectArrayInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIndexArray@?$FbxLayerElementTemplate@VFbxVector4@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@H@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetIndexArrayInternal(IntPtr InHandle);

        public FbxLayerElementBinormal(IntPtr handle)
            : base(handle)
        {
        }

        public FbxLayerElementBinormal(FbxLayerContainer pOwner, string pName)
            : this(CreateFromObject(pOwner.Handle, pName))
        {
        }

        public FbxLayerElementArray DirectArray
        {
            get
            {
                IntPtr Ptr = GetDirectArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }

        public FbxLayerElementArray IndexArray
        {
            get
            {
                IntPtr Ptr = GetIndexArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }
    }

    internal class FbxLayerElementUV : FbxLayerElement
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxLayerElementUV@fbxsdk@@SAPEAV12@PEAVFbxLayerContainer@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pOwner, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDirectArray@?$FbxLayerElementTemplate@VFbxVector2@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@VFbxVector2@fbxsdk@@@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetDirectArrayInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIndexArray@?$FbxLayerElementTemplate@VFbxVector2@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@H@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetIndexArrayInternal(IntPtr InHandle);

        public FbxLayerElementUV(IntPtr handle)
            : base(handle)
        {
        }

        public FbxLayerElementUV(FbxLayerContainer pOwner, string pName)
            : this(CreateFromObject(pOwner.Handle, pName))
        {
        }

        public FbxLayerElementArray DirectArray
        {
            get
            {
                IntPtr Ptr = GetDirectArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }

        public FbxLayerElementArray IndexArray
        {
            get
            {
                IntPtr Ptr = GetIndexArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }
    }

    internal class FbxLayerElementVertexColor : FbxLayerElement
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxLayerElementVertexColor@fbxsdk@@SAPEAV12@PEAVFbxLayerContainer@2@PEBD@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pOwner, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDirectArray@?$FbxLayerElementTemplate@VFbxColor@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@VFbxColor@fbxsdk@@@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetDirectArrayInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIndexArray@?$FbxLayerElementTemplate@VFbxColor@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@H@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetIndexArrayInternal(IntPtr InHandle);

        public FbxLayerElementVertexColor(IntPtr handle)
            : base(handle)
        {
        }

        public FbxLayerElementVertexColor(FbxLayerContainer pOwner, string pName)
            : this(CreateFromObject(pOwner.Handle, pName))
        {
        }

        public FbxLayerElementArray DirectArray
        {
            get
            {
                IntPtr Ptr = GetDirectArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }

        public FbxLayerElementArray IndexArray
        {
            get
            {
                IntPtr Ptr = GetIndexArrayInternal(pHandle);
                return Ptr == IntPtr.Zero ? null : new FbxLayerElementArray(Ptr);
            }
        }
    }

    internal class FbxLayerElementUserData : FbxLayerElement
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Create@FbxLayerElementUserData@fbxsdk@@SAPEAV12@PEAVFbxLayerContainer@2@AEBV12@@Z")]
        private static extern IntPtr CreateFromObject(IntPtr pOwner, [MarshalAs(UnmanagedType.LPStr)] string pName);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetDirectArray@?$FbxLayerElementTemplate@VFbxColor@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@VFbxColor@fbxsdk@@@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetDirectArrayInternal(IntPtr InHandle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetIndexArray@?$FbxLayerElementTemplate@VFbxColor@fbxsdk@@@fbxsdk@@QEAAAEAV?$FbxLayerElementArrayTemplate@H@2@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr GetIndexArrayInternal(IntPtr InHandle);

        public FbxLayerElementUserData(FbxLayerContainer pOwner, string pName)
            : base(CreateFromObject(pOwner.Handle, pName))
        {
        }

        public FbxLayerElementArray DirectArray
        {
            get
            {
                IntPtr Ptr = GetDirectArrayInternal(pHandle);
                if (Ptr == IntPtr.Zero)
                {
                    return null;
                }

                return new FbxLayerElementArray(Ptr);
            }
        }

        public FbxLayerElementArray IndexArray
        {
            get
            {
                IntPtr Ptr = GetIndexArrayInternal(pHandle);
                if (Ptr == IntPtr.Zero)
                {
                    return null;
                }

                return new FbxLayerElementArray(Ptr);
            }
        }
    }

    internal class FbxLayerElementArray : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Add@FbxLayerElementArray@fbxsdk@@QEAAHPEBXW4EFbxType@2@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern int AddInternal(IntPtr InHandle, IntPtr pItem, EFbxType pValueType);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetCount@FbxLayerElementArray@fbxsdk@@QEBAHXZ")]
        private static extern int GetCountInternal(IntPtr handle);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?GetAt@FbxLayerElementArray@fbxsdk@@QEBA_NHPEAPEAXW4EFbxType@2@@Z")]
        private static extern bool GetAtInternal(IntPtr handle, int pIndex, IntPtr pItem, EFbxType pValueType);

        public int Count => GetCountInternal(pHandle);

        public FbxLayerElementArray(IntPtr InHandle)
            : base(InHandle)
        {
        }

        public int Add(double x, double y, double z, double w = 0.0)
        {
            IntPtr ptr = FbxUtils.FbxMalloc(32);

            Marshal.WriteInt64(ptr, 0, BitConverter.ToInt64(BitConverter.GetBytes(x), 0));
            Marshal.WriteInt64(ptr, 8, BitConverter.ToInt64(BitConverter.GetBytes(y), 0));
            Marshal.WriteInt64(ptr, 16, BitConverter.ToInt64(BitConverter.GetBytes(z), 0));
            Marshal.WriteInt64(ptr, 24, BitConverter.ToInt64(BitConverter.GetBytes(w), 0));

            int idx = AddInternal(pHandle, ptr, EFbxType.eFbxDouble4);
            FbxUtils.FbxFree(ptr);

            return idx;
        }

        public int Add(double x, double y)
        {
            IntPtr ptr = FbxUtils.FbxMalloc(16);

            Marshal.WriteInt64(ptr, 0, BitConverter.ToInt64(BitConverter.GetBytes(x), 0));
            Marshal.WriteInt64(ptr, 8, BitConverter.ToInt64(BitConverter.GetBytes(y), 0));

            int idx = AddInternal(pHandle, ptr, EFbxType.eFbxDouble2);
            FbxUtils.FbxFree(ptr);

            return idx;
        }

        public int Add(int a)
        {
            IntPtr ptr = FbxUtils.FbxMalloc(4);
            Marshal.WriteInt32(ptr, 0, a);

            int idx = AddInternal(pHandle, ptr, EFbxType.eFbxInt);
            FbxUtils.FbxFree(ptr);

            return idx;
        }

        public void GetAt(int index, out SharpDX.Vector4 outValue)
        {
            outValue = new SharpDX.Vector4();
            IntPtr ptr = GetAt(index, EFbxType.eFbxDouble4);

            outValue.X = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 0));
            outValue.Y = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 8));
            outValue.Z = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 16));
            outValue.W = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 24));
            FbxUtils.FbxFree(ptr);
        }

        public void GetAt(int index, out SharpDX.Vector3 outValue)
        {
            outValue = new SharpDX.Vector3();
            IntPtr ptr = GetAt(index, EFbxType.eFbxDouble3);

            outValue.X = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 0));
            outValue.Y = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 8));
            outValue.Z = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 16));
            FbxUtils.FbxFree(ptr);
        }

        public void GetAt(int index, out SharpDX.Vector2 outValue)
        {
            outValue = new SharpDX.Vector2();
            IntPtr ptr = GetAt(index, EFbxType.eFbxDouble2);

            outValue.X = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 0));
            outValue.Y = (float)BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 8));
            FbxUtils.FbxFree(ptr);
        }

        public void GetAt(int index, out SharpDX.ColorBGRA outValue)
        {
            outValue = new SharpDX.ColorBGRA();
            IntPtr ptr = GetAt(index, EFbxType.eFbxDouble4);

            outValue.R = (byte)(BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 0)) * 255.0);
            outValue.G = (byte)(BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 8)) * 255.0);
            outValue.B = (byte)(BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 16)) * 255.0);
            outValue.A = (byte)(BitConverter.Int64BitsToDouble(Marshal.ReadInt64(ptr, 24)) * 255.0);
            FbxUtils.FbxFree(ptr);
        }

        public void GetAt(int index, out int outValue)
        {
            IntPtr ptr = GetAt(index, EFbxType.eFbxInt);
            outValue = Marshal.ReadInt32(ptr);
            FbxUtils.FbxFree(ptr);
        }

        private unsafe IntPtr GetAt(int index, EFbxType type)
        {
            ulong sizeToAlloc = 0;
            switch(type)
            {
                case EFbxType.eFbxDouble4: sizeToAlloc = 8 * 4; break;
                case EFbxType.eFbxDouble3: sizeToAlloc = 8 * 3; break;
                case EFbxType.eFbxDouble2: sizeToAlloc = 8 * 2; break;
                case EFbxType.eFbxInt: sizeToAlloc = 4; break;
            }

            IntPtr ptr = FbxUtils.FbxMalloc(sizeToAlloc);
            IntPtr ptrPtr = new IntPtr((void*)&ptr);
            GetAtInternal(pHandle, index, ptrPtr, type);
            ptrPtr = IntPtr.Zero;

            return ptr;
        }
    }

    internal class FbxString
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??0FbxString@fbxsdk@@QEAA@PEBD@Z")]
        private static extern void ConstructInternal(IntPtr Handle, [MarshalAs(UnmanagedType.LPStr)] string pParam);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??4FbxString@fbxsdk@@QEAAAEBV01@PEBD@Z")]
        private static extern void AssignInternal(IntPtr Handle, [MarshalAs(UnmanagedType.LPStr)] string pParam);

        public static IntPtr Construct(string InitialValue = "")
        {
            IntPtr Ptr = FbxUtils.FbxMalloc(8);
            ConstructInternal(Ptr, InitialValue);

            return Ptr;
        }

        public static void Assign(IntPtr InHandle, string pParam)
        {
            AssignInternal(InHandle, pParam);
        }

        public static unsafe string Get(IntPtr InHandle)
        {
            IntPtr Ptr = new IntPtr(*(long*)InHandle);
            return (Ptr != IntPtr.Zero) ? FbxUtils.IntPtrToString(Ptr) : "";
        }
    }

    internal class FbxDouble3
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??0?$FbxVectorTemplate3@N@fbxsdk@@QEAA@NNN@Z")]
        private static extern void ConstructInternal(IntPtr handle, double pValue1, double pValue2, double pValue3);

        public static IntPtr Construct(SharpDX.Vector3 value)
        {
            IntPtr ptr = FbxUtils.FbxMalloc(8 * 3);
            ConstructInternal(ptr, value.X, value.Y, value.Z);

            return ptr;
        }

        public static unsafe SharpDX.Vector3 Get(IntPtr inHandle)
        {
            float x = (float)*((double*)(inHandle.ToInt64()));
            float y = (float)*((double*)(inHandle.ToInt64() + 8));
            float z = (float)*((double*)(inHandle.ToInt64() + 16));

            return new SharpDX.Vector3(x, y, z);
        }
    }

    internal class FbxProperty
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Set@FbxProperty@fbxsdk@@IEAA_NPEBXAEBW4EFbxType@2@_N@Z")]
        private static extern void SetInternal(IntPtr InHandle, IntPtr pValue, ref EFbxType pValueType, bool pCheckForValueEquality);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Get@FbxProperty@fbxsdk@@IEBA_NPEAXAEBW4EFbxType@2@@Z")]
        private static extern bool GetInternal(IntPtr InHandle, ref IntPtr pValue, ref EFbxType pValueType);

        public static void Set(IntPtr inHandle, string value)
        {
            EFbxType type = EFbxType.eFbxString;
            IntPtr ptr = FbxString.Construct(value);

            SetInternal(inHandle, ptr, ref type, true);
            FbxUtils.FbxFree(ptr);
        }

        public static void Set(IntPtr inHandle, SharpDX.Vector3 value)
        {
            EFbxType type = EFbxType.eFbxDouble3;
            IntPtr ptr = FbxDouble3.Construct(value);

            SetInternal(inHandle, ptr, ref type, true);
            FbxUtils.FbxFree(ptr);
        }

        public static unsafe void Set(IntPtr inHandle, double value)
        {
            EFbxType type = EFbxType.eFbxDouble;

            IntPtr ptr = FbxUtils.FbxMalloc(8);
            Marshal.WriteInt64(ptr, *((long*)&value));

            SetInternal(inHandle, ptr, ref type, true);
            FbxUtils.FbxFree(ptr);
        }

        public static string GetString(IntPtr inHandle)
        {
            EFbxType type = EFbxType.eFbxString;
            IntPtr ptr = IntPtr.Zero;

            GetInternal(inHandle, ref ptr, ref type);
            return FbxUtils.IntPtrToString(ptr);
        }

        public static unsafe SharpDX.Vector3 GetDouble3(IntPtr inHandle)
        {
            EFbxType type = EFbxType.eFbxDouble3;
            IntPtr ptr = IntPtr.Zero;

            GetInternal(inHandle, ref ptr, ref type);
            return FbxDouble3.Get(new IntPtr((long*)&ptr));
        }

        public static unsafe double GetDouble(IntPtr inHandle)
        {
            EFbxType type = EFbxType.eFbxDouble;
            IntPtr ptr = IntPtr.Zero;

            GetInternal(inHandle, ref ptr, ref type);
            return *((double*)&ptr);
        }
    }

    internal class FbxTime : FbxNative
    {
        public static FbxTime FBXSDK_TIME_INFINITE = new FbxTime(0x7fffffffffffffff);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??0FbxTime@fbxsdk@@QEAA@_J@Z")]
        private static extern void ConstructInternal(IntPtr inHandle, long pTime);

        public FbxTime(long time)
        {
            pHandle = FbxUtils.FbxMalloc(8);
            ConstructInternal(pHandle, time);
            bNeedsFreeing = true;
        }
    }

    internal class FbxAMatrix : FbxNative
    {
        public FbxAMatrix(IntPtr ptr)
            : base(ptr)
        {
        }
    }

    internal class FbxMatrix : FbxNative
    {
        [DllImport("thirdparty/libfbxsdk", EntryPoint = "??0FbxMatrix@fbxsdk@@QEAA@AEBVFbxAMatrix@1@@Z")]
        private static extern void ConvertFromAffineInternal(IntPtr pHandle, IntPtr pMatrix);

        [DllImport("thirdparty/libfbxsdk", EntryPoint = "?Get@FbxMatrix@fbxsdk@@QEBANHH@Z")]
        private static extern double GetInternal(IntPtr pHandle, int row, int column);

        public FbxMatrix(IntPtr ptr)
            : base(ptr)
        {
        }

        public FbxMatrix(FbxAMatrix affineMatrix)
        {
            pHandle = FbxUtils.FbxMalloc(0x80);
            ConvertFromAffineInternal(pHandle, affineMatrix.Handle);
            bNeedsFreeing = true;
        }

        public SharpDX.Matrix ToSharpDX()
        {
            return new SharpDX.Matrix(
                (float)GetInternal(pHandle, 0, 0), (float)GetInternal(pHandle, 0, 1), (float)GetInternal(pHandle, 0, 2), (float)GetInternal(pHandle, 0, 3),
                (float)GetInternal(pHandle, 1, 0), (float)GetInternal(pHandle, 1, 1), (float)GetInternal(pHandle, 1, 2), (float)GetInternal(pHandle, 1, 3),
                (float)GetInternal(pHandle, 2, 0), (float)GetInternal(pHandle, 2, 1), (float)GetInternal(pHandle, 2, 2), (float)GetInternal(pHandle, 2, 3),
                (float)GetInternal(pHandle, 3, 0), (float)GetInternal(pHandle, 3, 1), (float)GetInternal(pHandle, 3, 2), (float)GetInternal(pHandle, 3, 3)
                );
        }
    }
}
