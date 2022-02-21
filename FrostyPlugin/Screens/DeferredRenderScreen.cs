using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using Frosty.Core.Viewport;
using FrostySdk;
using System.IO;
using FrostySdk.Managers;
using FrostySdk.IO;
using FrostySdk.Resources;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Frosty.Hash;
using DXUT = Frosty.Core.Viewport.DXUT;

namespace Frosty.Core.Screens
{
    public class InteropUtils
    {
        public static IntPtr CreateStructure<T>(T structure)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            Marshal.StructureToPtr<T>(structure, ptr, true);
            return ptr;
        }

        public static IntPtr AddressOf(IntPtr ptr)
        {
            IntPtr newPtr = Marshal.AllocHGlobal(8);
            Marshal.WriteInt64(newPtr, ptr.ToInt64());
            return newPtr;
        }

        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
    }

    public class GFSDK_ShadowLib
    {
        public enum ViewType
        {
            Single = 1,
            Cascades_2 = 2,
            Cascades_3 = 3,
            Cascades_4 = 4
        }

        public enum TechniqueType
        {
            Hard,
            PCF,
            PCSS,
            RT,
            HRTS,
            FT,
            HFTS,
            Max
        }

        public enum CascadedShadowMapType
        {
            UserDefined,
            SampleDistribution,
            Max
        }

        public enum CullModeType
        {
            Front,
            Back,
            None,
            Max
        }

        public enum DepthType
        {
            DepthBuffer,
            EyeViewZ,
            Max
        }

        public enum LightType
        {
            Directional,
            Spot,
            Max
        }

        public enum ConservativeRasterType
        {
            HW,
            SW,
            None,
            Max
        }

        public enum MapRenderType
        {
            Depth,
            RT,
            FT,
            Max
        }

        public enum MSAARenderMode
        {
            BruteForce,
            ComplexPixelMask,
            Max
        }

        public enum DebugViewType
        {
            None,
            Cascades,
            EyeDepth,
            EyeViewZ,
            FrustumTraceNodeList,
            Max
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Matrix
        {
            public float M11, M12, M13, M14;
            public float M21, M22, M23, M24;
            public float M31, M32, M33, M34;
            public float M41, M42, M43, M44;

            public static Matrix FromSharpDX(SharpDX.Matrix m)
            {
                Matrix mat = new Matrix
                {
                    M11 = m.M11,
                    M12 = m.M12,
                    M13 = m.M13,
                    M14 = m.M14,
                    M21 = m.M21,
                    M22 = m.M22,
                    M23 = m.M23,
                    M24 = m.M24,
                    M31 = m.M31,
                    M32 = m.M32,
                    M33 = m.M33,
                    M34 = m.M34,
                    M41 = m.M41,
                    M42 = m.M42,
                    M43 = m.M43,
                    M44 = m.M44
                };
                return mat;
            }

            public SharpDX.Matrix ToSharpDX()
            {
                SharpDX.Matrix m = new SharpDX.Matrix(M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
                return m;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Version
        {
            public uint uMajor;
            public uint uMinor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceContext
        {
            public IntPtr pD3DDevice;
            public IntPtr pDeviceContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RayTraceMapDesc
        {
            public byte bRequirePrimitiveMap;
            public uint uResolutionWidth;
            public uint uResolutionHeight;
            public uint uMaxNumberOfPrimitives;
            public uint uMaxNumberOfPrimitivesPerPixel;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FrustumTraceMapDesc
        {
            public byte bRequireFrustumTraceMap;
            public uint uResolutionWidth;
            public uint uResolutionHeight;
            public uint uDynamicReprojectionCascades;
            public uint uQuantizedListLengthTexelDimension;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MapDesc
        {
            public uint uResolutionWidth;
            public uint uResolutionHeight;
            public ViewType eViewType;
            public RayTraceMapDesc RayTraceMapDesc;
            public FrustumTraceMapDesc FrustumTraceMapDesc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BufferDesc
        {
            public uint uResolutionWidth;
            public uint uResolutionHeight;
            public uint uViewportTop;
            public uint uViewportLeft;
            public uint uViewportBottom;
            public uint uViewportRight;
            public uint uSampleCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DepthBufferDesc
        {
            public DepthType eDepthType;
            public IntPtr DepthSRV;
            public IntPtr ResolvedDepthSRV;
            public IntPtr ReadOnlyDSV;
            public uint uComplexRefValue;
            public uint uSimpleRefValue;

            public DepthBufferDesc(bool dummy = false)
            {
                eDepthType = DepthType.DepthBuffer;
                DepthSRV = IntPtr.Zero;
                ResolvedDepthSRV = IntPtr.Zero;
                ReadOnlyDSV = IntPtr.Zero;
                uComplexRefValue = 0x01;
                uSimpleRefValue = 0x00;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PCSSPenumbraParams
        {
            public float fMaxThreshold;
            public float fMaxClamp;
            public float fMinSizePercent_1;
            public float fMinSizePercent_2;
            public float fMinSizePercent_3;
            public float fMinSizePercent_4;
            public float fMinWeightThresholdPercent;
            public float fShiftMin;
            public float fShiftMax;
            public float fShiftMaxLerpThreshold;

            public PCSSPenumbraParams(bool dummy = false)
            {
                fMaxThreshold = 100.0f;
                fMaxClamp = 100.0f;
                fMinSizePercent_1 = 1.0f;
                fMinSizePercent_2 = 1.0f;
                fMinSizePercent_3 = 1.0f;
                fMinSizePercent_4 = 1.0f;
                fMinWeightThresholdPercent = 10.0f;
                fShiftMin = 2.0f;
                fShiftMax = 1.0f;
                fShiftMaxLerpThreshold = 0.01f;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LightDesc
        {
            public LightType eLightType;
            public Vector3 v3LightPos_1;
            public Vector3 v3LightPos_2;
            public Vector3 v3LightPos_3;
            public Vector3 v3LightPos_4;
            public Vector3 v3LightLookAt_1;
            public Vector3 v3LightLookAt_2;
            public Vector3 v3LightLookAt_3;
            public Vector3 v3LightLookAt_4;
            public float fLightSize;

            public LightDesc(bool dummy = false)
            {
                eLightType = LightType.Directional;
                v3LightPos_1 = Vector3.One;
                v3LightPos_2 = Vector3.One;
                v3LightPos_3 = Vector3.One;
                v3LightPos_4 = Vector3.One;
                v3LightLookAt_1 = Vector3.Zero;
                v3LightLookAt_2 = Vector3.Zero;
                v3LightLookAt_3 = Vector3.Zero;
                v3LightLookAt_4 = Vector3.Zero;
                fLightSize = 1.0f;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ZBiasParams
        {
            public int iDepthBias;
            public float fSlopeScaledDepthBias;
            public float fDistanceBiasMin;
            public float fDistanceBiasFactor;
            public float fDistanceBiasThreshold;
            public float fDistanceBiasPower;
            public byte bUseReceiverPlaneBias;

            public ZBiasParams(bool dummy = false)
            {
                iDepthBias = 1000;
                fSlopeScaledDepthBias = 8.0f;
                fDistanceBiasMin = 0.0000001f;
                fDistanceBiasFactor = 0.0000001f;
                fDistanceBiasThreshold = 1000.0f;
                fDistanceBiasPower = 3.0f;
                bUseReceiverPlaneBias = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RayTraceMapRenderParams
        {
            public ConservativeRasterType eConservativeRasterType;
            public CullModeType eCullModeType;
            public float fHitEpsilon;

            public RayTraceMapRenderParams(bool dummy = false)
            {
                eConservativeRasterType = ConservativeRasterType.HW;
                eCullModeType = CullModeType.Back;
                fHitEpsilon = 0.00001f;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FrustumTraceMapRenderParams
        {
            public ConservativeRasterType eConservativeRasterType;
            public CullModeType eCullModeType;
            public float fHitEpsilon;
            public byte bUseDynamicReprojection;
            public uint uListLengthTolerance;
            public uint uMaxPrimitiveCount;

            public FrustumTraceMapRenderParams(bool dummy = false)
            {
                eConservativeRasterType = ConservativeRasterType.HW;
                eCullModeType = CullModeType.None;
                fHitEpsilon = 0.001f;
                bUseDynamicReprojection = 1;
                uListLengthTolerance = 1536;
                uMaxPrimitiveCount = 1000000;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Frustum
        {
            public float fLeft;
            public float fRight;
            public float fTop;
            public float fBottom;
            public float fNear;
            public float fFar;
            public byte bFullyShadowed;
            public byte bClear;
            public byte bValid;

            public Frustum(bool dummy = false)
            {
                fLeft = 0;
                fRight = 0;
                fTop = 0;
                fBottom = 0;
                fNear = 0;
                fFar = 0;
                bFullyShadowed = 0;
                bClear = 1;
                bValid = 1;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MapRenderParams
        {
            public DepthBufferDesc DepthBufferDesc;
            public PCSSPenumbraParams PCSSPenumbraParams;
            public TechniqueType eTechniqueType; /* PCSS */
            public Matrix m4x4EyeViewMatrix;
            public Matrix m4x4EyeProjectionMatrix;
            public Vector3 v3WorldSpaceBBox_1;
            public Vector3 v3WorldSpaceBBox_2;
            public LightDesc LightDesc;
            public float fCascadeZLinearScale_1; /* 0.20f */
            public float fCascadeZLinearScale_2; /* 0.40f */
            public float fCascadeZLinearScale_3; /* 0.60f */
            public float fCascadeZLinearScale_4; /* 1.00f */
            public ZBiasParams ZBiasParams;
            public RayTraceMapRenderParams RayTraceMapRenderParams;
            public FrustumTraceMapRenderParams FrustumTraceMapRenderParams;
            public CascadedShadowMapType eCascadedShadowMapType; /* SampleDistribution */
            public Frustum UserDefinedFrustum_1;
            public Frustum UserDefinedFrustum_2;
            public Frustum UserDefinedFrustum_3;
            public Frustum UserDefinedFrustum_4;
            public float fCascadeMaxDistancePercent;
            public CullModeType eCullModeType;

            public MapRenderParams(bool dummy = false)
            {
                DepthBufferDesc = new DepthBufferDesc(true);
                PCSSPenumbraParams = new PCSSPenumbraParams(true);
                eTechniqueType = TechniqueType.PCSS;
                m4x4EyeViewMatrix = new Matrix();
                m4x4EyeProjectionMatrix = new Matrix();
                v3WorldSpaceBBox_1 = Vector3.Zero;
                v3WorldSpaceBBox_2 = Vector3.Zero;
                LightDesc = new LightDesc(true);
                fCascadeZLinearScale_1 = 0.20f;
                fCascadeZLinearScale_2 = 0.40f;
                fCascadeZLinearScale_3 = 0.60f;
                fCascadeZLinearScale_4 = 1.00f;
                ZBiasParams = new ZBiasParams(true);
                RayTraceMapRenderParams = new RayTraceMapRenderParams(true);
                FrustumTraceMapRenderParams = new FrustumTraceMapRenderParams(true);
                eCascadedShadowMapType = CascadedShadowMapType.SampleDistribution;
                UserDefinedFrustum_1 = new Frustum(true);
                UserDefinedFrustum_2 = new Frustum(true);
                UserDefinedFrustum_3 = new Frustum(true);
                UserDefinedFrustum_4 = new Frustum(true);
                fCascadeMaxDistancePercent = 100.0f;
                eCullModeType = CullModeType.None;
            }
        }

        public struct BufferRenderParams
        {
            public MSAARenderMode eMSAARenderMode;
            public float fCascadeBorderPercent;
            public float fCascadeBlendPercent;
            public DebugViewType eDebugViewType;

            public BufferRenderParams(bool dummy = false)
            {
                eMSAARenderMode = MSAARenderMode.BruteForce;
                fCascadeBorderPercent = 1.0f;
                fCascadeBlendPercent = 2.0f;
                eDebugViewType = DebugViewType.None;
            }
        }

        public struct Map
        {
            public IntPtr ptr;
        }

        public struct Buffer
        {
            public IntPtr ptr;
        }

        public class Context
        {
            private IntPtr nativePtr;
            public Context(IntPtr inPtr)
            {
                nativePtr = inPtr;
            }

            public int Destroy()
            {
                DestroyFunc InternalDestroy = Marshal.GetDelegateForFunctionPointer<DestroyFunc>(GetVtableEntry(0));
                return InternalDestroy(nativePtr);
            }

            public int AddMap(MapDesc pShadowMapDesc, BufferDesc pShadowBufferDesc, out Map pShadowMapHandle)
            {
                pShadowMapHandle = new Map();
                AddMapFunc InternalAddMap = Marshal.GetDelegateForFunctionPointer<AddMapFunc>(GetVtableEntry(2));
                IntPtr shadowMapDescPtr = InteropUtils.CreateStructure<MapDesc>(pShadowMapDesc);
                IntPtr shadowBufferDescPtr = InteropUtils.CreateStructure<BufferDesc>(pShadowBufferDesc);
                IntPtr ppShadowMapHandle = InteropUtils.AddressOf(pShadowMapHandle.ptr);

                int retVal = InternalAddMap(nativePtr, shadowMapDescPtr, shadowBufferDescPtr, ppShadowMapHandle);
                if (retVal == 0)
                    pShadowMapHandle.ptr = Marshal.ReadIntPtr(ppShadowMapHandle);

                Marshal.FreeHGlobal(ppShadowMapHandle);
                Marshal.FreeHGlobal(shadowBufferDescPtr);
                Marshal.FreeHGlobal(shadowMapDescPtr);

                return retVal;
            }

            public int RemoveMap(ref Map pShadowMapHandle)
            {
                RemoveMapFunc InternalRemoveMap = Marshal.GetDelegateForFunctionPointer<RemoveMapFunc>(GetVtableEntry(3));
                IntPtr ppShadowMapHandle = InteropUtils.AddressOf(pShadowMapHandle.ptr);

                int retVal = InternalRemoveMap(nativePtr, ppShadowMapHandle);
                if (retVal == 0)
                    pShadowMapHandle.ptr = IntPtr.Zero;

                Marshal.FreeHGlobal(ppShadowMapHandle);
                return retVal;
            }

            public int AddBuffer(BufferDesc pShadowBufferDesc, out Buffer pShadowBufferHandle)
            {
                pShadowBufferHandle = new Buffer();
                AddBufferFunc InternalAddBuffer = Marshal.GetDelegateForFunctionPointer<AddBufferFunc>(GetVtableEntry(4));
                IntPtr shadowBufferDescPtr = InteropUtils.CreateStructure<BufferDesc>(pShadowBufferDesc);
                IntPtr ppShadowBufferHandle = InteropUtils.AddressOf(pShadowBufferHandle.ptr);

                int retVal = InternalAddBuffer(nativePtr, shadowBufferDescPtr, ppShadowBufferHandle);
                if (retVal == 0)
                    pShadowBufferHandle.ptr = Marshal.ReadIntPtr(ppShadowBufferHandle);

                Marshal.FreeHGlobal(ppShadowBufferHandle);
                Marshal.FreeHGlobal(shadowBufferDescPtr);

                return retVal;
            }

            public int RemoveBuffer(ref Buffer pShadowBufferHandle)
            {
                RemoveBufferFunc InternalRemoveBuffer = Marshal.GetDelegateForFunctionPointer<RemoveBufferFunc>(GetVtableEntry(5));
                IntPtr ppShadowBufferHandle = InteropUtils.AddressOf(pShadowBufferHandle.ptr);

                int retVal = InternalRemoveBuffer(nativePtr, ppShadowBufferHandle);
                if (retVal == 0)
                    pShadowBufferHandle.ptr = IntPtr.Zero;

                Marshal.FreeHGlobal(ppShadowBufferHandle);
                return retVal;
            }

            public int SetMapRenderParams(Map pShadowMapHandle, MapRenderParams pShadowMapRenderParams)
            {
                SetMapRenderParamsFunc InternalSetMapRenderParams = Marshal.GetDelegateForFunctionPointer<SetMapRenderParamsFunc>(GetVtableEntry(6));
                IntPtr shadowMapRenderParamsPtr = InteropUtils.CreateStructure<MapRenderParams>(pShadowMapRenderParams);

                int retVal = InternalSetMapRenderParams(nativePtr, pShadowMapHandle.ptr, shadowMapRenderParamsPtr);
                Marshal.FreeHGlobal(shadowMapRenderParamsPtr);

                return retVal;
            }

            public int UpdateMapBounds(Map pShadowMapHandle, out Matrix[] pm4x4LightViewMatrix, out Matrix[] pm4x4LightProjectionMatrix, out Frustum[] pRenderFrustum)
            {
                pm4x4LightViewMatrix = new Matrix[4];
                pm4x4LightProjectionMatrix = new Matrix[4];
                pRenderFrustum = new Frustum[4];

                UpdateMapBoundsFunc InternalUpdateMapBounds = Marshal.GetDelegateForFunctionPointer<UpdateMapBoundsFunc>(GetVtableEntry(7));
                IntPtr param1 = Marshal.AllocHGlobal(4 * Marshal.SizeOf<Matrix>());
                IntPtr param2 = Marshal.AllocHGlobal(4 * Marshal.SizeOf<Matrix>());
                IntPtr param3 = Marshal.AllocHGlobal(4 * Marshal.SizeOf<Frustum>());

                for (int i = 0; i < 4; i++)
                {
                    Marshal.StructureToPtr<Matrix>(new Matrix(), param1 + (i * Marshal.SizeOf<Matrix>()), true);
                    Marshal.StructureToPtr<Matrix>(new Matrix(), param2 + (i * Marshal.SizeOf<Matrix>()), true);
                    Marshal.StructureToPtr<Frustum>(new Frustum(), param3 + (i * Marshal.SizeOf<Frustum>()), true);
                }

                int retVal = InternalUpdateMapBounds(nativePtr, pShadowMapHandle.ptr, param1, param2, param3);
                if (retVal == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        pm4x4LightViewMatrix[i] = Marshal.PtrToStructure<Matrix>(param1 + (i * Marshal.SizeOf<Matrix>()));
                        pm4x4LightProjectionMatrix[i] = Marshal.PtrToStructure<Matrix>(param2 + (i * Marshal.SizeOf<Matrix>()));
                        pRenderFrustum[i] = Marshal.PtrToStructure<Frustum>(param3 + (i * Marshal.SizeOf<Frustum>()));
                    }
                }

                Marshal.FreeHGlobal(param1);
                Marshal.FreeHGlobal(param2);
                Marshal.FreeHGlobal(param3);

                return retVal;
            }

            public int InitializeMapRendering(Map pShadowMapHandle, MapRenderType eMapRenderType)
            {
                InitializeMapRenderingFunc InternalInitializeMapRendering = Marshal.GetDelegateForFunctionPointer<InitializeMapRenderingFunc>(GetVtableEntry(8));
                return InternalInitializeMapRendering(nativePtr, pShadowMapHandle.ptr, eMapRenderType);
            }

            public int BeginMapRendering(Map pShadowMapHandle, MapRenderType eMapRenderType, uint uView)
            {
                BeginMapRenderingFunc InternalBeginMapRendering = Marshal.GetDelegateForFunctionPointer<BeginMapRenderingFunc>(GetVtableEntry(9));
                return InternalBeginMapRendering(nativePtr, pShadowMapHandle.ptr, eMapRenderType, uView);
            }

            public int IncrementMapPrimitiveCounter(Map pShadowMapHandle, MapRenderType eMapRenderType, uint uNumPrimitives)
            {
                IncrementMapPrimitiveCounterFunc InternalIncrementMapPrimitiveCounter = Marshal.GetDelegateForFunctionPointer<IncrementMapPrimitiveCounterFunc>(GetVtableEntry(10));
                return InternalIncrementMapPrimitiveCounter(nativePtr, pShadowMapHandle.ptr, eMapRenderType, uNumPrimitives);
            }

            public int EndMapRendering(Map pShadowMapHandle, MapRenderType eMapRenderType, uint uView)
            {
                EndMapRenderingFunc InternalEndMapRendering = Marshal.GetDelegateForFunctionPointer<EndMapRenderingFunc>(GetVtableEntry(12));
                return InternalEndMapRendering(nativePtr, pShadowMapHandle.ptr, eMapRenderType, uView);
            }

            public int ClearBuffer(Buffer pShadowBufferHandle)
            {
                ClearBufferFunc InternalClearBuffer = Marshal.GetDelegateForFunctionPointer<ClearBufferFunc>(GetVtableEntry(13));
                return InternalClearBuffer(nativePtr, pShadowBufferHandle.ptr);
            }

            public int RenderBuffer(Map pShadowMapHandle, Buffer pShadowBufferHandle, BufferRenderParams pShadowBufferRenderParams)
            {
                RenderBufferFunc InternalRenderBuffer = Marshal.GetDelegateForFunctionPointer<RenderBufferFunc>(GetVtableEntry(14));
                IntPtr shadowBufferRenderParamsPtr = InteropUtils.CreateStructure<BufferRenderParams>(pShadowBufferRenderParams);

                int retVal = InternalRenderBuffer(nativePtr, pShadowMapHandle.ptr, pShadowBufferHandle.ptr, shadowBufferRenderParamsPtr);
                Marshal.FreeHGlobal(shadowBufferRenderParamsPtr);

                return retVal;
            }

            public int FinalizeBuffer(Buffer pShadowBufferHandle, ref ShaderResourceView pShadowBufferSRV)
            {
                FinalizeBufferFunc InternalFinalizeBuffer = Marshal.GetDelegateForFunctionPointer<FinalizeBufferFunc>(GetVtableEntry(15));
                IntPtr srvPtr = InteropUtils.AddressOf(IntPtr.Zero);

                int retVal = InternalFinalizeBuffer(nativePtr, pShadowBufferHandle.ptr, srvPtr);
                if (retVal == 0)
                    pShadowBufferSRV = new ShaderResourceView(Marshal.ReadIntPtr(srvPtr));

                Marshal.FreeHGlobal(srvPtr);
                return retVal;
            }

            private IntPtr GetVtableEntry(int index)
            {
                return Marshal.ReadIntPtr(Marshal.ReadIntPtr(nativePtr, 0), index * 8);
            }

            private delegate int DestroyFunc(IntPtr self);
            private delegate int RemoveMapFunc(IntPtr self, IntPtr ppShadowMapHandle);
            private delegate int RemoveBufferFunc(IntPtr self, IntPtr ppShadowBufferHandle);
            private delegate int AddMapFunc(IntPtr self, IntPtr pShadowMapDesc, IntPtr pShadowBufferDesc, IntPtr ppShadowMapHandle);
            private delegate int AddBufferFunc(IntPtr self, IntPtr pShadowBufferDesc, IntPtr ppShadowBufferHandle);
            private delegate int SetMapRenderParamsFunc(IntPtr self, IntPtr pShadowMapHandle, IntPtr pShadowMapRenderParams);
            private delegate int UpdateMapBoundsFunc(IntPtr self, IntPtr pShadowMapHandle, IntPtr pm4x4LightViewMatrix, IntPtr pm4x4LightProjectionMatrix, IntPtr pRenderFrustum);
            private delegate int InitializeMapRenderingFunc(IntPtr self, IntPtr pShadowMapHandle, MapRenderType eMapRenderType);
            private delegate int BeginMapRenderingFunc(IntPtr self, IntPtr pShadowMapHandle, MapRenderType eMapRenderType, uint uView);
            private delegate int IncrementMapPrimitiveCounterFunc(IntPtr self, IntPtr pShadowMapHandle, MapRenderType eMapRenderType, uint uNumPrimitives);
            private delegate int EndMapRenderingFunc(IntPtr self, IntPtr pShadowMapHandle, MapRenderType eMapRenderType, uint uView);
            private delegate int ClearBufferFunc(IntPtr self, IntPtr pShadowMapHandle);
            private delegate int RenderBufferFunc(IntPtr self, IntPtr pShadowMapHandle, IntPtr pShadowBufferHandle, IntPtr pShadowBufferRenderParams);
            private delegate int FinalizeBufferFunc(IntPtr self, IntPtr pShadowBufferHandle, IntPtr pShadowBufferSRV);
        }

        [DllImport("thirdparty/GFSDK_ShadowLib_DX11.win64.dll", EntryPoint = "GFSDK_ShadowLib_GetDLLVersion")]
        public static extern int GetDLLVersion(IntPtr dllVersion);

        [DllImport("thirdparty/GFSDK_ShadowLib_DX11.win64.dll", EntryPoint = "GFSDK_ShadowLib_Create")]
        private static extern int CreateInernal(IntPtr pVersion, IntPtr ppContext, IntPtr pPlatformDevice, IntPtr customAllocator);

        public static int Create(Version pVersion, DeviceContext pPlatformDevice, out Context ppContext)
        {
            ppContext = null;

            IntPtr versionPtr = InteropUtils.CreateStructure<Version>(pVersion);
            IntPtr platformDevicePtr = InteropUtils.CreateStructure<DeviceContext>(pPlatformDevice);
            IntPtr contextPtr = InteropUtils.AddressOf(IntPtr.Zero);

            int retVal = CreateInernal(versionPtr, contextPtr, platformDevicePtr, IntPtr.Zero);
            if (retVal == 0)
                ppContext = new Context(Marshal.ReadIntPtr(contextPtr));

            Marshal.FreeHGlobal(contextPtr);
            Marshal.FreeHGlobal(versionPtr);
            Marshal.FreeHGlobal(platformDevicePtr);

            return retVal;
        }

        public static void Init(Device device, SharpDX.Direct3D11.DeviceContext context, int width, int height, ref Context shadowContext, ref Map shadowMapHandle, ref Buffer shadowBufferHandle)
        {
            if (shadowContext != null)
                return;

            // shadows
            GFSDK_ShadowLib.Version pVersion = new GFSDK_ShadowLib.Version() { uMajor = 3 };
            GFSDK_ShadowLib.DeviceContext deviceContext = new GFSDK_ShadowLib.DeviceContext()
            {
                pD3DDevice = device.NativePointer,
                pDeviceContext = context.NativePointer
            };

            int retCode = GFSDK_ShadowLib.Create(pVersion, deviceContext, out shadowContext);
            InitSizeDependent(shadowContext, width, height, ref shadowMapHandle, ref shadowBufferHandle);
        }

        public static int NumCSMLevels = 4;
        public static void InitSizeDependent(Context shadowContext, int width, int height, ref Map shadowMapHandle, ref Buffer shadowBufferHandle)
        {
            if (shadowContext == null)
                return;

            uint shadowMapRes = (uint)Config.Get<int>("RenderShadowRes", 2048);
            //uint shadowMapRes = (uint)Config.Get<int>("Render", "ShadowRes", 2048);
            uint FTMapRes = 256;
            uint FTMapScale = 8;
            uint RTMapRes = 256;
            uint RTMapScale = 8;

            GFSDK_ShadowLib.MapDesc mapDesc = new GFSDK_ShadowLib.MapDesc()
            {
                eViewType = (GFSDK_ShadowLib.ViewType)NumCSMLevels,
                uResolutionHeight = shadowMapRes,
                uResolutionWidth = shadowMapRes,
                FrustumTraceMapDesc = new GFSDK_ShadowLib.FrustumTraceMapDesc()
                {
                    bRequireFrustumTraceMap = 1,
                    uResolutionWidth = FTMapRes * FTMapScale,
                    uResolutionHeight = FTMapRes * FTMapScale,
                    uDynamicReprojectionCascades = 2,
                    uQuantizedListLengthTexelDimension = 4
                },
                RayTraceMapDesc = new GFSDK_ShadowLib.RayTraceMapDesc()
                {
                    bRequirePrimitiveMap = 1,
                    uMaxNumberOfPrimitives = 250000,
                    uMaxNumberOfPrimitivesPerPixel = 64,
                    uResolutionHeight = RTMapRes * RTMapScale,
                    uResolutionWidth = RTMapRes * RTMapScale
                }
            };
            GFSDK_ShadowLib.BufferDesc bufferDesc = new GFSDK_ShadowLib.BufferDesc()
            {
                uResolutionWidth = (uint)width,
                uResolutionHeight = (uint)height,
                uViewportTop = 0,
                uViewportLeft = 0,
                uViewportBottom = (uint)height,
                uViewportRight = (uint)width,
                uSampleCount = 1
            };

            shadowContext.AddMap(mapDesc, bufferDesc, out shadowMapHandle);
            shadowContext.AddBuffer(bufferDesc, out shadowBufferHandle);
        }
    }

    public class GFSDK_TXAA
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NvTxaaCurrentVersion
        {
            public uint major;
            public uint minor;
            public uint revision;
            public uint build;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UtilityFactoryParameters
        {
            public IntPtr allocator;
            public IntPtr errorHandler;
            public IntPtr performanceMonitor;
            public IntPtr device;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SharedPtr
        {
            public IntPtr ptr;
            public IntPtr allocator;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MotionVectorParameters
        {
            public IntPtr viewProj;
            public IntPtr prevViewProj;
            public int samples;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvTxaaResolveParametersDX11
        {
            public IntPtr txaaContext;
            public IntPtr deviceContext;
            public IntPtr resolveTarget;
            public IntPtr msaaSource;
            public IntPtr msaaDepth;
            public IntPtr feedbackSource;
            public IntPtr controlSource;
            public int alphaResolveMode;
            public IntPtr compressionRange;
            public IntPtr /* NvTxaaFeedbackParameters */ feedback;
            public IntPtr debug;
            public IntPtr /* NvTxaaPerFrameConstants */ perFrameConstants;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvTxaaPerFrameConstants
        {
            public uint useBHFilters;
            public uint useAntiFlickerFilter;
            public uint motionVecSelection;
            public uint useRGB;

            public uint isZFlipped;
            public float xJitter;
            public float yJitter;
            public float mvScale;

            public uint enableClipping;
            public float frameBlendFactor;
            public uint dbg1;
            public float bbScale;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvTxaaFeedbackParameters
        {
            public float weight;
            public float clippedWeight;

            public static NvTxaaFeedbackParameters NvTxaaDefaultFeedback = new NvTxaaFeedbackParameters() { weight = 0.75f, clippedWeight = 0.75f };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvTxaaMotionDX11
        {
            public IntPtr motionVectors;
            public IntPtr motionVectorsMS;
            public IntPtr /* NvTxaaMotionParameters */ parameters;
        }

        [DllImport("thirdparty/GFSDK_Txaa.win64.dll", EntryPoint = "GFSDK_TXAA_DX11_InitializeContext")]
        public static extern int InitializeContext(IntPtr txaaContext, IntPtr device, IntPtr version);

        [DllImport("thirdparty/NvTxaaUtil.x64.dll", EntryPoint = "GFSDK_TXAA_DX11_CreateUtilityFactory")]
        public static extern int CreateUtilityFactory(IntPtr factory, UtilityFactoryParameters inParams, uint version = 1);

        [DllImport("thirdparty/GFSDK_Txaa.win64.dll", EntryPoint = "GFSDK_TXAA_DX11_ResolveFromMotionVectors")]
        public static extern int ResolveFromMotionVectors(IntPtr resolveParams, IntPtr motion);

        [DllImport("thirdparty/GFSDK_Txaa.win64.dll", EntryPoint = "GFSDK_TXAA_DX11_ReleaseContext")]
        public static extern int ReleaseContext(IntPtr txaaContext);

        public delegate int CreateTargetCopierFunc(IntPtr self, IntPtr targetCopier);
        public delegate int CreateMotionVectorGenerator(IntPtr self, IntPtr mvGenerator);
        public delegate int GenerateMotionVectorFunc(IntPtr self, IntPtr context, IntPtr rtv, IntPtr depthSrv, MotionVectorParameters inParams);
        public delegate int CopyTargetFunc(IntPtr self, IntPtr deviceContext, IntPtr target, IntPtr source);

        public static void Init(Device device, ref IntPtr txaaContext, ref IntPtr motionVectorGenerator)
        {
            // txaa
            txaaContext = Marshal.AllocHGlobal(8192);
            GFSDK_TXAA.NvTxaaCurrentVersion version = new GFSDK_TXAA.NvTxaaCurrentVersion() { major = 3 };
            IntPtr versionPtr = Marshal.AllocHGlobal(16);
            Marshal.StructureToPtr<GFSDK_TXAA.NvTxaaCurrentVersion>(version, versionPtr, true);

            int retCode = GFSDK_TXAA.InitializeContext(txaaContext, device.NativePointer, versionPtr);

            GFSDK_TXAA.UtilityFactoryParameters utilParams = new GFSDK_TXAA.UtilityFactoryParameters() { device = device.NativePointer };

            IntPtr utilFactory = Marshal.AllocHGlobal(16);
            motionVectorGenerator = Marshal.AllocHGlobal(16);

            Marshal.StructureToPtr<GFSDK_TXAA.SharedPtr>(new GFSDK_TXAA.SharedPtr(), utilFactory, true);
            Marshal.StructureToPtr<GFSDK_TXAA.SharedPtr>(new GFSDK_TXAA.SharedPtr(), motionVectorGenerator, true);

            IntPtr utilParamsPtr = Marshal.AllocHGlobal(8 * 4);
            Marshal.StructureToPtr<GFSDK_TXAA.UtilityFactoryParameters>(utilParams, utilParamsPtr, true);

            retCode = GFSDK_TXAA.CreateUtilityFactory(utilFactory, utilParams);
            IntPtr utilFactoryVtbl = Marshal.ReadIntPtr(Marshal.ReadIntPtr(utilFactory, 0), 0);

            GFSDK_TXAA.CreateMotionVectorGenerator createMotionVectorGeneratorFunc = Marshal.GetDelegateForFunctionPointer<GFSDK_TXAA.CreateMotionVectorGenerator>(Marshal.ReadIntPtr(utilFactoryVtbl, 4 * 8));
            retCode = createMotionVectorGeneratorFunc(Marshal.ReadIntPtr(utilFactory), motionVectorGenerator);

            Marshal.FreeHGlobal(utilParamsPtr);
            Marshal.FreeHGlobal(utilFactory);
        }

        public static void Destroy(ref IntPtr txaaContext, ref IntPtr motionVectorGenerator)
        {
            ReleaseContext(txaaContext);
        }

        private static readonly byte[] Halton2_Sequence =
        {
            8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15, 0
        };

        private static readonly byte[] Halton3_Sequence =
        {
             5, 10, 1, 7, 12, 3, 9, 14, 0, 6, 11, 2, 8, 13, 4, 15
        };

        private static byte Halton2(int index, int numFrames)
        {
            return Halton2_Sequence[index % numFrames];
        }

        private static byte Halton3(int index, int numFrames)
        {
            return Halton3_Sequence[index % numFrames];
        }

        private static byte Halton(int index, int component, int numFrames)
        {
            switch (component)
            {
                case 0: return Halton2(index, numFrames);
                case 1: return Halton3(index, numFrames);
                default: return 0;
            }
        }

        private static int frameNumber = 0;
        private static readonly int NumFrames = 16;
        private static readonly float[] Jitter = { 0.0f, 0.0f };

        public static bool TxaaEnabled { get; set; }

        public static void Update(int width, int height)
        {
            if (TxaaEnabled)
            {
                byte x = Halton(frameNumber % NumFrames, 0, NumFrames);
                byte y = Halton(frameNumber % NumFrames, 1, NumFrames);
                Jitter[0] = 2.0f * (((float)x / 16.0f) - 0.5f) / width;
                Jitter[1] = 2.0f * (((float)y / 16.0f) - 0.5f) / height;
                frameNumber++;
            }
            else
            {
                Jitter[0] = 0.0f;
                Jitter[1] = 0.0f;
            }
        }

        public static void GetJitter(out float[] outJitter)
        {
            outJitter = new float[2] { Jitter[0], Jitter[1] };
        }
    }

    public class GFSDK_SSAO
    {
        public enum DepthStorage
        {
            FP16,
            FP32
        }

        public enum DepthClampMode
        {
            ClampToEdge,
            ClampToBorder
        }

        public enum BlurRadius
        {
            BlurRadius2,
            BlurRadius4
        }

        public enum DepthTextureType
        {
            HardwareDepths,
            HardwareDepthsSubRange,
            ViewDepths
        }

        public enum MatrixLayout
        {
            RowMajorOrder,
            ColumnMajorOrder
        }

        public enum BlendMode
        {
            OverwriteRGB,
            MultiplyRGB,
            CustomBlend
        }

        public enum DepthStencilMode
        {
            DisabledDepthStencil,
            CustomDepthStencil
        }

        [Flags]
        public enum RenderMask
        {
            DrawZ = 1,
            DrawAO = 2,
            DrawDebugN = 4,
            DrawDebugX = 8,
            DrawDebugY = 16,
            DrawDebugZ = 32,
            RenderAO = DrawZ|DrawAO,
            RenderDebugNormal = DrawZ|DrawDebugN,
            RenderDebugNormalX = DrawZ|DrawDebugN|DrawDebugX,
            RenderDebugNormalY = DrawZ|DrawDebugN|DrawDebugY,
            RenderDebugNormalZ = DrawZ|DrawDebugN|DrawDebugZ,
        }

        public struct Version
        {
            public uint Major;
            public uint Minor;
            public uint Branch;
            public uint Revision;

            public static Version Default = new Version()
            {
                Major = 3,
                Minor = 1,
                Branch = 0,
                Revision = 21602716
            };
        }

        public struct ForegroundAO
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public float ForegroundViewDepth;
        }

        public struct BackgroundAO
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public float BackgroundViewDepth;
        }

        public struct DepthThreshold
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public float MaxViewDepth;
            public float Sharpness;
        }

        public struct BlurParameters
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public BlurRadius Radius;
            public float Sharpness;
            public BlurSharpnessProfile SharpnessProfile;

            public BlurParameters(bool dummy)
            {
                Enable = true;
                Radius = BlurRadius.BlurRadius2;
                Sharpness = 64.0f;
                SharpnessProfile = new BlurSharpnessProfile(true);
            }
        }

        public struct BlurSharpnessProfile
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public float ForegroundSharpnessScale;
            public float ForegroundViewDepth;
            public float BackgroundViewDepth;

            public BlurSharpnessProfile(bool dummy)
            {
                Enable = false;
                ForegroundSharpnessScale = 4.0f;
                ForegroundViewDepth = 0.0f;
                BackgroundViewDepth = 1.0f;
            }
        }

        public struct Parameters
        {
            public float Radius;
            public float Bias;
            public float SmallScaleAO;
            public float LargeScaleAO;
            public float PowerExponent;
            public ForegroundAO ForegroundAO;
            public BackgroundAO BackgroundAO;
            public DepthStorage DepthStorage;
            public DepthClampMode DepthClampMode;
            public DepthThreshold DepthTheshold;
            public BlurParameters Blur;

            public Parameters(bool dummy)
            {
                Radius = 0.1f;
                Bias =10.0f;
                SmallScaleAO = 1.0f;
                LargeScaleAO = 1.0f;
                PowerExponent = 4.0f;
                ForegroundAO = new ForegroundAO();
                BackgroundAO = new BackgroundAO();
                DepthStorage = DepthStorage.FP16;
                DepthClampMode = DepthClampMode.ClampToEdge;
                DepthTheshold = new DepthThreshold();
                Blur = new BlurParameters(true);
            }
        }

        public struct InputViewport
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public uint TopLeftX;
            public uint TopLeftY;
            public uint Width;
            public uint Height;
            public float MinDepth;
            public float MaxDepth;

            public static InputViewport FromViewport(SharpDX.Viewport viewport)
            {
                return new InputViewport
                {
                    Enable = true,
                    TopLeftX = (uint)viewport.X,
                    TopLeftY = (uint)viewport.Y,
                    Width = (uint)viewport.Width,
                    Height = (uint)viewport.Height,
                    MinDepth = viewport.MinDepth,
                    MaxDepth = viewport.MaxDepth
                };
            }
        }

        public struct MatrixData
        {
            public Matrix Data;
            public MatrixLayout Layout;
        }
        public struct InputNormalData
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public MatrixData WorldToViewMatrix;
            public float DecodeScale;
            public float DecodeBias;
            public IntPtr pFullResNormalTextureSRV;
        }

        public struct InputDepthData
        {
            public DepthTextureType DepthTextureType;
            public MatrixData ProjectionMatrix;
            public float MetersToViewSpaceUnits;
            public InputViewport Viewport;
            public IntPtr pFullResDepthTextureSRV;
        }

        public struct InputData
        {
            public InputDepthData DepthData;
            public InputNormalData NormalData;
        }

        public struct CustomBlendState
        {
            public IntPtr pBlendState;
            public IntPtr pBlendFactor;
        }

        public struct CustomDepthStencilState
        {
            public IntPtr pDepthStencilState;
            public uint StencilRef;
        }

        public struct BlendState
        {
            public BlendMode Mode;
            public CustomBlendState CustomState;
        }

        public struct DepthStencilState
        {
            public DepthStencilMode Mode;
            public CustomDepthStencilState CustomState;
        }

        public struct BlendPass
        {
            public BlendState Blend;
            public DepthStencilState DepthStencil;
        }

        public struct TwoPassBlend
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool Enable;
            public IntPtr pDepthStencilView;
            public BlendPass FirstPass;
            public BlendPass SecondPass;
        }

        public struct Output
        {
            public IntPtr pRenderTargetView;
            public BlendState Blend;
            public TwoPassBlend TwoPassBlend;
        }

        public class Context
        {
            private IntPtr nativePtr;
            public Context(IntPtr inPtr)
            {
                nativePtr = inPtr;
            }

            // 0 = GetAllocatedVideoMemoryBytes

            // RenderAO
            public int RenderAO(DeviceContext pDeviceContext, InputData InputData, Parameters Parameters, Output Output, RenderMask RenderMask = RenderMask.RenderAO)
            {
                RenderAOFunc RenderAOInternal = Marshal.GetDelegateForFunctionPointer<RenderAOFunc>(GetVtableEntry(1));
                return RenderAOInternal(nativePtr, pDeviceContext.NativePointer, InputData, Parameters, Output, RenderMask);
            }

            // 2 = PreCreateRTs
            // 3 = GetProjectionMatrixDepthRange

            // Release
            public void Release()
            {
                ReleaseFunc ReleaseInternal = Marshal.GetDelegateForFunctionPointer<ReleaseFunc>(GetVtableEntry(4));
                ReleaseInternal(nativePtr);
            }

            private IntPtr GetVtableEntry(int index)
            {
                return Marshal.ReadIntPtr(Marshal.ReadIntPtr(nativePtr, 0), index * 8);
            }

            private delegate void ReleaseFunc(IntPtr self);
            private delegate int RenderAOFunc(IntPtr self, IntPtr pDeviceContext, InputData InputData, Parameters Parameters, Output Output, RenderMask RenderMask);
        }

        [DllImport("thirdparty/GFSDK_SSAO_D3D11.win64.dll", EntryPoint = "GFSDK_SSAO_CreateContext_D3D11")]
        private static extern int CreateContextInternal(IntPtr pD3DDevice, IntPtr ppContext, IntPtr pCustomHeap, Version HeaderVersion);

        public static int CreateContext(Device device, out Context ppContext)
        {
            ppContext = null;

            IntPtr contextPtr = InteropUtils.AddressOf(IntPtr.Zero);
            int retVal = CreateContextInternal(device.NativePointer, contextPtr, IntPtr.Zero, Version.Default);
            if (retVal == 0)
                ppContext = new Context(Marshal.ReadIntPtr(contextPtr));

            Marshal.FreeHGlobal(contextPtr);
            return retVal;
        }

        public static int Init(Device device, ref Context context)
        {
            int retVal = CreateContext(device, out context);
            return retVal;
        }
    }

    public class TextureLibrary : IDisposable
    {
        private Dictionary<string, Tuple<SharpDX.Direct3D11.Resource, ShaderResourceView>> textures = new Dictionary<string, Tuple<SharpDX.Direct3D11.Resource, ShaderResourceView>>();
        private Device device;

        public TextureLibrary(Device inDevice)
        {
            device = inDevice;
        }

        public ShaderResourceView LoadTextureAsset(string filename, bool generateMips = false)
        {
            if (textures.ContainsKey(filename))
            {
                int count = (int)textures[filename].Item1.Tag;
                textures[filename].Item1.Tag = ++count;

                return textures[filename].Item2;
            }

            Texture2D dxtex = TextureUtils.LoadTexture(device, filename, generateMips);
            ShaderResourceView srv = new ShaderResourceView(device, dxtex);
            dxtex.Tag = (int)1;

            textures.Add(filename, new Tuple<SharpDX.Direct3D11.Resource, ShaderResourceView>(dxtex, srv));
            return srv;
        }

        public ShaderResourceView LoadTextureAsset(Guid AssetGuid, bool generateMips = false)
        {
            if (AssetGuid == Guid.Empty)
                return null;

            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(AssetGuid);
            if (entry == null)
                return null;

            if (textures.ContainsKey(entry.Name))
            {
                int count = (int)textures[entry.Name].Item1.Tag;
                textures[entry.Name].Item1.Tag = ++count;

                return textures[entry.Name].Item2;
            }

            EbxAsset asset = App.AssetManager.GetEbx(entry);

            dynamic root = asset.RootObject;
            ulong resourceId = root.Resource;

            using (Texture texture = App.AssetManager.GetResAs<Texture>(App.AssetManager.GetResEntry(resourceId)))
            {
                ShaderResourceViewDescription desc = new ShaderResourceViewDescription {Format = TextureUtils.ToShaderFormat(texture.PixelFormat, (texture.Flags & TextureFlags.SrgbGamma) != 0)};

                if (texture.Type == TextureType.TT_3d)
                {
                    // @temp
                    desc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DArray;
                    desc.Texture2DArray = new ShaderResourceViewDescription.Texture2DArrayResource()
                    {
                        ArraySize = texture.Depth,
                        FirstArraySlice = 0,
                        MipLevels = texture.MipCount,
                        MostDetailedMip = 0
                    };
                }
                else if (texture.Type == TextureType.TT_Cube)
                {
                    desc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.TextureCube;
                    desc.TextureCube = new ShaderResourceViewDescription.TextureCubeResource()
                    {
                        MipLevels = texture.MipCount,
                        MostDetailedMip = 0
                    };
                }
                else
                {
                    if (texture.SliceCount > 1)
                    {
                        desc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DArray;
                        desc.Texture2DArray = new ShaderResourceViewDescription.Texture2DArrayResource()
                        {
                            ArraySize = texture.SliceCount,
                            FirstArraySlice = 0,
                            MipLevels = texture.MipCount,
                            MostDetailedMip = 0
                        };
                    }
                    else
                    {
                        desc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
                        desc.Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                        {
                            MipLevels = texture.MipCount,
                            MostDetailedMip = 0
                        };
                    }
                }

                var dxtex = TextureUtils.LoadTexture(device, texture, generateMips);
                ShaderResourceView srv = new ShaderResourceView(device, dxtex, desc);

                dxtex.Tag = (int)1;
                textures.Add(entry.Name, new Tuple<SharpDX.Direct3D11.Resource, ShaderResourceView>(dxtex, srv));

                return srv;
            }
        }

        public void UnloadTexture(ShaderResourceView srv)
        {
            if (srv == null)
                return;

            foreach (string key in textures.Keys)
            {
                Tuple<SharpDX.Direct3D11.Resource, ShaderResourceView> tview = textures[key];
                if (tview.Item2 == srv)
                {
                    int count = (int)tview.Item1.Tag;
                    if (--count != 0)
                    {
                        tview.Item1.Tag = count;
                        return;
                    }

                    tview.Item1.Dispose();
                    tview.Item2.Dispose();
                    textures.Remove(key);
                    return;
                }
            }
        }

        public void UnloadTextures(params ShaderResourceView[] srvs)
        {
            foreach (ShaderResourceView srv in srvs)
                UnloadTexture(srv);
        }

        public void Dispose()
        {
            foreach (Tuple<SharpDX.Direct3D11.Resource, ShaderResourceView> tuple in textures.Values)
            {
                tuple.Item1.Dispose();
                tuple.Item2.Dispose();
            }
            textures.Clear();
        }
    }

    public class ShaderLibrary : IDisposable
    {
        private class ShaderInfo
        {
            public string XmlDescriptor;
            public Shader Shader;
        }

        private Dictionary<int, ShaderInfo> userShaders = new Dictionary<int, ShaderInfo>();
        private Shader fallbackShader;

        public ShaderLibrary(Shader inFallbackShader)
        {
            if (userShaders.Count == 0)
            {
                fallbackShader = inFallbackShader;
                foreach (string line in App.PluginManager.GetUserShaders())
                {
                    ExtractShader(line);
                }
                if (File.Exists("Shaders/Shaders.txt"))
                {
                    using (NativeReader reader = new NativeReader(new FileStream("Shaders/Shaders.txt", FileMode.Open, FileAccess.Read)))
                    {
                        while (reader.Position < reader.Length)
                        {
                            string line = reader.ReadLine();
                            ExtractShader(line);
                        }
                    }
                }
            }
        }

        private void ExtractShader(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            string[] arr = line.Split(',');
            if (arr.Length < 2)
                return;

            ShaderInfo info = new ShaderInfo() { XmlDescriptor = arr[1], Shader = null };
            userShaders.Add(Fnv1.HashString(arr[0].ToLower()), info);
        }

        public ShaderPermutation GetUserShader(string name, GeometryDeclarationDesc geomDecl)
        {
            int nameHash = Fnv1.HashString(name.ToLower());
            if (!userShaders.ContainsKey(nameHash))
                return null;

            ShaderInfo info = userShaders[nameHash];
            if (info.Shader == null)
            {
                info.Shader = new Shader();
                if (!info.Shader.Load(info.XmlDescriptor))
                    return null;
            }

            info.Shader.RefCount++;
            return info.Shader.GetPermutation(geomDecl);
        }

        public ShaderPermutation GetFallbackShader()
        {
            fallbackShader.RefCount++;
            return fallbackShader.GetPermutation(0);
        }

        public void UnloadShader(ShaderPermutation permutation)
        {
            Shader shader = permutation.Parent;
            shader.RefCount--;
            if (shader.RefCount == 0 && !permutation.IsFallback)
                shader.Dispose();
        }

        public void Dispose()
        {
            foreach (ShaderInfo shaderInfo in userShaders.Values)
            {
                shaderInfo.Shader?.Dispose();
            }
            userShaders.Clear();

            fallbackShader?.Dispose();
        }
    }

    public class GBuffer : IDisposable
    {
        public Texture2D Texture { get; private set; }
        public ShaderResourceView SRV { get; private set; }
        public RenderTargetView RTV { get; private set; }
        public Color4 ClearColor { get; private set; }
        public string DebugName { get; private set; }

        public GBuffer(Device device, SharpDX.DXGI.Format format, int width, int height, Color4 clearColor, string debugName = "")
        {
            Texture = new Texture2D(device, new Texture2DDescription()
            {
                Format = format,
                Width = width,
                Height = height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });
            SRV = new ShaderResourceView(device, Texture);
            RTV = new RenderTargetView(device, Texture);
            ClearColor = clearColor;

            Texture.DebugName = debugName;
            SRV.DebugName = debugName + " (SRV)";
            RTV.DebugName = debugName + " (RTV)";
        }

        public void Clear(DeviceContext context)
        {
            context.ClearRenderTargetView(RTV, ClearColor);
        }

        public void Dispose()
        {
            RTV.Dispose();
            SRV.Dispose();
            Texture.Dispose();
        }
    }

    public struct GBufferDescription
    {
        public SharpDX.DXGI.Format Format { get; set; }
        public SharpDX.Color4 ClearColor { get; set; }
        public string DebugName { get; set; }
    }

    public class GBufferCollection : IDisposable
    {
        private List<GBuffer> gBuffers = new List<GBuffer>();
        public GBufferCollection(Device device, int width, int height, params GBufferDescription[] descriptions)
        {
            foreach (GBufferDescription description in descriptions)
                gBuffers.Add(new GBuffer(device, description.Format, width, height, description.ClearColor, description.DebugName));
        }

        public ShaderResourceView[] GBufferSRVs
        {
            get
            {
                ShaderResourceView[] srvs = new ShaderResourceView[gBuffers.Count];
                for (int i = 0; i < gBuffers.Count; i++)
                    srvs[i] = gBuffers[i].SRV;
                return srvs;
            }
        }

        public RenderTargetView[] GBufferRTVs
        {
            get
            {
                RenderTargetView[] rtvs = new RenderTargetView[gBuffers.Count];
                for (int i = 0; i < gBuffers.Count; i++)
                    rtvs[i] = gBuffers[i].RTV;
                return rtvs;
            }
        }

        public void Clear(DeviceContext context)
        {
            foreach (GBuffer gBuffer in gBuffers)
                gBuffer.Clear(context);
        }

        public void Dispose()
        {
            foreach (GBuffer gBuffer in gBuffers)
                gBuffer.Dispose();
            gBuffers.Clear();
        }
    }

    public struct MeshRenderInstance
    {
        public MeshRenderBase RenderMesh;
        public Matrix Transform;
    }

    public enum LightRenderType
    {
        Sphere
    }

    public struct LightRenderInstance
    {
        public LightRenderType Type;
        public Matrix Transform;
        public Vector3 Color;
        public float Intensity;
        public float AttenuationRadius;
        public float SphereRadius;
        public int LightId;
    }

    // @temp: Find a better place for this
    public struct Matrix4x3
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;

        public Matrix4x3(float[] v)
        {
            if (v.Length < 12)
                throw new InvalidDataException();
            M11 = v[0]; M12 = v[1]; M13 = v[2]; M14 = v[3];
            M21 = v[4]; M22 = v[5]; M23 = v[6]; M24 = v[7];
            M31 = v[8]; M32 = v[9]; M33 = v[10]; M34 = v[11];
        }
    }

    public class BindableTexture : IDisposable
    {
        public Texture2D Texture { get; protected set; }
        public ShaderResourceView SRV { get; protected set; }
        public RenderTargetView RTV { get; protected set; }

        protected BindableTexture()
        {
        }

        public BindableTexture(Device device, Texture2DDescription description, bool srv, bool rtv, ShaderResourceViewDescription? srvDesc = null, RenderTargetViewDescription? rtvDesc = null)
        {
            description.BindFlags |= (srv) ? BindFlags.ShaderResource : BindFlags.None;
            description.BindFlags |= (rtv) ? BindFlags.RenderTarget : BindFlags.None;
            description.BindFlags |= GetAdditionalFlags();

            Texture = new Texture2D(device, description);
            if (srv)
            {
                SRV = srvDesc != null ? new ShaderResourceView(device, Texture, srvDesc.Value) : new ShaderResourceView(device, Texture);
            }
            if (rtv)
            {
                RTV = rtvDesc != null ? new RenderTargetView(device, Texture, rtvDesc.Value) : new RenderTargetView(device, Texture);
            }
        }

        public void Clear(DeviceContext context, Color4 color)
        {
            context.ClearRenderTargetView(RTV, color);
        }

        public virtual void Dispose()
        {
            RTV?.Dispose();
            SRV?.Dispose();
            Texture.Dispose();
        }

        protected virtual BindFlags GetAdditionalFlags()
        {
            return BindFlags.None;
        }
    }

    public class BindableCubeTexture : BindableTexture, IDisposable
    {
        private RenderTargetView[] rtvs;
        private int arraySize;
        private int mipCount;

        public BindableCubeTexture(Device device, Texture2DDescription description, bool srv, bool rtv, ShaderResourceViewDescription? srvDesc = null, RenderTargetViewDescription? rtvDesc = null)
        {
            description.BindFlags |= (srv) ? BindFlags.ShaderResource : BindFlags.None;
            description.BindFlags |= (rtv) ? BindFlags.RenderTarget : BindFlags.None;

            Texture = new Texture2D(device, description);
            if (srv)
            {
                SRV = srvDesc != null ? new ShaderResourceView(device, Texture, srvDesc.Value) : new ShaderResourceView(device, Texture);
            }
            if (rtv)
            {
                arraySize = description.ArraySize;
                mipCount = description.MipLevels;
                rtvs = new RenderTargetView[arraySize * mipCount];

                if (!rtvDesc.HasValue)
                    rtvDesc = new RenderTargetViewDescription() { Format = description.Format };

                for (int i = 0; i < arraySize; i++)
                {
                    for (int j = 0; j < mipCount; j++)
                    {
                        RenderTargetViewDescription desc = rtvDesc.Value;
                        desc.Dimension = RenderTargetViewDimension.Texture2DArray;
                        desc.Texture2DArray = new RenderTargetViewDescription.Texture2DArrayResource()
                        {
                            FirstArraySlice = i,
                            MipSlice = j,
                            ArraySize = 1
                        };

                        rtvs[(i * mipCount) + j] = new RenderTargetView(device, Texture, desc);
                    }
                }
            }
        }

        public RenderTargetView GetRTV(int arraySlice, int mipLevel)
        {
            return rtvs[(arraySlice * mipCount) + mipLevel];
        }

        public void Clear(DeviceContext context, int arraySlice, int mipLevel, Color4 color)
        {
            context.ClearRenderTargetView(rtvs[(arraySlice * mipCount) + mipLevel], color);
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (RenderTargetView rtv in rtvs)
                rtv.Dispose();
        }
    }

    public class BindableDepthTexture : BindableTexture, IDisposable
    {
        public DepthStencilView DSV { get; protected set; }
        public BindableDepthTexture(Device device, Texture2DDescription description, bool srv, DepthStencilViewDescription? dsvDesc = null, ShaderResourceViewDescription? srvDesc = null)
            : base(device, description, srv, false, srvDesc)
        {
            DSV = dsvDesc.HasValue ? new DepthStencilView(device, Texture, dsvDesc.Value) : new DepthStencilView(device, Texture);
        }

        public void Clear(DeviceContext context, bool clearDepth, bool clearStencil, float depth, byte stencil)
        {
            DepthStencilClearFlags flags = 0;
            flags |= (clearDepth) ? DepthStencilClearFlags.Depth : 0;
            flags |= (clearStencil) ? DepthStencilClearFlags.Stencil : 0;

            context.ClearDepthStencilView(DSV, flags, depth, stencil);
        }

        public override void Dispose()
        {
            base.Dispose();
            DSV.Dispose();
        }

        protected override BindFlags GetAdditionalFlags()
        {
            return BindFlags.DepthStencil;
        }
    }

    public class BindableBuffer : IDisposable
    {
        public SharpDX.Direct3D11.Buffer Buffer { get; private set; }
        public ShaderResourceView SRV { get; private set; }
        public int SizeInBytes { get; private set; }

        public BindableBuffer(Device device, int sizeInBytes, bool srv)
        {
            Construct(device, sizeInBytes, srv);
        }

        protected void Construct(Device device, int sizeInBytes, bool srv)
        {
            SizeInBytes = sizeInBytes;

            BindFlags flags = BindFlags.None;
            flags |= (srv) ? BindFlags.ShaderResource : BindFlags.None;

            BufferDescription desc = new BufferDescription()
            {
                BindFlags = flags,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = SizeInBytes,
                Usage = ResourceUsage.Dynamic,
            };
            Buffer = new SharpDX.Direct3D11.Buffer(device, desc);
            if (srv)
            {
                SRV = new ShaderResourceView(device, Buffer, new ShaderResourceViewDescription()
                {
                    Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Buffer,
                    Format = SharpDX.DXGI.Format.R32G32B32A32_Float,
                    Buffer = new ShaderResourceViewDescription.BufferResource()
                    {
                        ElementCount = 1,
                        ElementWidth = SizeInBytes / 16
                    }
                });
            }
        }

        public void Dispose()
        {
            SRV?.Dispose();
            Buffer.Dispose();
        }
    }

    public class BoneBuffer : BindableBuffer
    {
        public int BoneCount { get; private set; }

        public BoneBuffer(Device device, int numBones)
            : base(device, 3 * 16 * numBones, true)
        {
            BoneCount = numBones;
        }

        public void Update(DeviceContext context, int realBoneCount, params Matrix[] boneMatrices)
        {
            if (boneMatrices.Length > BoneCount)
            {
                Dispose();
                Construct(context.Device, 16 + (3 * 16 * boneMatrices.Length), true);
                BoneCount = boneMatrices.Length;
            }

            // @hack
            if (realBoneCount == -1)
                realBoneCount = 0;

            context.MapSubresource(Buffer, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
            {
                stream.Write(new Vector4((float)realBoneCount, 0, 0, 0));
                foreach (Matrix boneMatrix in boneMatrices)
                {
                    boneMatrix.Transpose();
                    stream.Write(boneMatrix.Row1);
                    stream.Write(boneMatrix.Row2);
                    stream.Write(boneMatrix.Row3);
                }
            }
            context.UnmapSubresource(Buffer, 0);
        }
    }

    public enum DebugRenderMode
    {
        Default,
        Wireframe,
        BaseColor,
        SpecularColor,
        Normals,
        MaterialAO,
        Smoothness,
        Metallic,
        Reflectance,
        Ambient,
        HBAO
    }

    public class SphericalHarmonicsHelper
    {
        public static float GaussianDistribution(float x, float y, float rho)
        {
            float g = 1.0f / (float)Math.Sqrt(2.0f * Math.PI * rho * rho);
            g *= (float)Math.Exp(-(x * x + y * y) / (2 * rho * rho));

            return g;
        }

        public static float[] shEvaluateDir(Vector3 dir)
        {
            float[] result = new float[9];
            double p_0_0 = 0.282094791773878140;
            double p_1_0 = 0.488602511902919920 * dir.Z;
            double p_1_1 = -0.488602511902919920;
            double p_2_0 = 0.946174695757560080 * dir.Z * dir.Z - 0.315391565252520050;
            double p_2_1 = -1.092548430592079200 * dir.Z;
            double p_2_2 = 0.546274215296039590;
            result[0] = (float)p_0_0;
            result[1] = (float)(p_1_1 * dir.Y);
            result[2] = (float)p_1_0;
            result[3] = (float)(p_1_1 * dir.X);
            result[4] = (float)(p_2_2 * (dir.X * dir.Y + dir.Y * dir.X));
            result[5] = (float)(p_2_1 * dir.Y);
            result[6] = (float)p_2_0;
            result[7] = (float)(p_2_1 * dir.X);
            result[8] = (float)(p_2_2 * (dir.X * dir.X - dir.Y * dir.Y));
            return result;
        }

        public static float[] shAdd(float[] inputA, float[] inputB)
        {
            int numCoeff = 9;
            float[] result = new float[numCoeff];

            for (int i = 0; i < numCoeff; i++)
            {
                result[i] = inputA[i] + inputB[i];
            }

            return result;
        }

        public static float[] shScale(float[] input, float scale)
        {
            const int numCoeff = 9;
            float[] result = new float[numCoeff];

            for (int i = 0; i < numCoeff; i++)
            {
                result[i] = input[i] * scale;
            }

            return result;
        }
    }

    public class Histogram
    {
        private float[] values;
        private int index = 0;
        private int maxValue = 0;
        private bool looped = false;

        public Histogram(int inMaxValue)
        {
            maxValue = inMaxValue;
            values = new float[maxValue];
        }

        public void Add(float value)
        {
            values[index++] = value;
            if (index == maxValue)
            {
                index = 0;
                looped = true;
            }
        }

        public float GetAverage()
        {
            int count = (looped) ? maxValue : index;
            if (count == 0)
                return 0.0006747352f;

            float totalValue = 0.0f;

            for (int i = 0; i < count; i++)
                totalValue += values[i];

            return totalValue / (float)count;
        }
    }

    public class DeferredRenderScreen2 : Screen
    {
        #region -- Shader Constants --
        protected struct ViewConstants
        {
            public Vector4 Time;
            public Vector4 ScreenSize;
            public Matrix ViewMatrix;
            public Matrix ProjMatrix;
            public Matrix ViewProjMatrix;
            public Matrix CrViewProjMatrix;
            public Matrix PrevViewProjMatrix;
            public Matrix CrPrevViewProjMatrix;
            public Matrix4x3 NormalBasisTransforms1;
            public Matrix4x3 NormalBasisTransforms2;
            public Matrix4x3 NormalBasisTransforms3;
            public Matrix4x3 NormalBasisTransforms4;
            public Matrix4x3 NormalBasisTransforms5;
            public Matrix4x3 NormalBasisTransforms6;
            public Vector4 ExposureMultipliers;
            public Vector4 CameraPos;
        }

        protected struct CommonConstants
        {
            public Matrix InvViewProjMatrix;
            public Matrix InvProjMatrix;
            public Vector4 CameraPos;
            public Vector4 InvScreenSize;
            public Vector4 ExposureMultipliers;
            public Matrix4x3 NormalBasisTransforms1;
            public Matrix4x3 NormalBasisTransforms2;
            public Matrix4x3 NormalBasisTransforms3;
            public Matrix4x3 NormalBasisTransforms4;
            public Matrix4x3 NormalBasisTransforms5;
            public Matrix4x3 NormalBasisTransforms6;
            public Vector4 LightProbeIntensity;

            public static float ComputeEV100(float aperture, float shutterTime, float ISO)
            {
                return (float)Math.Log((aperture * aperture) / shutterTime * 100 / ISO, 2);
            }

            public static float ConvertEV100ToExposure(float EV100)
            {
                float maxLuminance = 1.2f * (float)Math.Pow(2.0f, EV100);
                return 1.0f / maxLuminance;
            }

            public static float ComputeEV100FromAvgLuminance(float avgLuminance)
            {
                return (float)Math.Log(avgLuminance * 100.0f / 12.5f, 2);
            }

            //public static Vector2 ComputeExposure(float inAperture, float inShutterSpeed, float inISO)
            //{
            //    float exposure = ConvertEV100ToExposure(ComputeEV100(inAperture, inShutterSpeed, inISO));
            //    Vector2 outExposure = new Vector2();

            //    outExposure.X = exposure;
            //    outExposure.Y = 1.0f / exposure;

            //    return outExposure;
            //}

            public static Vector2 ComputeExposure(float avgLuminance, float min, float max)
            {
                float minEV100 = min;
                float maxEV100 = max;

                float EV100 = ComputeEV100FromAvgLuminance(avgLuminance);

                if (EV100 < minEV100) EV100 = minEV100;
                if (EV100 > maxEV100) EV100 = maxEV100;

                float exposure = ConvertEV100ToExposure(EV100);
                Vector2 outExposure = new Vector2
                {
                    X = avgLuminance,
                    Y = 1.0f / avgLuminance
                };


                return outExposure;
            }
        }

        protected struct LightConstants
        {
            public Vector4 LightPosAndInvSqrRadius;
            public Vector4 LightColorAndIntensity;
        }

        protected struct FunctionConstants
        {
            public Matrix WorldMatrix;
            public Vector4 LightProbe1;
            public Vector4 LightProbe2;
            public Vector4 LightProbe3;
            public Vector4 LightProbe4;
            public Vector4 LightProbe5;
            public Vector4 LightProbe6;
            public Vector4 LightProbe7;
            public Vector4 LightProbe8;
            public Vector4 LightProbe9;
        }

        protected struct CubeMapConstants
        {
            public int CubeFace;
            public uint MipIndex;
            public uint NumMips;
            public uint Pad;
        }

        protected struct TableLookupConstants
        {
            public float LutSize;
            public float FlipY;
            public Vector2 Pad;
        }
        #endregion

        /// <summary>
        /// The list of meshes to be rendered in the next frame
        /// </summary>
        protected List<MeshRenderInstance> meshes;
        protected List<MeshRenderInstance> editorMeshes;
        protected List<LightRenderInstance> lights;

        /// <summary>
        /// The collection of GBuffers
        /// </summary>
        protected GBufferCollection gBufferCollection;

        // various libraries
        protected TextureLibrary textureLibrary;
        protected ShaderLibrary shaderLibrary;

        // constant buffers
        protected ConstantBuffer<ViewConstants> viewConstants;
        protected ConstantBuffer<FunctionConstants> functionConstants;
        protected ConstantBuffer<CommonConstants> commonConstants;
        protected ConstantBuffer<LightConstants> lightConstants;
        protected ConstantBuffer<CubeMapConstants> cubeMapConstants;
        protected ConstantBuffer<TableLookupConstants> lookupTableConstants;
        protected SharpDX.Direct3D11.Buffer postProcessConstants;

        // resources
        protected BindableTexture normalBasisCubemapTexture;
        protected BindableTexture lightAccumulationTexture;
        protected BindableTexture preintegratedDFGTexture;
        protected BindableCubeTexture preintegratedDLDTexture;
        protected BindableCubeTexture preintegratedSLDTexture;
        protected BindableTexture scaledSceneTexture;
        protected BindableTexture[] toneMapTextures = new BindableTexture[7];
        protected BindableTexture postProcessTexture;
        protected BindableTexture editorCompositeTexture;
        protected BindableDepthTexture editorCompositeDepthTexture;
        protected BindableTexture finalColorTexture;
        protected BindableDepthTexture selectionDepthTexture;
        protected BindableTexture selectionOutlineTexture;
        protected BindableTexture worldNormalsForHBAOTexture;
        protected BindableTexture brightPassTexture;
        protected BindableTexture blurTexture;
        protected BindableTexture bloomSourceTexture;
        protected BindableTexture[] bloomTextures = new BindableTexture[3];

        // light shaders
        protected PixelShader psSunLight;
        protected PixelShader psPointLight;
        protected PixelShader psSphereLight;

        // IBL shaders
        protected PixelShader psIntegrateDFG;
        protected PixelShader psIntegrateDiffuseLD;
        protected PixelShader psIntegrateSpecularLD;
        protected PixelShader psIBLRender;

        // utility shaders
        protected VertexShader vsFullscreenQuad;
        protected PixelShader psResolve;
        protected PixelShader psResolveDepthToMsaa;
        protected PixelShader psResolveWorldNormals;

        // post processing shaders
        protected PixelShader psDownscale4x4;
        protected PixelShader psSampleLumInitial;
        protected PixelShader psSampleLumIterative;
        protected PixelShader psSampleLumFinal;
        protected PixelShader psCalcAdaptedLum;
        protected PixelShader psLookupTable;
        protected PixelShader psEditorComposite;
        protected PixelShader psSelectionOutline;
        protected PixelShader psDebugRenderMode;
        protected PixelShader psBrightPass;
        protected PixelShader psGaussianBlur5x5;
        protected PixelShader psDownSample2x2;
        protected PixelShader psBloomBlur;
        protected PixelShader psRenderBloom;

        // txaa
        protected IntPtr txaaContext;
        protected IntPtr txaaMotionVectorGenerator;

        protected BindableTexture txaaMotionVectorsTexture;
        protected BindableTexture txaaFeedbackTeture;

        // shadows
        protected GFSDK_ShadowLib.Context shadowContext;
        protected GFSDK_ShadowLib.Map shadowMapHandle;
        protected GFSDK_ShadowLib.Buffer shadowBufferHandle;
        protected ShaderResourceView shadowSRV;

        // hbao
        protected GFSDK_SSAO.Context hbaoContext;

        #region -- Temporary Stuff --

        // everything here is mainly here for testing purposes and may be completely removed

        protected RenderCreateState RenderCreateState => new RenderCreateState(Viewport.Device, textureLibrary, shaderLibrary);

        public DXUT.BaseCamera camera;

        public float CameraAperture { get; set; } = 16.0f;
        public float CameraShutterSpeed { get; set; } = 1 / 100.0f;
        public float CameraISO { get; set; } = 100.0f;

        public Vector3 SunPosition { get; set; } = new Vector3(10, 20, 20);
        public float SunIntensity { get; set; } = 1000.0f;
        public float SunAngularRadius { get; set; } = 0.029f;

        public ShaderResourceView DistantLightProbe
        {
            get => distantLightProbe;
            set
            {
                distantLightProbe = value;
                if (value == null)
                    distantLightProbe = defaultDistantLightProbe;
                bRecalculateLightProbe = true;
            }
        }
        public float LightProbeIntensity { get; set; } = 1.0f;
        public ShaderResourceView LookupTable { get; set; }
        public Vector4[] SHLightProbe { get; set; } = new Vector4[9];
        public DebugRenderMode RenderMode { get; set; }
        public bool GroundVisible { get; set; } = true;
        public bool GridVisible { get; set; } = true;
        public float MinEV100 { get; set; } = 8.0f;
        public float MaxEV100 { get; set; } = 20.0f;

        private ShaderResourceView distantLightProbe;
        private ShaderResourceView defaultDistantLightProbe;
        private bool bRecalculateLightProbe;

        private MeshRenderShape skySphere;
        private MeshRenderShape groundBox;
        private MeshRenderShape gridPlane;

        private Histogram luminanceHistogram = new Histogram(1);
        private double totalTime = 0.0;
        private double lastDeltaTime = 0.0;

        private const float NearPlane = 0.1f;
        private const float FarPlane = 1000000.0f;

        public int iDepthBias { get; set; } = 100;
        public float fSlopeScaledDepthBias { get; set; } = 5;
        public float fDistanceBiasMin { get; set; } = 0.00000001f;
        public float fDistanceBiasFactor { get; set; } = 0.00000001f;
        public float fDistanceBiasThreshold { get; set; } = 700.0f;
        public float fDistanceBiasPower { get; set; } = 0.3f;

        public bool ShadowsEnabled;
        public bool HBAOEnabled;
        public bool TXAAEnabled;

        #endregion

#if FROSTY_DEVELOPER
        protected enum RenderDocCaptureState
        {
            NotStarted,
            BeginCapture,
            CaptureInProgress
        }

        /// <summary>
        /// Pointer to renderdoc api (can be null if dll not present)
        /// </summary>
        private RenderDoc.Api renderDocApi;
        protected RenderDocCaptureState renderDocCaptureState;
#endif

        // default costructor
        public DeferredRenderScreen2()
        {
#if FROSTY_DEVELOPER
            InitializeRenderDoc();
#endif

            GroundVisible = Config.Get<bool>("MeshSetViewerShowFloor", true);
            GridVisible = Config.Get<bool>("MeshSetViewerShowGrid", true);
            //GroundVisible = Config.Get<bool>("MeshViewer", "ShowFloor", true);
            //GridVisible = Config.Get<bool>("MeshViewer", "ShowGrid", true);
        }

        #region -- Creation --
        /// <summary>
        /// Creates all buffers that are dependent on viewport size
        /// </summary>
        public override void CreateSizeDependentBuffers()
        {
            // initialize the gbuffers
            gBufferCollection = new GBufferCollection(Viewport.Device, Viewport.ViewportWidth, Viewport.ViewportHeight, new GBufferDescription[]
            {
                new GBufferDescription() { Format = SharpDX.DXGI.Format.R10G10B10A2_UNorm, ClearColor = new Color4(0,0,0,0), DebugName = "GBufferA" },
                new GBufferDescription() { Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb, ClearColor = new Color4(0,0,0,0), DebugName = "GBufferB" },
                new GBufferDescription() { Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm, ClearColor = new Color4(0,0,0,0), DebugName = "GBufferC" },
                new GBufferDescription() { Format = SharpDX.DXGI.Format.R16G16B16A16_Float, ClearColor = new Color4(0,0,0,0), DebugName = "GBufferD" },
            });
            finalColorTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);
            lightAccumulationTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            int scaledWidth = (Viewport.ViewportWidth - (Viewport.ViewportWidth % 8)) / 4;
            if (scaledWidth < 1)
                scaledWidth = 1;
            int scaledHeight = (Viewport.ViewportHeight - (Viewport.ViewportHeight % 8)) / 4;
            if (scaledHeight < 1)
                scaledHeight = 1;

            scaledSceneTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Width = scaledWidth,
                Height = scaledHeight,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            // bloom
            brightPassTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Width = scaledWidth,
                Height = scaledHeight,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);
            blurTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Width = scaledWidth,
                Height = scaledHeight,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default

            }, true, true);

            scaledWidth = (Viewport.ViewportWidth - (Viewport.ViewportWidth % 8)) / 8;
            if (scaledWidth < 1)
                scaledWidth = 1;
            scaledHeight = (Viewport.ViewportHeight - (Viewport.ViewportHeight % 8)) / 8;
            if (scaledHeight < 1)
                scaledHeight = 1;

            bloomSourceTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Width = scaledWidth,
                Height = scaledHeight,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            for (int i = 0; i < 3; i++)
            {
                bloomTextures[i] = new BindableTexture(Viewport.Device, new Texture2DDescription()
                {
                    ArraySize = 1,
                    Width = scaledWidth,
                    Height = scaledHeight,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                    Usage = ResourceUsage.Default
                }, true, true);
            }

            postProcessTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            // txaa
            txaaMotionVectorsTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R16G16_Float,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);
            txaaFeedbackTeture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, false);

            // editor composite
            editorCompositeTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(4, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            // editor MSAA depth buffer
            editorCompositeDepthTexture = new BindableDepthTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R24G8_Typeless,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(4, 0),
                Usage = ResourceUsage.Default
            }, true,
            new DepthStencilViewDescription()
            {
                Dimension = DepthStencilViewDimension.Texture2DMultisampled,
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource()
            },
            new ShaderResourceViewDescription()
            {
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DMultisampled,
                Format = SharpDX.DXGI.Format.R24_UNorm_X8_Typeless,
                Texture2DMS = new ShaderResourceViewDescription.Texture2DMultisampledResource()
            });

            // for drawing selection outlines
            selectionOutlineTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);
            selectionDepthTexture = new BindableDepthTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R24G8_Typeless,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true,
            new DepthStencilViewDescription()
            {
                Dimension = DepthStencilViewDimension.Texture2D,
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                Texture2D = new DepthStencilViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            },
            new ShaderResourceViewDescription()
            {
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DMultisampled,
                Format = SharpDX.DXGI.Format.R24_UNorm_X8_Typeless,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });

            // world normals for HBAO
            worldNormalsForHBAOTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Width = Viewport.ViewportWidth,
                Height = Viewport.ViewportHeight,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            if (ShadowsEnabled)
            {
                // resize shadow screen dependent buffers
                GFSDK_ShadowLib.InitSizeDependent(shadowContext, Viewport.ViewportWidth, Viewport.ViewportHeight, ref shadowMapHandle, ref shadowBufferHandle);
            }

            // update camera (for when returning from other tabs)
            camera?.SetProjParams(90.0f * ((float)Math.PI / 360.0f), Viewport.ViewportWidth / (float)Viewport.ViewportHeight, NearPlane, FarPlane);

            // clear the average luminance on return (this ensures that the luminance is not
            // skewed to a really dark value when switching between tabs)
            toneMapTextures[5]?.Clear(Viewport.Context, new Color4(0.00177f, 0, 0, 0));
        }

        /// <summary>
        /// Creates all other buffers
        /// </summary>
        public override void CreateBuffers()
        {
            ShadowsEnabled = Config.Get<bool>("RenderShadowsEnabled", true);
            HBAOEnabled = Config.Get<bool>("RenderHBAOEnabled", true);
            TXAAEnabled = Config.Get<bool>("RenderTXAAEnabled", true);
            //ShadowsEnabled = Config.Get<bool>("Render", "ShadowsEnabled", true);
            //HBAOEnabled = Config.Get<bool>("Render", "HBAOEnabled", true);
            //TXAAEnabled = Config.Get<bool>("Render", "TXAAEnabled", true);


            // initialize the libraries
            textureLibrary = new TextureLibrary(Viewport.Device);
            shaderLibrary = new ShaderLibrary(Shader.CreateFallback(Viewport.Device));

            // constant buffers
            viewConstants = new ConstantBuffer<ViewConstants>(Viewport.Device, new ViewConstants());
            functionConstants = new ConstantBuffer<FunctionConstants>(Viewport.Device, new FunctionConstants());
            commonConstants = new ConstantBuffer<CommonConstants>(Viewport.Device, new CommonConstants());
            lightConstants = new ConstantBuffer<LightConstants>(Viewport.Device, new LightConstants());
            cubeMapConstants = new ConstantBuffer<CubeMapConstants>(Viewport.Device, new CubeMapConstants());
            lookupTableConstants = new ConstantBuffer<TableLookupConstants>(Viewport.Device, new TableLookupConstants());
            postProcessConstants = new SharpDX.Direct3D11.Buffer(Viewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 32 * 4 * 4,
                StructureByteStride = 0,
                Usage = ResourceUsage.Dynamic
            });

            // shaders
            psSunLight = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "SunLight");
            psPointLight = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "PointLight");
            psSphereLight = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "SphereLight");

            vsFullscreenQuad = FrostyShaderDb.GetShader<VertexShader>(Viewport.Device, "FullscreenQuad");
            psResolve = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "Resolve");
            psResolveDepthToMsaa = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "ResolveDepthToMsaa");
            psResolveWorldNormals = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "ResolveWorldNormals");

            psIntegrateDFG = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "IBL_IntegrateDFG");
            psIntegrateDiffuseLD = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "IBL_IntegrateDiffuseLD");
            psIntegrateSpecularLD = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "IBL_IntegrateSpecularLD");
            psIBLRender = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "IBL_Main");

            psDownscale4x4 = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "DownScale4x4");
            psSampleLumInitial = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "SampleLumInitial");
            psSampleLumIterative = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "SampleLumIterative");
            psSampleLumFinal = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "SampleLumFinal");
            psCalcAdaptedLum = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "CalculateAdaptedLum");
            psLookupTable = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "LookupTable");
            psEditorComposite = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "EditorComposite");
            psSelectionOutline = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "SelectionOutline");
            psDebugRenderMode = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "DebugRenderMode");
            psBrightPass = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "BrightPass");
            psGaussianBlur5x5 = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "GaussianBlur5x5");
            psDownSample2x2 = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "DownSample2x2");
            psBloomBlur = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "BloomBlur");
            psRenderBloom = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "RenderBloom");

            // resources
            preintegratedDFGTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                Width = 128,
                Height = 128,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);
            preintegratedDLDTexture = new BindableCubeTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 6,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                Height = 32,
                Width = 32,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.TextureCube,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);
            preintegratedSLDTexture = new BindableCubeTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 6,
                Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                Height = 256,
                Width = 256,
                MipLevels = 9,
                OptionFlags = ResourceOptionFlags.TextureCube,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            }, true, true);

            // tonemaps
            int sampleLen = 0;
            for (int i = 0; i < 6; i++)
            {
                sampleLen = 1 << (2 * i);
                if (i >= 4)
                    sampleLen = 1;

                toneMapTextures[i] = new BindableTexture(Viewport.Device, new Texture2DDescription()
                {
                    ArraySize = 1,
                    Format = SharpDX.DXGI.Format.R32_Float,
                    Height = sampleLen,
                    MipLevels = 1,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    Width = sampleLen
                }, true, true);
            }
            toneMapTextures[5].Clear(Viewport.Context, new Color4(0.00177f, 0, 0, 0));

            // staging texture for luminance gathering
            toneMapTextures[6] = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R32_Float,
                Height = sampleLen,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
                Width = sampleLen,
                CpuAccessFlags = CpuAccessFlags.Read,
            }, false, false);

            if (TXAAEnabled)
            {
                // initialise TXAA
                GFSDK_TXAA.Init(Viewport.Device, ref txaaContext, ref txaaMotionVectorGenerator);
            }

            if (ShadowsEnabled)
            {
                // initialize ShadowLib
                GFSDK_ShadowLib.Init(Viewport.Device, Viewport.Context, Viewport.ViewportWidth, Viewport.ViewportHeight, ref shadowContext, ref shadowMapHandle, ref shadowBufferHandle);
            }

            if (HBAOEnabled)
            {
                // initialize HBAO
                GFSDK_SSAO.Init(Viewport.Device, ref hbaoContext);
            }

            normalBasisCubemapTexture = new BindableTexture(Viewport.Device, new Texture2DDescription()
            {
                ArraySize = 6,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Height = 1,
                MipLevels = 1,
                Width = 1,
                OptionFlags = ResourceOptionFlags.TextureCube,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default

            }, true, false);

            uint[] values = new uint[]
            {
                0x00000000,
                0x01010101,
                0x02020202,
                0x03030303,
                0x04040404,
                0x05050505
            };
            GCHandle handle = GCHandle.Alloc(values, GCHandleType.Pinned);

            for (int i = 0; i < 6; i++)
            {
                int subResourceId = normalBasisCubemapTexture.Texture.CalculateSubResourceIndex(0, i, out int rowPitch);

                IntPtr bufferPtr = handle.AddrOfPinnedObject();
                bufferPtr += (i * 4);

                DataBox box = new DataBox(bufferPtr, rowPitch, 0);
                Viewport.Device.ImmediateContext.UpdateSubresource(box, normalBasisCubemapTexture.Texture, subResourceId);
            }

            handle.Free();

            skySphere = MeshRenderShape.CreateSphere(RenderCreateState, "SkySphere","Skybox", 200000.0f, 32);
            groundBox = MeshRenderShape.CreateCube(RenderCreateState, "GroundBox", "GroundPlane", 1, 1, 1);
            gridPlane = MeshRenderShape.CreatePlane(RenderCreateState, "Grid", "Grid", 1, 1);

            // load in a default light probe cubemap
            defaultDistantLightProbe = textureLibrary.LoadTextureAsset("Resources/Textures/DefaultLightProbe.dds", true);
            DistantLightProbe = defaultDistantLightProbe;

            camera?.SetProjParams(90.0f * ((float)Math.PI / 360.0f), Viewport.ViewportWidth / (float)Viewport.ViewportHeight, NearPlane, FarPlane);
        }
        #endregion

        #region -- Update/Render --
        /// <summary>
        /// Called once a frame to perform any update steps like animation, etc.
        /// </summary>
        public override void Update(double timestep)
        {
            GFSDK_TXAA.Update(Viewport.ViewportWidth, Viewport.ViewportHeight);
            camera.FrameMove((float)(timestep));
            totalTime += timestep;
            lastDeltaTime = timestep;
        }

        /// <summary>
        /// Performs the actual render to screen
        /// </summary>
        public override void Render()
        {
            GFSDK_TXAA.TxaaEnabled = RenderMode == DebugRenderMode.Default && TXAAEnabled;

            BeginFrameActions();
            {
                // collect the meshes and lights to be rendered this frame
                meshes = CollectMeshInstances();
                lights = CollectLightInstances();

                // add in sky sphere and ground plane
                //meshes.Add(new MeshRenderInstance() { RenderMesh = skySphere, Transform = Matrix.Identity });
                if (GroundVisible)
                    meshes.Add(new MeshRenderInstance() { RenderMesh = groundBox, Transform = Matrix.Scaling(8, 0.25f, 8) * Matrix.Translation(0, -0.125f, 0) });

                // add grid to editor meshes
                editorMeshes = new List<MeshRenderInstance>();
                if (GridVisible)
                    editorMeshes.Add(new MeshRenderInstance() { RenderMesh = gridPlane, Transform = Matrix.Translation(0, (GroundVisible) ? -0.125f : 0.0f, 0) });

                {
                    GFSDK_TXAA.GetJitter(out float[] jitter);

                    // update the view constants
                    UpdateViewConstants(true);

                    // update the common constants
                    Matrix invProjMatrix = camera.GetProjMatrix();
                    Matrix invViewProjMatrix = camera.GetViewProjMatrix();

                    invProjMatrix.Invert();
                    invProjMatrix.Transpose();
                    invViewProjMatrix.Invert();
                    invViewProjMatrix.Transpose();

                    Matrix4x3[] normalBasisTransforms = new Matrix4x3[6]
                    {
                        new Matrix4x3(new float[] { 0, 0, -1, 0, 0, -1, 0, 0, -1, 0, 0, 0 }),
                        new Matrix4x3(new float[] { 0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0 }),
                        new Matrix4x3(new float[] { -1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0 }),
                        new Matrix4x3(new float[] { -1, 0, 0, 0, 0, 0, -1, 0, 0, -1, 0, 0 }),
                        new Matrix4x3(new float[] { -1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0 }),
                        new Matrix4x3(new float[] { 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, -1, 0 })
                    };

                    commonConstants.UpdateData(Viewport.Context, new CommonConstants()
                    {
                        InvViewProjMatrix = invViewProjMatrix,
                        InvProjMatrix = invProjMatrix,
                        CameraPos = new Vector4(camera.GetEyePt() * new Vector3(-1,1,1), (float)RenderMode),
                        InvScreenSize = new Vector4(1.0f / Viewport.ViewportWidth, 1.0f / Viewport.ViewportHeight, Viewport.ViewportWidth, Viewport.ViewportHeight),
                        ExposureMultipliers = new Vector4(CommonConstants.ComputeExposure(luminanceHistogram.GetAverage(), MinEV100, MaxEV100), MinEV100, MaxEV100),

                        NormalBasisTransforms1 = normalBasisTransforms[0],
                        NormalBasisTransforms2 = normalBasisTransforms[1],
                        NormalBasisTransforms3 = normalBasisTransforms[2],
                        NormalBasisTransforms4 = normalBasisTransforms[3],
                        NormalBasisTransforms5 = normalBasisTransforms[4],
                        NormalBasisTransforms6 = normalBasisTransforms[5],

                        LightProbeIntensity = new Vector4(LightProbeIntensity, 0, 0, 0)
                    });
                }

                ClearRenderTargets();
                if (bRecalculateLightProbe)
                {
                    PreintegrateIBL();
                    CalculateSphericalHarmonics();
                    bRecalculateLightProbe = false;
                }
                RenderBasePass();
                RenderShadows();
                RenderLights();
                RenderIBL();
                ResolveNormalsForHBAO();
                RenderEmissive();
                PostProcess();
                Resolve();
            }
            EndFrameActions();
        }

        public virtual List<MeshRenderInstance> CollectMeshInstances()
        {
            return new List<MeshRenderInstance>();
        }

        public virtual List<LightRenderInstance> CollectLightInstances()
        {
            return new List<LightRenderInstance>();
        }

        protected virtual void UpdateViewConstants(bool bJitter)
        {
            Matrix4x3[] normalBasisTransforms = new Matrix4x3[6]
            {
                new Matrix4x3(new float[] { 0, 0, 1, 0, 0, -1, 0, 0, -1, 0, 0, 0 }),
                new Matrix4x3(new float[] { 0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0 }),
                new Matrix4x3(new float[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0 }),
                new Matrix4x3(new float[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0 }),
                new Matrix4x3(new float[] { 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0 }),
                new Matrix4x3(new float[] { -1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0 })
            };

            Matrix viewMatrix = camera.GetViewMatrix();
            viewMatrix.Transpose();
            Matrix projMatrix = camera.GetProjMatrix();
            projMatrix.Transpose();
            Matrix viewProjMatrix = camera.GetViewProjMatrix();
            viewProjMatrix.Transpose();
            Matrix crViewProjMatrix = camera.GetCrViewProjMatrix();
            if (bJitter)
            {
                GFSDK_TXAA.GetJitter(out float[] jitter);

                crViewProjMatrix = camera.GetCrViewProjMatrix(jitter);
            }
            crViewProjMatrix.Transpose();

            viewConstants.UpdateData(Viewport.Context, new ViewConstants()
            {
                Time = new Vector4((float)totalTime, 0, 0, 0),
                ScreenSize = new Vector4(Viewport.ViewportWidth, Viewport.ViewportHeight, 1.0f / Viewport.ViewportWidth, 1.0f / Viewport.ViewportHeight),

                ViewMatrix = viewMatrix,
                ProjMatrix = projMatrix,
                ViewProjMatrix = viewProjMatrix,
                CrViewProjMatrix = crViewProjMatrix,
                NormalBasisTransforms1 = normalBasisTransforms[0],
                NormalBasisTransforms2 = normalBasisTransforms[1],
                NormalBasisTransforms3 = normalBasisTransforms[2],
                NormalBasisTransforms4 = normalBasisTransforms[3],
                NormalBasisTransforms5 = normalBasisTransforms[4],
                NormalBasisTransforms6 = normalBasisTransforms[5],
                ExposureMultipliers = new Vector4(CommonConstants.ComputeExposure(luminanceHistogram.GetAverage(), MinEV100, MaxEV100), MinEV100, MaxEV100),
                CameraPos = new Vector4(camera.GetEyePt(), 1.0f),
            });
        }
        #endregion

        #region -- Input --
        public override void MouseMove(int x, int y)
        {
            camera?.MouseMove(x, y);
        }

        public override void MouseDown(int x, int y, Frosty.Core.Viewport.MouseButton button)
        {
            camera?.MouseButtonDown(x, y, button);
        }

        public override void MouseUp(int x, int y, Frosty.Core.Viewport.MouseButton button)
        {
            camera?.MouseButtonUp(button);
        }

        public override void MouseScroll(int delta)
        {
            camera?.MouseWheel(delta);
        }

        public override void KeyDown(int key)
        {
            camera?.KeyDown((Key)key);
        }

        public override void KeyUp(int key)
        {
            camera?.KeyUp((Key)key);

#if FROSTY_DEVELOPER
            if ((Key)key == Key.F12)
            {
                CaptureNextFrame();
            }
#endif
        }
        #endregion

        #region -- Dispose --
        /// <summary>
        /// Dispose of any buffers dependent on viewport size, this is called when the viewport
        /// changes sizes or is closed
        /// </summary>
        public override void DisposeSizeDependentBuffers()
        {
            gBufferCollection.Dispose();
            lightAccumulationTexture.Dispose();
            scaledSceneTexture.Dispose();
            postProcessTexture.Dispose();
            txaaFeedbackTeture.Dispose();
            txaaMotionVectorsTexture.Dispose();
            editorCompositeDepthTexture.Dispose();
            editorCompositeTexture.Dispose();
            finalColorTexture.Dispose();
            selectionDepthTexture.Dispose();
            worldNormalsForHBAOTexture.Dispose();
            brightPassTexture.Dispose();
            blurTexture.Dispose();
            bloomSourceTexture.Dispose();

            foreach (BindableTexture texture in bloomTextures)
                texture.Dispose();

            if (ShadowsEnabled)
            {
                shadowContext.RemoveMap(ref shadowMapHandle);
                shadowContext.RemoveBuffer(ref shadowBufferHandle);
            }
        }

        /// <summary>
        /// Dispose of all buffers not viewport dependent
        /// </summary>
        public override void DisposeBuffers()
        {
            textureLibrary.Dispose();
            shaderLibrary.Dispose();

            viewConstants.Dispose();
            functionConstants.Dispose();
            commonConstants.Dispose();
            lightConstants.Dispose();
            postProcessConstants.Dispose();
            cubeMapConstants.Dispose();
            lookupTableConstants.Dispose();

            psPointLight.Dispose();
            psSunLight.Dispose();
            psSphereLight.Dispose();

            vsFullscreenQuad.Dispose();
            psResolve.Dispose();
            psResolveDepthToMsaa.Dispose();
            psResolveWorldNormals.Dispose();

            psIntegrateDFG.Dispose();
            psIntegrateDiffuseLD.Dispose();
            psIntegrateSpecularLD.Dispose();
            psIBLRender.Dispose();

            psDownscale4x4.Dispose();
            psSampleLumInitial.Dispose();
            psSampleLumIterative.Dispose();
            psSampleLumFinal.Dispose();
            psCalcAdaptedLum.Dispose();
            psLookupTable.Dispose();
            psEditorComposite.Dispose();
            psSelectionOutline.Dispose();
            psDebugRenderMode.Dispose();
            psBrightPass.Dispose();
            psGaussianBlur5x5.Dispose();
            psDownSample2x2.Dispose();
            psBloomBlur.Dispose();
            psRenderBloom.Dispose();

            normalBasisCubemapTexture.Dispose();
            preintegratedDFGTexture.Dispose();
            preintegratedDLDTexture.Dispose();
            preintegratedSLDTexture.Dispose();

            for (int i = 0; i < 7; i++)
                toneMapTextures[i].Dispose();

            if (TXAAEnabled)
                GFSDK_TXAA.Destroy(ref txaaContext, ref txaaMotionVectorGenerator);
            if (HBAOEnabled)
                hbaoContext.Release();
            if (ShadowsEnabled)
                shadowContext.Destroy();

            skySphere.Dispose();
            groundBox.Dispose();
            gridPlane.Dispose();
        }
        #endregion

        #region -- Render Stages --
        /// <summary>
        /// 
        /// </summary>
        protected virtual void CalculateSphericalHarmonics()
        {
            if (DistantLightProbe == null)
                return;

            SharpDX.Mathematics.Interop.RawViewportF[] origViewports = Viewport.Context.Rasterizer.GetViewports<SharpDX.Mathematics.Interop.RawViewportF>();

            D3DUtils.BeginPerfEvent(Viewport.Context, "Spherical Harmonics");
            {
                PixelShader ps = FrostyShaderDb.GetShader<PixelShader>(Viewport.Device, "ResolveCubeMapFace");

                Texture2DDescription desc = new Texture2DDescription()
                {
                    ArraySize = 1,
                    BindFlags = BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = SharpDX.DXGI.Format.R16G16B16A16_Float,
                    Height = preintegratedSLDTexture.Texture.Description.Height,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    Width = preintegratedSLDTexture.Texture.Description.Width
                };

                Texture2D tmpTexture = new Texture2D(Viewport.Device, desc);
                desc.CpuAccessFlags = CpuAccessFlags.Read;
                desc.BindFlags = BindFlags.None;
                desc.Usage = ResourceUsage.Staging;
                Texture2D resolveTexture = new Texture2D(Viewport.Device, desc);
                RenderTargetView tmpRtv = new RenderTargetView(Viewport.Device, tmpTexture);

                float[] resultR = new float[9];
                float[] resultG = new float[9];
                float[] resultB = new float[9];
                float[] shBuffB = new float[9];
                float weight = 0.0f;

                for (int i = 0; i < 6; i++)
                {
                    cubeMapConstants.UpdateData(Viewport.Context, new CubeMapConstants() { CubeFace = i });

                    // render out cubemap face
                    Viewport.Context.OutputMerger.SetRenderTargets(null, tmpRtv);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, desc.Width, desc.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.PixelShader.Set(ps);
                    Viewport.Context.PixelShader.SetConstantBuffer(0, cubeMapConstants.Buffer);
                    Viewport.Context.PixelShader.SetShaderResource(0, preintegratedSLDTexture.SRV);
                    Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                    Viewport.Context.Draw(6, 0);

                    // resolve to staging
                    Viewport.Context.OutputMerger.SetRenderTargets(null, new RenderTargetView[] { });
                    Viewport.Context.CopyResource(tmpTexture, resolveTexture);

                    // read staging texture
                    Viewport.Context.MapSubresource(resolveTexture, 0, MapMode.Read, MapFlags.None, out DataStream stream);
                    {
                        float invWidth = 1.0f / preintegratedSLDTexture.Texture.Description.Width;
                        float negativeBound = -1.0f + invWidth;
                        float invWidthBy2 = 2.0f / preintegratedSLDTexture.Texture.Description.Width;

                        for (int y = 0; y < preintegratedSLDTexture.Texture.Description.Height; y++)
                        {
                            float fV = negativeBound + y * invWidthBy2;
                            for (int x = 0; x < preintegratedSLDTexture.Texture.Description.Width; x++)
                            {
                                float fU = negativeBound + x * invWidthBy2;
                                Vector3 dir = Vector3.Zero;

                                switch (i)
                                {
                                    case 0: /* X+ */
                                        dir.X = 1.0f;
                                        dir.Y = 1.0f - (invWidthBy2 * y + invWidth);
                                        dir.Z = 1.0f - (invWidthBy2 * x + invWidth);
                                        dir = -dir;
                                        break;
                                    case 1: /* X- */
                                        dir.X = -1.0f;
                                        dir.Y = 1.0f - (invWidthBy2 * y + invWidth);
                                        dir.Z = -1.0f + (invWidthBy2 * x + invWidth);
                                        dir = -dir;
                                        break;
                                    case 2: /* Y+ */
                                        dir.X = -1.0f + (invWidthBy2 * x + invWidth);
                                        dir.Y = 1.0f;
                                        dir.Z = -1.0f + (invWidthBy2 * y + invWidth);
                                        dir = -dir;
                                        break;
                                    case 3: /* Y- */
                                        dir.X = -1.0f + (invWidthBy2 * x + invWidth);
                                        dir.Y = -1.0f;
                                        dir.Z = 1.0f - (invWidthBy2 * y + invWidth);
                                        dir = -dir;
                                        break;
                                    case 4: /* Z+ */
                                        dir.X = -1.0f + (invWidthBy2 * x + invWidth);
                                        dir.Y = 1.0f - (invWidthBy2 * y + invWidth);
                                        dir.Z = 1.0f;
                                        break;
                                    case 5: /* Z- */
                                        dir.X = 1.0f - (invWidthBy2 * x + invWidth);
                                        dir.Y = 1.0f - (invWidthBy2 * y + invWidth);
                                        dir.Z = -1.0f;
                                        break;
                                }

                                dir.Normalize();
                                float diffSolid = 4.0f / ((1.0f + fU * fU + fV * fV) * (float)Math.Sqrt(1.0f + fU * fU + fV * fV));
                                float[] sh = SphericalHarmonicsHelper.shEvaluateDir(dir);

                                weight += diffSolid;

                                float R = HalfUtils.Unpack(stream.Read<ushort>());
                                float G = HalfUtils.Unpack(stream.Read<ushort>());
                                float B = HalfUtils.Unpack(stream.Read<ushort>());
                                float A = HalfUtils.Unpack(stream.Read<ushort>());

                                Vector3 color = new Vector3(R, G, B);

                                shBuffB = SphericalHarmonicsHelper.shScale(sh, color.X * diffSolid);
                                resultR = SphericalHarmonicsHelper.shAdd(resultR, shBuffB);
                                shBuffB = SphericalHarmonicsHelper.shScale(sh, color.Y * diffSolid);
                                resultG = SphericalHarmonicsHelper.shAdd(resultG, shBuffB);
                                shBuffB = SphericalHarmonicsHelper.shScale(sh, color.Z * diffSolid);
                                resultB = SphericalHarmonicsHelper.shAdd(resultB, shBuffB);
                            }
                        }
                    }
                    Viewport.Context.UnmapSubresource(resolveTexture, 0);
                }

                float normProj = (4.0f * (float)Math.PI) / weight;
                resultR = SphericalHarmonicsHelper.shScale(resultR, normProj);
                resultG = SphericalHarmonicsHelper.shScale(resultG, normProj);
                resultB = SphericalHarmonicsHelper.shScale(resultB, normProj);

                for (int i = 0; i < 9; i++)
                    SHLightProbe[i] = new Vector4(resultR[i], resultG[i], resultB[i], 1.0f);

                ps.Dispose();
                tmpRtv.Dispose();
                tmpTexture.Dispose();
                resolveTexture.Dispose();
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            Viewport.Context.Rasterizer.SetViewports(origViewports);
        }

        /// <summary>
        /// Generates the preintegrated DFG, Diffuse LD, and Specular LD textures required for IBL
        /// </summary>
        protected virtual void PreintegrateIBL()
        {
            SharpDX.Mathematics.Interop.RawViewportF[] origViewports = Viewport.Context.Rasterizer.GetViewports<SharpDX.Mathematics.Interop.RawViewportF>();

            D3DUtils.BeginPerfEvent(Viewport.Context, "Preintegrate DFG");
            {
                preintegratedDFGTexture.Clear(Viewport.Context, new Color4(0, 0, 0, 0));

                Viewport.Context.OutputMerger.SetRenderTargets(null, preintegratedDFGTexture.RTV);
                Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, preintegratedDFGTexture.Texture.Description.Width, preintegratedDFGTexture.Texture.Description.Height));
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.PixelShader.Set(psIntegrateDFG);

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            if (DistantLightProbe != null)
            {
                D3DUtils.BeginPerfEvent(Viewport.Context, "Preintegrate Diffuse LD");
                {
                    for (int i = 0; i < 6; i++)
                    {
                        cubeMapConstants.UpdateData(Viewport.Context, new CubeMapConstants() { CubeFace = i });
                        preintegratedDLDTexture.Clear(Viewport.Context, i, 0, new Color4(0, 0, 0, 0));

                        Viewport.Context.OutputMerger.SetRenderTargets(null, preintegratedDLDTexture.GetRTV(i, 0));
                        Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, preintegratedDLDTexture.Texture.Description.Width, preintegratedDLDTexture.Texture.Description.Height));
                        Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                        Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                        Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                        Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                        Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                        Viewport.Context.InputAssembler.InputLayout = null;

                        Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                        Viewport.Context.PixelShader.Set(psIntegrateDiffuseLD);
                        Viewport.Context.PixelShader.SetConstantBuffers(0, cubeMapConstants.Buffer);
                        Viewport.Context.PixelShader.SetShaderResources(0, DistantLightProbe);
                        Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                        Viewport.Context.Draw(6, 0);
                    }
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "Preintegrate Specular LD");
                {
                    // generate lower level mips for specular LD
                    Viewport.Context.GenerateMips(DistantLightProbe);

                    for (int mipIdx = 0; mipIdx < 9; mipIdx++)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            cubeMapConstants.UpdateData(Viewport.Context, new CubeMapConstants() { CubeFace = i, MipIndex = (uint)mipIdx, NumMips = 9 });
                            preintegratedSLDTexture.Clear(Viewport.Context, i, mipIdx, new Color4(0, 0, 0, 0));

                            Viewport.Context.OutputMerger.SetRenderTargets(null, preintegratedSLDTexture.GetRTV(i, mipIdx));
                            Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, preintegratedSLDTexture.Texture.Description.Width >> mipIdx, preintegratedSLDTexture.Texture.Description.Height >> mipIdx));
                            Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                            Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                            Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                            Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                            Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                            Viewport.Context.InputAssembler.InputLayout = null;

                            Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                            Viewport.Context.PixelShader.Set(psIntegrateSpecularLD);
                            Viewport.Context.PixelShader.SetConstantBuffers(0, cubeMapConstants.Buffer);
                            Viewport.Context.PixelShader.SetShaderResources(0, DistantLightProbe);
                            Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                            Viewport.Context.Draw(6, 0);
                        }
                    }
                }
                D3DUtils.EndPerfEvent(Viewport.Context);
            }

            Viewport.Context.Rasterizer.SetViewports(origViewports);
        }

        /// <summary>
        /// Clears all render targets associated with this screen
        /// </summary>
        protected virtual void ClearRenderTargets()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "ClearTargets");
            {
                Viewport.Context.ClearRenderTargetView(Viewport.ColorBufferRTV, Color4.Black);
                Viewport.Context.ClearDepthStencilView(Viewport.DepthBufferDSV, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

                editorCompositeDepthTexture.Clear(Viewport.Context, true, true, 1.0f, 0);
                selectionDepthTexture.Clear(Viewport.Context, true, true, 1.0f, 0);

                gBufferCollection.Clear(Viewport.Context);
                lightAccumulationTexture.Clear(Viewport.Context, Color4.Black);
                finalColorTexture.Clear(Viewport.Context, Color4.Black);
                editorCompositeTexture.Clear(Viewport.Context, Color4.Black);
                scaledSceneTexture.Clear(Viewport.Context, Color4.Black);
                worldNormalsForHBAOTexture.Clear(Viewport.Context, Color4.Black);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// Renders the geometry into gbuffers
        /// </summary>
        protected virtual void RenderBasePass()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "BasePass");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(Viewport.DepthBufferDSV, gBufferCollection.GBufferRTVs);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(depthComparison: Comparison.LessEqual);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.Front, depthClip: true, fillMode: (RenderMode == DebugRenderMode.Wireframe) ? FillMode.Wireframe : FillMode.Solid);

                Viewport.Context.VertexShader.SetConstantBuffer(0, viewConstants.Buffer);

                Viewport.Context.PixelShader.SetConstantBuffer(0, viewConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResource(0, normalBasisCubemapTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                RenderMeshes(MeshRenderPath.Deferred, meshes);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void RenderShadows()
        {
            if (!ShadowsEnabled)
                return;

            D3DUtils.BeginPerfEvent(Viewport.Context, "Shadows");
            {
                BoundingBox aabb = CalcWorldBoundingBox();

                // account for the floor mesh
                aabb = BoundingBox.Merge(aabb, new BoundingBox(new Vector3(-4, -1, -4), new Vector3(4, 1, 4)));

                // shadow pass
                GFSDK_ShadowLib.MapRenderParams renderParams = new GFSDK_ShadowLib.MapRenderParams(true)
                {
                    LightDesc =
                    {
                        eLightType = GFSDK_ShadowLib.LightType.Directional,
                        fLightSize = 1.0f,
                        v3LightPos_1 = SunPosition,
                        v3LightPos_2 = SunPosition,
                        v3LightPos_3 = SunPosition,
                        v3LightPos_4 = SunPosition,
                        v3LightLookAt_1 = Vector3.Zero,
                        v3LightLookAt_2 = Vector3.Zero,
                        v3LightLookAt_3 = Vector3.Zero,
                        v3LightLookAt_4 = Vector3.Zero
                    },

                    m4x4EyeViewMatrix = GFSDK_ShadowLib.Matrix.FromSharpDX(camera.GetViewMatrix()),
                    m4x4EyeProjectionMatrix = GFSDK_ShadowLib.Matrix.FromSharpDX(camera.GetProjMatrix()),
                    v3WorldSpaceBBox_1 = aabb.Minimum * 1.05f,
                    v3WorldSpaceBBox_2 = aabb.Maximum * 1.05f,
                    eCullModeType = GFSDK_ShadowLib.CullModeType.Front,
                    eTechniqueType = GFSDK_ShadowLib.TechniqueType.PCF,
                    eCascadedShadowMapType = GFSDK_ShadowLib.CascadedShadowMapType.SampleDistribution,
                    fCascadeMaxDistancePercent = 50.0f,
                    fCascadeZLinearScale_1 = 0.00001f,
                    fCascadeZLinearScale_2 = 0.00002f,
                    fCascadeZLinearScale_3 = 0.00005f,
                    fCascadeZLinearScale_4 = 1.0f,

                    ZBiasParams =
                    {
                        iDepthBias = iDepthBias,
                        fSlopeScaledDepthBias = fSlopeScaledDepthBias,
                        bUseReceiverPlaneBias = 0,
                        fDistanceBiasMin = fDistanceBiasMin,
                        fDistanceBiasFactor = fDistanceBiasFactor,
                        fDistanceBiasThreshold = fDistanceBiasThreshold,
                        fDistanceBiasPower = fDistanceBiasPower
                    },

                    PCSSPenumbraParams =
                    {
                        fMaxThreshold = 247.0f,
                        fMinSizePercent_1 = 1.8f,
                        fMinSizePercent_2 = 1.8f,
                        fMinSizePercent_3 = 1.8f,
                        fMinSizePercent_4 = 1.8f,
                        fMinWeightThresholdPercent = 3.0f
                    },

                    FrustumTraceMapRenderParams =
                    {
                        eConservativeRasterType = GFSDK_ShadowLib.ConservativeRasterType.HW,
                        eCullModeType = GFSDK_ShadowLib.CullModeType.None,
                        fHitEpsilon = 0.009f
                    },

                    RayTraceMapRenderParams =
                    {
                        fHitEpsilon = 0.02f,
                        eCullModeType = GFSDK_ShadowLib.CullModeType.None,
                        eConservativeRasterType = GFSDK_ShadowLib.ConservativeRasterType.HW
                    },

                    DepthBufferDesc =
                    {
                        eDepthType = GFSDK_ShadowLib.DepthType.DepthBuffer,
                        DepthSRV = Viewport.DepthBufferSRV.NativePointer
                    }
                };

                //renderParams.DepthBufferDesc.ReadOnlyDSV = todo

                int retVal = shadowContext.SetMapRenderParams(shadowMapHandle, renderParams);
                retVal = shadowContext.UpdateMapBounds(shadowMapHandle, out GFSDK_ShadowLib.Matrix[] lightViewMatrices, out GFSDK_ShadowLib.Matrix[] lightProjMatrices, out GFSDK_ShadowLib.Frustum[] renderFrustums);

                shadowContext.InitializeMapRendering(shadowMapHandle, GFSDK_ShadowLib.MapRenderType.Depth);

                for (uint uView = 0; uView < GFSDK_ShadowLib.NumCSMLevels; uView++)
                {
                    Matrix viewMatrix = lightViewMatrices[uView].ToSharpDX();
                    Matrix projMatrix = lightProjMatrices[uView].ToSharpDX();
                    Matrix viewProjMatrix = viewMatrix * projMatrix;
                    viewProjMatrix.Transpose();

                    viewConstants.UpdateData(Viewport.Context, new ViewConstants()
                    {
                        CrViewProjMatrix = viewProjMatrix,
                    });

                    shadowContext.BeginMapRendering(shadowMapHandle, GFSDK_ShadowLib.MapRenderType.Depth, uView);
                    RenderMeshes(MeshRenderPath.Shadows, meshes);
                    shadowContext.EndMapRendering(shadowMapHandle, GFSDK_ShadowLib.MapRenderType.Depth, uView);
                }
                shadowContext.ClearBuffer(shadowBufferHandle);
                shadowContext.RenderBuffer(shadowMapHandle, shadowBufferHandle, new GFSDK_ShadowLib.BufferRenderParams());
                retVal = shadowContext.FinalizeBuffer(shadowBufferHandle, ref shadowSRV);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void RenderMeshes(MeshRenderPath renderPath, List<MeshRenderInstance> meshList)
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "RenderMeshes");
            {
                RasterizerStateDescription desc = Viewport.Context.Rasterizer.State.Description;
                foreach (MeshRenderInstance mesh in meshList)
                {
                    D3DUtils.BeginPerfEvent(Viewport.Context, mesh.RenderMesh.DebugName);
                    {
                        Matrix transform = mesh.Transform;                       
                        transform.Transpose();

                        functionConstants.UpdateData(Viewport.Context, new FunctionConstants()
                        {
                            WorldMatrix = Matrix.Scaling(-1,1,1) * transform,
                            LightProbe1 = SHLightProbe[0],
                            LightProbe2 = SHLightProbe[1],
                            LightProbe3 = SHLightProbe[2],
                            LightProbe4 = SHLightProbe[3],
                            LightProbe5 = SHLightProbe[4],
                            LightProbe6 = SHLightProbe[5],
                            LightProbe7 = SHLightProbe[6],
                            LightProbe8 = SHLightProbe[7],
                            LightProbe9 = SHLightProbe[8],
                        });

                        Viewport.Context.VertexShader.SetConstantBuffer(1, functionConstants.Buffer);
                        Viewport.Context.PixelShader.SetConstantBuffer(1, functionConstants.Buffer);

                        mesh.RenderMesh.Render(Viewport.Context, renderPath);

                        //if (renderPath == MeshRenderPath.Shadows)
                        //{
                        //    foreach (MeshRenderSection section in mesh.Lod.Sections)
                        //        shadowContext.IncrementMapPrimitiveCounter(shadowMapHandle, GFSDK_ShadowLib.MapRenderType.Depth, (uint)section.PrimitiveCount);
                        //}
                    }
                    D3DUtils.EndPerfEvent(Viewport.Context);
                }
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderLights()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "Lights");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, lightAccumulationTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(new RenderTargetBlendDescription() { IsBlendEnabled = true, SourceBlend = BlendOption.One, DestinationBlend = BlendOption.One, BlendOperation = BlendOperation.Add, SourceAlphaBlend = BlendOption.One, DestinationAlphaBlend = BlendOption.One, AlphaBlendOperation = BlendOperation.Add, RenderTargetWriteMask = ColorWriteMaskFlags.All });
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.SetConstantBuffers(0, commonConstants.Buffer, lightConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResources(0, gBufferCollection.GBufferSRVs);
                Viewport.Context.PixelShader.SetShaderResources(4, Viewport.DepthBufferSRV);

                if (SunIntensity > 0)
                {
                    lightConstants.UpdateData(Viewport.Context, new LightConstants()
                    {
                        LightColorAndIntensity = new Vector4(0, 0, 0, SunIntensity),
                        LightPosAndInvSqrRadius = new Vector4(SunPosition * new Vector3(-1, 1, 1), SunAngularRadius)
                    });

                    // directional sunlight first
                    Viewport.Context.PixelShader.SetShaderResources(5, shadowSRV);
                    Viewport.Context.PixelShader.Set(psSunLight);
                    Viewport.Context.Draw(6, 0);
                }

                // then all other lights
                foreach (LightRenderInstance light in lights)
                {
                    if (light.Intensity > 0)
                    {
                        lightConstants.UpdateData(Viewport.Context, new LightConstants()
                        {
                            LightColorAndIntensity = new Vector4(light.Color, light.Intensity),
                            LightPosAndInvSqrRadius = new Vector4(light.Transform.TranslationVector * new Vector3(-1, 1, 1), (light.SphereRadius > 0) ? light.SphereRadius : (1.0f / (float)(light.AttenuationRadius * light.AttenuationRadius)))
                        });

                        Viewport.Context.PixelShader.Set((light.SphereRadius > 0) ? psSphereLight : psPointLight);
                        Viewport.Context.Draw(6, 0);
                    }
                }
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderIBL()
        {
            if (DistantLightProbe == null)
                return;

            D3DUtils.BeginPerfEvent(Viewport.Context, "IBL");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, lightAccumulationTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(new RenderTargetBlendDescription() { IsBlendEnabled = true, SourceBlend = BlendOption.One, DestinationBlend = BlendOption.One, BlendOperation = BlendOperation.Add, SourceAlphaBlend = BlendOption.One, DestinationAlphaBlend = BlendOption.One, AlphaBlendOperation = BlendOperation.Add, RenderTargetWriteMask = ColorWriteMaskFlags.All });
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psIBLRender);
                Viewport.Context.PixelShader.SetConstantBuffers(0, commonConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResources(0, gBufferCollection.GBufferSRVs);
                Viewport.Context.PixelShader.SetShaderResources(4, Viewport.DepthBufferSRV, preintegratedDFGTexture.SRV, preintegratedDLDTexture.SRV, preintegratedSLDTexture.SRV, DistantLightProbe);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));
                Viewport.Context.PixelShader.SetSampler(1, D3DUtils.CreateSamplerState(address: TextureAddressMode.Wrap, filter: Filter.MinMagMipLinear));

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResolveNormalsForHBAO()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "ResolveNormalsForHBAO");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, worldNormalsForHBAOTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psResolveWorldNormals);
                Viewport.Context.PixelShader.SetConstantBuffers(0, commonConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResources(0, gBufferCollection.GBufferSRVs);
                Viewport.Context.PixelShader.SetShaderResources(4, Viewport.DepthBufferSRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderEmissive()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "Emissive");
            {
                UpdateViewConstants(true);

                Viewport.Context.OutputMerger.SetRenderTargets(Viewport.DepthBufferDSV, lightAccumulationTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(depthComparison: Comparison.LessEqual);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.Front, depthClip: true);

                Viewport.Context.VertexShader.SetConstantBuffer(0, viewConstants.Buffer);

                Viewport.Context.PixelShader.SetConstantBuffer(0, viewConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResource(0, normalBasisCubemapTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                RenderMeshes(MeshRenderPath.Deferred, new List<MeshRenderInstance>() { new MeshRenderInstance() { RenderMesh = skySphere, Transform = Matrix.Identity } });
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcess()
        {
            SharpDX.Mathematics.Interop.RawViewportF[] origViewports = Viewport.Context.Rasterizer.GetViewports<SharpDX.Mathematics.Interop.RawViewportF>();

            D3DUtils.BeginPerfEvent(Viewport.Context, "PostProcess");
            {
                PostProcessCollectSelections();
                PostProcessEditorPrimitives();
                PostProcessHBAO();
                PostProcessTAA();
                PostProcessDownScaleScene();
                PostProcessMeasureLuminance();
                PostProcessBloom();
                PostProcessColorLookupTable();
                PostProcessSelectionOutline();
                PostProcessEditorComposite();
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            Viewport.Context.Rasterizer.SetViewports(origViewports);
        }

        /// <summary>
        /// Resolve from light accumulation to final render target
        /// </summary>
        private void Resolve()
        {
            if (RenderMode == DebugRenderMode.Default || RenderMode == DebugRenderMode.HBAO)
                return;

            D3DUtils.BeginPerfEvent(Viewport.Context, "DebugRenderMode");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, Viewport.ColorBufferRTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psDebugRenderMode);
                Viewport.Context.PixelShader.SetShaderResources(0, gBufferCollection.GBufferSRVs);
                Viewport.Context.PixelShader.SetShaderResources(4, Viewport.DepthBufferSRV);
                Viewport.Context.PixelShader.SetConstantBuffer(0, commonConstants.Buffer);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            Viewport.Context.OutputMerger.SetRenderTargets(null, new RenderTargetView[5]);
        }

        #region -- Post Processing --
        /// <summary>
        /// 
        /// </summary>
        private void PostProcessTAA()
        {
            if (GFSDK_TXAA.TxaaEnabled)
            {
                D3DUtils.BeginPerfEvent(Viewport.Context, "TXAA");
                {
                    D3DUtils.BeginPerfEvent(Viewport.Context, "CameraMotionVectors");
                    {
                        // TXAA Camera motion vectors
                        Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(null, 0, 0));
                        Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                        txaaMotionVectorsTexture.Clear(Viewport.Context, new Color4(0, 0, 0, 0));

                        Matrix viewProjMatrix = camera.GetViewProjMatrix();
                        viewProjMatrix.Transpose();
                        Matrix prevViewProjMatrix = camera.GetPrevViewProjMatrix();
                        prevViewProjMatrix.Transpose();

                        IntPtr ptr1 = Marshal.AllocHGlobal(64);
                        IntPtr ptr2 = Marshal.AllocHGlobal(64);

                        Marshal.Copy(viewProjMatrix.ToArray(), 0, ptr1, 4 * 4);
                        Marshal.Copy(prevViewProjMatrix.ToArray(), 0, ptr2, 4 * 4);

                        GFSDK_TXAA.MotionVectorParameters mvParams = new GFSDK_TXAA.MotionVectorParameters
                        {
                            viewProj = ptr1,
                            prevViewProj = ptr2,
                            samples = 1
                        };

                        IntPtr motionGeneratorVtbl = Marshal.ReadIntPtr(Marshal.ReadIntPtr(txaaMotionVectorGenerator, 0), 0);
                        GFSDK_TXAA.GenerateMotionVectorFunc generateMotionVector = Marshal.GetDelegateForFunctionPointer<GFSDK_TXAA.GenerateMotionVectorFunc>(Marshal.ReadIntPtr(motionGeneratorVtbl, 1 * 8));
                        int retVal = generateMotionVector(Marshal.ReadIntPtr(txaaMotionVectorGenerator), Viewport.Context.NativePointer, txaaMotionVectorsTexture.RTV.NativePointer, Viewport.DepthBufferSRV.NativePointer, mvParams);

                        Marshal.FreeHGlobal(ptr1);
                        Marshal.FreeHGlobal(ptr2);
                    }
                    D3DUtils.EndPerfEvent(Viewport.Context);

                    D3DUtils.BeginPerfEvent(Viewport.Context, "Resolve");
                    {
                        // TXAA Resolve
                        Viewport.Context.OutputMerger.SetRenderTargets(null, null, null, null, null);

                        GFSDK_TXAA.NvTxaaFeedbackParameters feedbackParams = GFSDK_TXAA.NvTxaaFeedbackParameters.NvTxaaDefaultFeedback;
                        IntPtr feedbackParamsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<GFSDK_TXAA.NvTxaaFeedbackParameters>());
                        Marshal.StructureToPtr<GFSDK_TXAA.NvTxaaFeedbackParameters>(feedbackParams, feedbackParamsPtr, true);

                        GFSDK_TXAA.GetJitter(out float[] jitter);

                        GFSDK_TXAA.NvTxaaPerFrameConstants constants = new GFSDK_TXAA.NvTxaaPerFrameConstants
                        {
                            xJitter = jitter[0],
                            yJitter = jitter[1],
                            mvScale = 1024.0f,
                            motionVecSelection = 3,
                            useRGB = 0,
                            frameBlendFactor = 0.04f,
                            dbg1 = 0,
                            bbScale = 1.0f,
                            enableClipping = 1,
                            useBHFilters = 1
                        };
                        //constants.isZFlipped = 1;
                        IntPtr constantsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<GFSDK_TXAA.NvTxaaPerFrameConstants>());
                        Marshal.StructureToPtr<GFSDK_TXAA.NvTxaaPerFrameConstants>(constants, constantsPtr, true);

                        GFSDK_TXAA.NvTxaaResolveParametersDX11 resolveParams = new GFSDK_TXAA.NvTxaaResolveParametersDX11
                        {
                            txaaContext = txaaContext,
                            deviceContext = Viewport.Context.NativePointer,
                            resolveTarget = postProcessTexture.RTV.NativePointer,
                            msaaSource = lightAccumulationTexture.SRV.NativePointer,
                            msaaDepth = Viewport.DepthBufferSRV.NativePointer,
                            feedbackSource = txaaFeedbackTeture.SRV.NativePointer,
                            alphaResolveMode = 1,
                            feedback = feedbackParamsPtr,
                            perFrameConstants = constantsPtr
                        };

                        GFSDK_TXAA.NvTxaaMotionDX11 mParams = new GFSDK_TXAA.NvTxaaMotionDX11
                        {
                            motionVectors = txaaMotionVectorsTexture.SRV.NativePointer,
                            motionVectorsMS = txaaMotionVectorsTexture.SRV.NativePointer
                        };

                        IntPtr resolveParamsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<GFSDK_TXAA.NvTxaaResolveParametersDX11>());
                        Marshal.StructureToPtr<GFSDK_TXAA.NvTxaaResolveParametersDX11>(resolveParams, resolveParamsPtr, true);

                        IntPtr mParamsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<GFSDK_TXAA.NvTxaaMotionDX11>());
                        Marshal.StructureToPtr<GFSDK_TXAA.NvTxaaMotionDX11>(mParams, mParamsPtr, true);

                        int retCode = GFSDK_TXAA.ResolveFromMotionVectors(resolveParamsPtr, mParamsPtr);
                        Viewport.Context.CopyResource(postProcessTexture.Texture, txaaFeedbackTeture.Texture);

                        Marshal.FreeHGlobal(mParamsPtr);
                        Marshal.FreeHGlobal(resolveParamsPtr);
                        Marshal.FreeHGlobal(constantsPtr);
                        Marshal.FreeHGlobal(feedbackParamsPtr);
                    }
                    D3DUtils.EndPerfEvent(Viewport.Context);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);
            }
            else
            {
                D3DUtils.BeginPerfEvent(Viewport.Context, "Resolve");
                {
                    Viewport.Context.CopyResource(lightAccumulationTexture.Texture, postProcessTexture.Texture);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcessDownScaleScene()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "Downscale4x4");
            {
                Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                {
                    float tU = 1.0f / (postProcessTexture.Texture.Description.Width);
                    float tV = 1.0f / (postProcessTexture.Texture.Description.Height);

                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            stream.Write((x - 1.5f) * tU);
                            stream.Write((y - 1.5f) * tV);
                            stream.Write(Vector2.Zero);
                        }
                    }
                }
                Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                Viewport.Context.OutputMerger.SetRenderTargets(null, scaledSceneTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
                Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, scaledSceneTexture.Texture.Description.Width, scaledSceneTexture.Texture.Description.Height));

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psDownscale4x4);
                Viewport.Context.PixelShader.SetShaderResources(0, postProcessTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));
                Viewport.Context.PixelShader.SetConstantBuffer(1, postProcessConstants);

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcessMeasureLuminance()
        {
            int curTexture = 3;

            D3DUtils.BeginPerfEvent(Viewport.Context, "SampleLuminanceInitial");
            {
                // first pass
                Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                {
                    float tU = 1.0f / (3.0f * toneMapTextures[curTexture].Texture.Description.Width);
                    float tV = 1.0f / (3.0f * toneMapTextures[curTexture].Texture.Description.Height);

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            stream.Write(x * tU);
                            stream.Write(y * tV);
                            stream.Write(Vector2.Zero);
                        }
                    }
                }
                Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                toneMapTextures[curTexture].Clear(Viewport.Context, new Color4(0, 0, 0, 0));
                Viewport.Context.OutputMerger.SetRenderTargets(null, toneMapTextures[curTexture].RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
                Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, toneMapTextures[curTexture].Texture.Description.Width, toneMapTextures[curTexture].Texture.Description.Height));

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psSampleLumInitial);
                Viewport.Context.PixelShader.SetShaderResources(0, scaledSceneTexture.SRV, toneMapTextures[5].SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));
                Viewport.Context.PixelShader.SetConstantBuffers(0, commonConstants.Buffer, postProcessConstants);

                Viewport.Context.Draw(6, 0);
                curTexture--;
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            D3DUtils.BeginPerfEvent(Viewport.Context, "SampleLuminanceIterative");
            {
                // iterative downscale
                while (curTexture > 0)
                {
                    Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    {
                        float tU = 1.0f / (toneMapTextures[curTexture + 1].Texture.Description.Width);
                        float tV = 1.0f / (toneMapTextures[curTexture + 1].Texture.Description.Height);

                        for (int y = 0; y < 4; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                stream.Write((x - 1.5f) * tU);
                                stream.Write((y - 1.5f) * tV);
                                stream.Write(Vector2.Zero);
                            }
                        }
                    }
                    Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                    toneMapTextures[curTexture].Clear(Viewport.Context, new Color4(0, 0, 0, 0));
                    Viewport.Context.OutputMerger.SetRenderTargets(null, toneMapTextures[curTexture].RTV);
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, toneMapTextures[curTexture].Texture.Description.Width, toneMapTextures[curTexture].Texture.Description.Height));

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psSampleLumIterative);
                    Viewport.Context.PixelShader.SetShaderResources(1, toneMapTextures[curTexture + 1].SRV);
                    Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));
                    Viewport.Context.PixelShader.SetConstantBuffer(1, postProcessConstants);

                    Viewport.Context.Draw(6, 0);
                    curTexture--;
                }
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            D3DUtils.BeginPerfEvent(Viewport.Context, "SampleLuminanceFinal");
            {
                // downscale 1x1
                Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                {
                    float tU = 1.0f / (toneMapTextures[1].Texture.Description.Width);
                    float tV = 1.0f / (toneMapTextures[1].Texture.Description.Height);

                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            stream.Write((x - 1.5f) * tU);
                            stream.Write((y - 1.5f) * tV);
                            stream.Write(Vector2.Zero);
                        }
                    }
                }
                Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                toneMapTextures[0].Clear(Viewport.Context, new Color4(0, 0, 0, 0));
                Viewport.Context.OutputMerger.SetRenderTargets(null, toneMapTextures[0].RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
                Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, toneMapTextures[0].Texture.Description.Width, toneMapTextures[0].Texture.Description.Height));

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psSampleLumFinal);
                Viewport.Context.PixelShader.SetShaderResources(1, toneMapTextures[1].SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));
                Viewport.Context.PixelShader.SetConstantBuffer(1, postProcessConstants);

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);

            D3DUtils.BeginPerfEvent(Viewport.Context, "CalculateAdaptedLuminance");
            {
                // calculate adapted luminance
                Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                {
                    stream.Write((float)lastDeltaTime);
                }
                Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                toneMapTextures[4].Clear(Viewport.Context, new Color4(0, 0, 0, 0));
                Viewport.Context.OutputMerger.SetRenderTargets(null, toneMapTextures[4].RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
                Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, toneMapTextures[4].Texture.Description.Width, toneMapTextures[4].Texture.Description.Height));

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psCalcAdaptedLum);
                Viewport.Context.PixelShader.SetShaderResources(0, toneMapTextures[5].SRV, toneMapTextures[0].SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));
                Viewport.Context.PixelShader.SetConstantBuffer(1, postProcessConstants);

                Viewport.Context.Draw(6, 0);

                // copy current luminance into previous
                Viewport.Context.ResolveSubresource(toneMapTextures[4].Texture, 0, toneMapTextures[5].Texture, 0, SharpDX.DXGI.Format.R32_Float);
                Viewport.Context.CopyResource(toneMapTextures[4].Texture, toneMapTextures[6].Texture);

                // read out average luminance
                Viewport.Context.MapSubresource(toneMapTextures[6].Texture, 0, MapMode.Read, MapFlags.None, out stream);
                {
                    // store into a histogram
                    float avgLuminance = stream.Read<float>();
                    luminanceHistogram.Add(avgLuminance);
                }
                Viewport.Context.UnmapSubresource(toneMapTextures[6].Texture, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcessBloom()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "Bloom");
            {
                brightPassTexture.Clear(Viewport.Context, Color4.Black);
                blurTexture.Clear(Viewport.Context, Color4.Black);
                bloomSourceTexture.Clear(Viewport.Context, Color4.Black);
                bloomTextures[0].Clear(Viewport.Context, Color4.Black);
                bloomTextures[1].Clear(Viewport.Context, Color4.Black);
                bloomTextures[2].Clear(Viewport.Context, Color4.Black);

                D3DUtils.BeginPerfEvent(Viewport.Context, "BrightPass");
                {
                    Viewport.Context.OutputMerger.SetRenderTargets(null, brightPassTexture.RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, brightPassTexture.Texture.Description.Width, brightPassTexture.Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psBrightPass);
                    Viewport.Context.PixelShader.SetShaderResources(0, scaledSceneTexture.SRV, toneMapTextures[4].SRV);
                    Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "Blur");
                {
                    Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    {
                        float tu = 1.0f / (float)blurTexture.Texture.Description.Width;
                        float tv = 1.0f / (float)blurTexture.Texture.Description.Height;

                        Vector4 vWhite = new Vector4(1, 1, 1, 1);
                        Vector4[] avSampleWeight = new Vector4[16];
                        Vector2[] avTexOffsets = new Vector2[16];

                        float totalWeight = 0.0f;
                        int index = 0;
                        for (int x = -2; x <= 2; x++)
                        {
                            for (int y = -2; y <= 2; y++)
                            {
                                if (Math.Abs(x) + Math.Abs(y) > 2)
                                    continue;

                                avTexOffsets[index] = new Vector2(x * tu, y * tv);
                                avSampleWeight[index] = (vWhite * GaussianDistribution((float)x, (float)y, 1.0f));
                                totalWeight += avSampleWeight[index].X;

                                index++;
                            }
                        }

                        for (int i = 0; i < index; i++)
                            avSampleWeight[i] /= totalWeight;

                        for (int i = 0; i < 16; i++)
                        {
                            stream.Write(avTexOffsets[i]);
                            stream.Write(Vector2.Zero);
                        }
                        for (int i = 0; i < 16; i++)
                            stream.Write(avSampleWeight[i]);
                    }
                    Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                    Viewport.Context.OutputMerger.SetRenderTargets(null, blurTexture.RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, blurTexture.Texture.Description.Width, blurTexture.Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psGaussianBlur5x5);
                    Viewport.Context.PixelShader.SetShaderResources(0, brightPassTexture.SRV);
                    Viewport.Context.PixelShader.SetConstantBuffers(1, postProcessConstants);
                    Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "BloomSource");
                {
                    Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    {
                        float tU = 1.0f / brightPassTexture.Texture.Description.Width;
                        float tV = 1.0f / brightPassTexture.Texture.Description.Height;

                        for (int y = 0; y < 2; y++)
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                stream.Write((x - 0.5f) * tU);
                                stream.Write((y - 0.5f) * tV);
                                stream.Write(Vector2.Zero);
                            }
                        }
                    }
                    Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                    Viewport.Context.OutputMerger.SetRenderTargets(null, bloomSourceTexture.RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, bloomSourceTexture.Texture.Description.Width, bloomSourceTexture.Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psDownSample2x2);
                    Viewport.Context.PixelShader.SetShaderResources(0, blurTexture.SRV);
                    Viewport.Context.PixelShader.SetConstantBuffers(1, postProcessConstants);
                    Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "Blur");
                {
                    Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    {
                        float tu = 1.0f / (float)bloomSourceTexture.Texture.Description.Width;
                        float tv = 1.0f / (float)bloomSourceTexture.Texture.Description.Height;

                        Vector4 vWhite = new Vector4(1, 1, 1, 1);
                        Vector4[] avSampleWeight = new Vector4[16];
                        Vector2[] avTexOffsets = new Vector2[16];

                        float totalWeight = 0.0f;
                        int index = 0;
                        for (int x = -2; x <= 2; x++)
                        {
                            for (int y = -2; y <= 2; y++)
                            {
                                if (Math.Abs(x) + Math.Abs(y) > 2)
                                    continue;

                                avTexOffsets[index] = new Vector2(x * tu, y * tv);
                                avSampleWeight[index] = (vWhite * GaussianDistribution((float)x, (float)y, 1.0f));
                                totalWeight += avSampleWeight[index].X;

                                index++;
                            }
                        }

                        for (int i = 0; i < index; i++)
                            avSampleWeight[i] /= totalWeight;

                        for (int i = 0; i < 16; i++)
                        {
                            stream.Write(avTexOffsets[i]);
                            stream.Write(Vector2.Zero);
                        }
                        for (int i = 0; i < 16; i++)
                            stream.Write(avSampleWeight[i]);
                    }
                    Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                    Viewport.Context.OutputMerger.SetRenderTargets(null, bloomTextures[2].RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, bloomTextures[2].Texture.Description.Width, bloomTextures[2].Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psGaussianBlur5x5);
                    Viewport.Context.PixelShader.SetShaderResources(0, bloomSourceTexture.SRV);
                    Viewport.Context.PixelShader.SetConstantBuffers(1, postProcessConstants);
                    Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "HorizontalBlur");
                {
                    Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    {
                        float tu = 1.0f / bloomTextures[2].Texture.Description.Width;

                        float weight = 2.0f * GaussianDistribution(0, 0, 3.0f);
                        Vector4[] avColorWeights = new Vector4[16];
                        float[] afTexCoordOffsets = new float[16];

                        avColorWeights[0] = new Vector4(weight, weight, weight, 1.0f);
                        afTexCoordOffsets[0] = 0.0f;

                        for (int i = 1; i < 8; i++)
                        {
                            weight = 2.0f * GaussianDistribution(i, 0, 3.0f);
                            afTexCoordOffsets[i] = i * tu;
                            avColorWeights[i] = new Vector4(weight, weight, weight, 1.0f);
                        }
                        for (int i = 8; i < 15; i++)
                        {
                            avColorWeights[i] = avColorWeights[i - 7];
                            afTexCoordOffsets[i] = -afTexCoordOffsets[i - 7];
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            stream.Write(afTexCoordOffsets[i]);
                            stream.Write(Vector3.Zero);
                        }
                        for (int i = 0; i < 16; i++)
                            stream.Write(avColorWeights[i]);
                    }
                    Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                    Viewport.Context.OutputMerger.SetRenderTargets(null, bloomTextures[1].RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, bloomTextures[1].Texture.Description.Width, bloomTextures[1].Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psBloomBlur);
                    Viewport.Context.PixelShader.SetShaderResources(0, bloomTextures[2].SRV);
                    Viewport.Context.PixelShader.SetConstantBuffers(1, postProcessConstants);
                    Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "VerticalBlur");
                {
                    Viewport.Context.MapSubresource(postProcessConstants, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                    {
                        float tu = 1.0f / bloomTextures[1].Texture.Description.Height;

                        float weight = 2.0f * GaussianDistribution(0, 0, 3.0f);
                        Vector4[] avColorWeights = new Vector4[16];
                        float[] afTexCoordOffsets = new float[16];

                        avColorWeights[0] = new Vector4(weight, weight, weight, 1.0f);
                        afTexCoordOffsets[0] = 0.0f;

                        for (int i = 1; i < 8; i++)
                        {
                            weight = 2.0f * GaussianDistribution(i, 0, 3.0f);
                            afTexCoordOffsets[i] = i * tu;
                            avColorWeights[i] = new Vector4(weight, weight, weight, 1.0f);
                        }
                        for (int i = 8; i < 15; i++)
                        {
                            avColorWeights[i] = avColorWeights[i - 7];
                            afTexCoordOffsets[i] = -afTexCoordOffsets[i - 7];
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            stream.Write(0.0f);
                            stream.Write(afTexCoordOffsets[i]);
                            stream.Write(Vector2.Zero);
                        }
                        for (int i = 0; i < 16; i++)
                            stream.Write(avColorWeights[i]);
                    }
                    Viewport.Context.UnmapSubresource(postProcessConstants, 0);

                    Viewport.Context.OutputMerger.SetRenderTargets(null, bloomTextures[0].RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, bloomTextures[0].Texture.Description.Width, bloomTextures[0].Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psBloomBlur);
                    Viewport.Context.PixelShader.SetShaderResources(0, bloomTextures[1].SRV);
                    Viewport.Context.PixelShader.SetConstantBuffers(1, postProcessConstants);
                    Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);

                D3DUtils.BeginPerfEvent(Viewport.Context, "RenderBloom");
                {
                    Viewport.Context.OutputMerger.SetRenderTargets(null, postProcessTexture.RTV);
                    Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, lightAccumulationTexture.Texture.Description.Width, lightAccumulationTexture.Texture.Description.Height));
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(new RenderTargetBlendDescription() { IsBlendEnabled = true, SourceBlend = BlendOption.One, DestinationBlend = BlendOption.One, BlendOperation = BlendOperation.Add, SourceAlphaBlend = BlendOption.One, DestinationAlphaBlend = BlendOption.One, AlphaBlendOperation = BlendOperation.Add, RenderTargetWriteMask = ColorWriteMaskFlags.All });
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psRenderBloom);
                    Viewport.Context.PixelShader.SetShaderResources(0, bloomTextures[0].SRV);
                    Viewport.Context.PixelShader.SetSamplers(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));

                    Viewport.Context.Draw(6, 0);
                }
                D3DUtils.EndPerfEvent(Viewport.Context);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcessHBAO()
        {
            if (!HBAOEnabled)
                return;

            D3DUtils.BeginPerfEvent(Viewport.Context, "HBAO");
            {
                GFSDK_TXAA.GetJitter(out float[] jitter);

                GFSDK_SSAO.InputData inputData = new GFSDK_SSAO.InputData
                {
                    DepthData =
                    {
                        pFullResDepthTextureSRV = Viewport.DepthBufferSRV.NativePointer,
                        DepthTextureType = GFSDK_SSAO.DepthTextureType.HardwareDepths,
                        MetersToViewSpaceUnits = 1.0f,
                        ProjectionMatrix =
                        {
                            Data = camera.GetProjMatrix(jitter),
                            Layout = GFSDK_SSAO.MatrixLayout.RowMajorOrder
                        },
                        Viewport = GFSDK_SSAO.InputViewport.FromViewport(new SharpDX.Viewport(0, 0, Viewport.DepthBuffer.Description.Width, Viewport.DepthBuffer.Description.Height, 0.0f, 1.0f))
                    },

                    NormalData =
                    {
                        Enable = true,
                        pFullResNormalTextureSRV = worldNormalsForHBAOTexture.SRV.NativePointer,
                        WorldToViewMatrix = {Data = Matrix.Scaling(-1, 1, 1) * camera.GetViewMatrix()},
                        DecodeScale = 2.0f,
                        DecodeBias = -1.0f
                    }
                };

                inputData.NormalData.WorldToViewMatrix.Layout = GFSDK_SSAO.MatrixLayout.RowMajorOrder;

                GFSDK_SSAO.Output output = new GFSDK_SSAO.Output
                {
                    pRenderTargetView = lightAccumulationTexture.RTV.NativePointer,

                    Blend =
                    {
                        Mode = (RenderMode == DebugRenderMode.HBAO)
                            ? GFSDK_SSAO.BlendMode.OverwriteRGB
                            : GFSDK_SSAO.BlendMode.MultiplyRGB
                    }
                };

                int retVal = hbaoContext.RenderAO(Viewport.Context, inputData, new GFSDK_SSAO.Parameters(true), output, GFSDK_SSAO.RenderMask.RenderAO);
#if FROSTY_DEVELOPER
                System.Diagnostics.Debug.Assert(retVal == 0);
#endif
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcessColorLookupTable()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "ColorLookupTable");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, finalColorTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
                Viewport.Context.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, Viewport.ColorBuffer.Description.Width, Viewport.ColorBuffer.Description.Height));

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                if (LookupTable != null)
                {
                    Texture2D lookupTableTexture = LookupTable.ResourceAs<Texture2D>();
                    lookupTableConstants.UpdateData(Viewport.Context, new TableLookupConstants()
                    {
                        LutSize = lookupTableTexture.Description.Width,
                        FlipY = (lookupTableTexture.Description.Width == 33) ? 1.0f : 0.0f
                    });

                    Viewport.Context.PixelShader.Set(psLookupTable);
                    Viewport.Context.PixelShader.SetConstantBuffer(1, lookupTableConstants.Buffer);
                }
                else
                {
                    // otherwise just resolve to final color
                    Viewport.Context.PixelShader.Set(psResolve);
                }

                Viewport.Context.PixelShader.SetShaderResources(0, postProcessTexture.SRV, LookupTable);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));
                Viewport.Context.PixelShader.SetSampler(1, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipLinear));
                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// Collect the selected objects and render them to the selection buffer
        /// </summary>
        private void PostProcessCollectSelections()
        {
            if (meshes.Count == 0)
                return;

            D3DUtils.BeginPerfEvent(Viewport.Context, "CollectSelections");
            {
                // need to update the view constants to get a non jittered matrix
                UpdateViewConstants(false);

                Viewport.Context.OutputMerger.SetRenderTargets(selectionDepthTexture.DSV, null, null);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(depthComparison: Comparison.LessEqual);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.Front, depthClip: true);

                Viewport.Context.VertexShader.SetConstantBuffer(0, viewConstants.Buffer);

                Viewport.Context.PixelShader.SetConstantBuffer(0, viewConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResource(0, normalBasisCubemapTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                RenderMeshes(MeshRenderPath.Selection, new List<MeshRenderInstance>() { meshes[0] });
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostProcessEditorPrimitives()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "EditorPrimitives");
            {
                // resolve main depth into MSAA depth target
                {
                    Viewport.Context.OutputMerger.SetRenderTargets(editorCompositeDepthTexture.DSV, null, null);
                    Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(true, depthComparison: Comparison.Less);
                    Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                    Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                    Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                    Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                    Viewport.Context.InputAssembler.InputLayout = null;

                    Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                    Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                    Viewport.Context.PixelShader.Set(psResolveDepthToMsaa);
                    Viewport.Context.PixelShader.SetShaderResources(0, Viewport.DepthBufferSRV);
                    Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                    Viewport.Context.Draw(6, 0);
                }

                // render editor primitives
                Viewport.Context.OutputMerger.SetRenderTargets(editorCompositeDepthTexture.DSV, editorCompositeTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(depthComparison: Comparison.LessEqual, depthWriteMask: DepthWriteMask.Zero);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.Front, depthClip: true);

                Viewport.Context.VertexShader.SetConstantBuffer(0, viewConstants.Buffer);

                Viewport.Context.PixelShader.SetConstantBuffer(0, viewConstants.Buffer);
                Viewport.Context.PixelShader.SetShaderResource(0, normalBasisCubemapTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                RenderMeshes(MeshRenderPath.Forward, editorMeshes);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void PostProcessSelectionOutline()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "SelectionOutline");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, selectionOutlineTexture.RTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psSelectionOutline);
                Viewport.Context.PixelShader.SetShaderResources(0, finalColorTexture.SRV, selectionDepthTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void PostProcessEditorComposite()
        {
            D3DUtils.BeginPerfEvent(Viewport.Context, "EditorComposite");
            {
                Viewport.Context.OutputMerger.SetRenderTargets(null, Viewport.ColorBufferRTV);
                Viewport.Context.OutputMerger.DepthStencilState = D3DUtils.CreateDepthStencilState(false);
                Viewport.Context.OutputMerger.BlendState = D3DUtils.CreateBlendState(D3DUtils.CreateBlendStateRenderTarget());
                Viewport.Context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);

                Viewport.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
                Viewport.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding());
                Viewport.Context.InputAssembler.InputLayout = null;

                Viewport.Context.VertexShader.Set(vsFullscreenQuad);
                Viewport.Context.VertexShader.SetConstantBuffer(0, commonConstants.Buffer);

                Viewport.Context.PixelShader.Set(psEditorComposite);
                Viewport.Context.PixelShader.SetShaderResources(0, selectionOutlineTexture.SRV, editorCompositeTexture.SRV);
                Viewport.Context.PixelShader.SetSampler(0, D3DUtils.CreateSamplerState(address: TextureAddressMode.Clamp, filter: Filter.MinMagMipPoint));

                Viewport.Context.Draw(6, 0);
            }
            D3DUtils.EndPerfEvent(Viewport.Context);
        }
        #endregion

        #endregion

        /// <summary>
        /// Calculates a bounding box that encompasses all render meshes in the current world
        /// </summary>
        protected virtual BoundingBox CalcWorldBoundingBox()
        {
            return new BoundingBox();
        }

#if FROSTY_DEVELOPER
        /// <summary>
        /// Sets the next frame for capturing
        /// </summary>
        public void CaptureNextFrame()
        {
            renderDocCaptureState = RenderDocCaptureState.BeginCapture;
        }
#endif

        /// <summary>
        /// Do any actions required at the beginning of the frame
        /// </summary>
        protected virtual void BeginFrameActions()
        {
#if FROSTY_DEVELOPER
            // begin frame capturing if requested
            if (renderDocCaptureState == RenderDocCaptureState.BeginCapture)
            {
                renderDocApi.StartFrameCapture(Viewport.Device, IntPtr.Zero);
                renderDocCaptureState = RenderDocCaptureState.CaptureInProgress;
            }
#endif
        }

        /// <summary>
        /// Do any actions required at the end of the frame (where present would normally occur)
        /// </summary>
        protected virtual void EndFrameActions()
        {
#if FROSTY_DEVELOPER
            // end frame capturing and launch ui if in progress
            if (renderDocCaptureState == RenderDocCaptureState.CaptureInProgress)
            {
                renderDocApi.EndFrameCapture(Viewport.Device, IntPtr.Zero);
                renderDocApi.LaunchReplayUI(true, "");
                renderDocCaptureState = RenderDocCaptureState.NotStarted;
            }
#endif
        }

#if FROSTY_DEVELOPER
        /// <summary>
        /// Attempts to initialize the renderdoc api, if dll is present
        /// </summary>
        protected virtual void InitializeRenderDoc()
        {
            try
            {
                // try to load renderdoc
                renderDocApi = RenderDoc.GetAPI(10101);
                renderDocApi.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_DebugOutputMute, 0);
                renderDocApi.SetCaptureOptionU32(RENDERDOC_CaptureOption.eRENDERDOC_Option_HookIntoChildren, 1);
                renderDocApi.SetActiveWindow(null, IntPtr.Zero);
            }
            catch
            {
                // failed to load renderdoc, ignore.
            }
        }
#endif

        protected virtual float GaussianDistribution(float x, float y, float rho)
        {
            float g = 1.0f / (float)Math.Sqrt(2.0f * Math.PI * rho * rho);
            g *= (float)Math.Exp(-(x * x + y * y) / (2 * rho * rho));

            return g;
        }
    }
}
