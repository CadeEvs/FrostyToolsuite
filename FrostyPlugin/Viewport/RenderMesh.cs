using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;
using FrostySdk;
using SharpDX.Direct3D11;
using FrostySdk.Managers;
using FrostySdk.IO;
using System.IO;
using System.Xml;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.Direct3D;
using FrostySdk.Ebx;
using Frosty.Controls;
using SharpDX.D3DCompiler;
using System.Collections;
using FrostySdk.Attributes;
using System.Windows;
using System.Globalization;
using Frosty.Hash;
using Frosty.Core.Screens;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Viewport
{
    #region -- Utilities --
    public class DirectXMathUtils
    {
        public static void XMScalarSinCos(ref float Sin, ref float Cos, float Value)
        {
            const float XM1div2PI = 0.159154943f;
            // Map Value to y in [-pi,pi], x = 2*pi*quotient + remainder.
            float quotient = XM1div2PI * Value;
            if (Value >= 0.0f)
            {
                quotient = (float)((int)(quotient + 0.5f));
            }
            else
            {
                quotient = (float)((int)(quotient - 0.5f));
            }
            float y = Value - MathUtil.TwoPi * quotient;

            // Map y to [-pi/2,pi/2] with sin(y) = sin(Value).
            float sign;
            if (y > MathUtil.PiOverTwo)
            {
                y = MathUtil.Pi - y;
                sign = -1.0f;
            }
            else if (y < -(MathUtil.PiOverTwo))
            {
                y = -(MathUtil.Pi) - y;
                sign = -1.0f;
            }
            else
            {
                sign = +1.0f;
            }

            float y2 = y * y;

            // 11-degree minimax approximation
            Sin = (((((-2.3889859e-08f * y2 + 2.7525562e-06f) * y2 - 0.00019840874f) * y2 + 0.0083333310f) * y2 - 0.16666667f) * y2 + 1.0f) * y;

            // 10-degree minimax approximation
            float p = (float)((float)((((-2.6051615e-07f * y2 + 2.4760495e-05f) * y2 - 0.0013888378f) * y2 + 0.041666638f) * y2 - 0.5f) * y2 + 1.0f);

            Cos = sign * p;
        }
    }
    #endregion

    #region -- Shaders --
    public struct FallbackVertex
    {
        public Vector3 Position;
        public Vector4 Normal;
        public Vector4 Tangent;
        public Vector4 Bitangent;
        public Vector2 TexCoord0;
        public Vector2 TexCoord1;
        public Vector2 TexCoord2;
        public Vector4 Color0;
        public Vector4 Color1;
        public uint BoneIndices0;
        public uint BoneIndices1;
        public uint BoneIndices2;
        public uint BoneIndices3;
        public uint BoneIndices4;
        public uint BoneIndices5;
        public uint BoneIndices6;
        public uint BoneIndices7;
        public float BoneWeights0;
        public float BoneWeights1;
        public float BoneWeights2;
        public float BoneWeights3;
        public float BoneWeights4;
        public float BoneWeights5;
        public float BoneWeights6;
        public float BoneWeights7;
        public uint TangentSpace;
    }

    public enum ShaderParameterType
    {
        Bool,

        Float,
        Float2,
        Float3,
        Float4,

        Tex2d,
        Tex2dArray,
        TexCube
    }
    public class ShaderIncludeHandler : Include
    {
        public IDisposable Shadow { get; set; }
        public Stream Open(IncludeType type, string fileName, Stream parentStream)
        {
            return new FileStream("Shaders/" + fileName, FileMode.Open, FileAccess.Read);
        }

        public void Close(Stream stream) => stream.Dispose();

        public void Dispose()
        {
        }
    }

    public class ShaderParameter
    {
        public string Name { get; private set; }
        public ShaderParameterType Type { get; private set; }
        public string StringValue => value != null ? Encoding.UTF8.GetString(value) : null;
        public bool BoolValue => value != null && value[0] == 1;
        public float FloatValue => value != null ? BitConverter.ToSingle(value, 0) : 0.0f;
        public Vector2 Float2Value => value != null ? new Vector2(BitConverter.ToSingle(value, 0), BitConverter.ToSingle(value, 4)) : Vector2.Zero;
        public Vector3 Float3Value => value != null ? new Vector3(BitConverter.ToSingle(value, 0), BitConverter.ToSingle(value, 4), BitConverter.ToSingle(value, 8)) : Vector3.Zero;
        public Vector4 Float4Value => value != null ? new Vector4(BitConverter.ToSingle(value, 0), BitConverter.ToSingle(value, 4), BitConverter.ToSingle(value, 8), BitConverter.ToSingle(value, 12)) : Vector4.Zero;

        private byte[] value;

        public ShaderParameter(string inName, ShaderParameterType inType, bool inValue)
        {
            Name = inName;
            Type = inType;
            value = new byte[] { (byte)((inValue) ? 1 : 0) };
        }

        public ShaderParameter(string inName, ShaderParameterType inType)
        {
            Name = inName;
            Type = inType;
            value = null;
        }

        public ShaderParameter(string inName, ShaderParameterType inType, params float[] inValue)
        {
            Name = inName;
            Type = inType;
            value = null;
            value = new byte[inValue.Length * 4];
            for (int i = 0; i < inValue.Length; i++)
            {
                byte[] b = BitConverter.GetBytes(inValue[i]);
                value[(i * 4) + 0] = b[0];
                value[(i * 4) + 1] = b[1];
                value[(i * 4) + 2] = b[2];
                value[(i * 4) + 3] = b[3];
            }
        }

        public ShaderParameter(string inName, ShaderParameterType inType, string inValue)
        {
            Name = inName;
            Type = inType;
            if (inValue != "")
                value = Encoding.UTF8.GetBytes(inValue);
        }
    }
    public class ShaderPermutation : IDisposable
    {
        public GeometryDeclarationDesc GeometryDeclaration => geomDecl;
        public string Name { get; }
        public Shader Parent { get; }

        public bool IsFallback { get; set; }
        public bool IsSkinned { get; set; }
        public bool IsTwoSided { get; set; }
        public int MaxBonesPerVertex { get; set; }

        public List<ShaderParameter> VertexParameters = new List<ShaderParameter>();
        public List<ShaderParameter> VertexTextures = new List<ShaderParameter>();
        public List<ShaderParameter> PixelParameters = new List<ShaderParameter>();
        public List<ShaderParameter> PixelTextures = new List<ShaderParameter>();
        public List<SamplerStateDescription> PixelSamplerDescs = new List<SamplerStateDescription>();

        public int PixelConstantsSize;
        public int VertexConstantsSize;

        public VertexShader vertexShader;
        public PixelShader pixelShader;
        public InputLayout inputLayout;
        public List<SamplerState> pixelSamplers = new List<SamplerState>();
        public BoneBuffer boneBuffer;

        private GeometryDeclarationDesc geomDecl;
        private string shaderFilename;

        public ShaderPermutation(string inName, GeometryDeclarationDesc inGeomDecl, string inShaderFilename, Shader inParent)
        {
            Name = inName;
            geomDecl = inGeomDecl;
            shaderFilename = inShaderFilename;
            Parent = inParent;
        }

        private ShaderPermutation(Shader inParent)
        {
            Parent = inParent;
        }

        /// <summary>
        /// Loads in the necessary shaders and resources
        /// </summary>
        public bool LoadShaders(Device device)
        {
            if (vertexShader != null)
                return true;

            byte[] vsBytecode = null;
            byte[] psBytecode = null;
            bool bLoaded = false;

            string shaderPath = "Shaders/" + shaderFilename + ".bin";
            if (File.Exists(shaderPath))
            {
                int nameHash = Fnv1.HashString(Name);
                using (NativeReader reader = new NativeReader(new FileStream(shaderPath, FileMode.Open, FileAccess.Read)))
                {
                    long lastCompileTime = reader.ReadLong();
                    int permutationCount = reader.ReadInt();

                    for (int i = 0; i < permutationCount; i++)
                    {
                        int permutationHash = reader.ReadInt();
                        long offset = reader.ReadLong();

                        if (permutationHash == nameHash)
                        {
                            reader.Position = offset;
                            int vsBytecodeSize = reader.ReadInt();
                            int psBytecodeSize = reader.ReadInt();

                            vsBytecode = reader.ReadBytes(vsBytecodeSize);
                            psBytecode = reader.ReadBytes(psBytecodeSize);
                            bLoaded = true;

                            break;
                        }
                    }
                }
            }

            if (!bLoaded)
                return false;

            vertexShader = new VertexShader(device, vsBytecode);

            // generate the input layout
            List<InputElement> elems = new List<InputElement>();
            for (int i = 0; i < geomDecl.ElementCount; i++)
            {
                SharpDX.DXGI.Format format = SharpDX.DXGI.Format.Unknown;
                switch (geomDecl.Elements[i].Format)
                {
                    case VertexElementFormat.Float: format = SharpDX.DXGI.Format.R32_Float; break;
                    case VertexElementFormat.Float2: format = SharpDX.DXGI.Format.R32G32_Float; break;
                    case VertexElementFormat.Float3: format = SharpDX.DXGI.Format.R32G32B32_Float; break;
                    case VertexElementFormat.Float4: format = SharpDX.DXGI.Format.R32G32B32A32_Float; break;

                    case VertexElementFormat.Half: format = SharpDX.DXGI.Format.R16_Float; break;
                    case VertexElementFormat.Half2: format = SharpDX.DXGI.Format.R16G16_Float; break;
                    case VertexElementFormat.Half3: format = SharpDX.DXGI.Format.R16G16B16A16_Float; break;
                    case VertexElementFormat.Half4: format = SharpDX.DXGI.Format.R16G16B16A16_Float; break;

                    case VertexElementFormat.UByteN: format = SharpDX.DXGI.Format.R8_UInt; break;
                    case VertexElementFormat.Byte4: format = SharpDX.DXGI.Format.R8G8B8A8_SInt; break;
                    case VertexElementFormat.Byte4N: format = SharpDX.DXGI.Format.R8G8B8A8_SNorm; break;
                    case VertexElementFormat.UByte4N: format = SharpDX.DXGI.Format.R8G8B8A8_UNorm; break;
                    case VertexElementFormat.UByte4: format = SharpDX.DXGI.Format.R8G8B8A8_UInt; break;

                    case VertexElementFormat.Short2N: format = SharpDX.DXGI.Format.R16G16_SNorm; break;
                    case VertexElementFormat.UShort4: format = SharpDX.DXGI.Format.R16G16B16A16_UInt; break;
                    case VertexElementFormat.Short4: format = SharpDX.DXGI.Format.R16G16B16A16_SInt; break;
                    case VertexElementFormat.Short4N: format = SharpDX.DXGI.Format.R16G16B16A16_SNorm; break;
                    case VertexElementFormat.UInt: format = SharpDX.DXGI.Format.R32_UInt; break;
                }

                int index = 0;
                string semanticName = geomDecl.Elements[i].Usage.ToString().ToUpper();

                if (geomDecl.Elements[i].Usage >= VertexElementUsage.TexCoord0 && geomDecl.Elements[i].Usage <= VertexElementUsage.TexCoord7)
                {
                    index = (int)(geomDecl.Elements[i].Usage - VertexElementUsage.TexCoord0);
                    semanticName = semanticName.Remove(semanticName.Length - 1, 1);
                }
                if (geomDecl.Elements[i].Usage >= VertexElementUsage.Color0 && geomDecl.Elements[i].Usage <= VertexElementUsage.Color1)
                {
                    index = (int)(geomDecl.Elements[i].Usage - VertexElementUsage.Color0);
                    semanticName = semanticName.Remove(semanticName.Length - 1, 1);
                }
                if (geomDecl.Elements[i].Usage == VertexElementUsage.BoneIndices2 || geomDecl.Elements[i].Usage == VertexElementUsage.BoneWeights2)
                {
                    index = 2;
                    semanticName = semanticName.Remove(semanticName.Length - 1, 1);
                }

                elems.Add(new InputElement(semanticName, index, format, geomDecl.Elements[i].Offset, geomDecl.Elements[i].StreamIndex, InputClassification.PerVertexData, 0));
            }
            inputLayout = new InputLayout(device, vsBytecode, elems.ToArray());
            pixelShader = new PixelShader(device, psBytecode);

            // create any samplers
            foreach (SamplerStateDescription desc in PixelSamplerDescs)
                pixelSamplers.Add(D3DUtils.CreateSamplerState(desc));

            if (IsSkinned)
            {
                // setup a new bone buffer
                boneBuffer = new BoneBuffer(device, 100);
            }

            return true;
        }

        /// <summary>
        /// Sets up the render state for this permutation
        /// </summary>
        public void SetState(DeviceContext context, MeshRenderPath renderPath)
        {
            context.VertexShader.Set(vertexShader);
            context.InputAssembler.InputLayout = inputLayout;

            if (IsSkinned)
                context.VertexShader.SetShaderResources(0, boneBuffer.SRV);

            if (renderPath != MeshRenderPath.Shadows)
            {
                context.PixelShader.Set(pixelShader);
                context.PixelShader.SetSamplers(1, pixelSamplers.ToArray());

                if (IsTwoSided)
                    context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
            }
            else
            {
                context.PixelShader.Set(null);
                context.Rasterizer.State = D3DUtils.CreateRasterizerState(CullMode.None);
            }
        }

        /// <summary>
        /// Assigns only the shaders default parameter values
        /// </summary>
        public void AssignParameters(RenderCreateState state, ref D3D11.Buffer pixelParameters, ref List<ShaderResourceView> pixelTextures)
        {
            AssignParameters(state, new List<ShaderParameter>(), new List<ShaderParameter>(), ref pixelParameters, ref pixelTextures);
        }

        /// <summary>
        /// Assigns the shader parameters from the provided lists
        /// </summary>
        public void AssignParameters(RenderCreateState state, List<ShaderParameter> pixelParamValues, List<ShaderParameter> pixelTextureValues, ref D3D11.Buffer pixelParameters, ref List<ShaderResourceView> pixelTextures)
        {
            if (pixelParameters != null)
            {
                pixelParameters.Dispose();
                pixelParameters = null;
            }

            int size = PixelConstantsSize;
            size = ((size + 15) / 16) * 16;

            if (size > 0)
            {
                using (DataStream stream = new DataStream(size, false, true))
                {
                    foreach (ShaderParameter param in PixelParameters)
                    {
                        ShaderParameter vecParam = pixelParamValues.Find((ShaderParameter a) => a.Name == param.Name) ?? param;

                        Vector4 value = vecParam.Float4Value;
                        stream.Write<float>(value.X);
                        stream.Write<float>(value.Y);
                        stream.Write<float>(value.Z);
                        stream.Write<float>(value.W);
                    }

                    stream.Position = 0;
                    pixelParameters = new D3D11.Buffer(state.Device, stream, size, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

                }
            }

            ShaderResourceView[] srvs = pixelTextures.ToArray();
            pixelTextures.Clear();

            foreach (ShaderParameter param in PixelTextures)
            {
                ShaderParameter texParam = pixelTextureValues.Find((ShaderParameter a) => a.Name == param.Name);
                ShaderResourceView srv = null;

                if (texParam == null)
                    texParam = param;

                if (texParam.StringValue != null)
                {
                    string texPath = texParam.StringValue;
                    if (texPath.StartsWith("Resources"))
                        srv = state.TextureLibrary.LoadTextureAsset(texPath);
                    else
                        srv = state.TextureLibrary.LoadTextureAsset(App.AssetManager.GetEbxEntry(texPath).Guid);
                }

                pixelTextures.Add(srv);
            }

            state.TextureLibrary.UnloadTextures(srvs);
        }

        /// <summary>
        /// Assigns the shader parameters from the provided material
        /// </summary>
        public void AssignParameters(RenderCreateState state, MeshMaterial material, ref D3D11.Buffer pixelParameters, ref List<ShaderResourceView> pixelTextures)
        {
            if (pixelParameters != null)
            {
                pixelParameters.Dispose();
                pixelParameters = null;
            }

            int size = PixelConstantsSize;
            size = ((size + 15) / 16) * 16;

            if (size > 0)
            {
                using (DataStream stream = new DataStream(size, false, true))
                {
                    foreach (ShaderParameter param in PixelParameters)
                    {
                        dynamic vecParam = material.VectorParameters.Find((dynamic a) => a.ParameterName == param.Name);
                        if (vecParam != null)
                        {
                            stream.Write<float>(vecParam.Value.x);
                            stream.Write<float>(vecParam.Value.y);
                            stream.Write<float>(vecParam.Value.z);
                            stream.Write<float>(vecParam.Value.w);
                        }
                        else
                        {
                            Vector4 defValue = param.Float4Value;
                            stream.Write<float>(defValue.X);
                            stream.Write<float>(defValue.Y);
                            stream.Write<float>(defValue.Z);
                            stream.Write<float>(defValue.W);
                        }
                    }

                    stream.Position = 0;
                    pixelParameters = new D3D11.Buffer(state.Device, stream, size, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

                }
            }

            ShaderResourceView[] srvs = pixelTextures.ToArray();
            pixelTextures.Clear();

            foreach (ShaderParameter param in PixelTextures)
            {
                dynamic texParam = material.TextureParameters.Find((dynamic a) =>
                {
                    string paramName = a.ParameterName;
                    int idx = paramName.LastIndexOf('/');

                    if (idx == -1)
                    {
                        return paramName.Equals(param.Name, StringComparison.OrdinalIgnoreCase);
                    }

                    paramName = paramName.Substring(idx + 1);
                    return paramName.Equals(param.Name, StringComparison.OrdinalIgnoreCase);

                });
                ShaderResourceView srv = null;

                if (texParam != null)
                {
                    PointerRef value = texParam.Value;
                    srv = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                }
                else if (param.StringValue != null)
                {
                    string texPath = param.StringValue;
                    srv = texPath.StartsWith("Resources") ? state.TextureLibrary.LoadTextureAsset(texPath) : state.TextureLibrary.LoadTextureAsset(App.AssetManager.GetEbxEntry(texPath).Guid);
                }

                pixelTextures.Add(srv);
            }

            state.TextureLibrary.UnloadTextures(srvs);
        }

        /// <summary>
        /// Assigns the shader parameters for the fallback shader only (this should only be used for a particular shader and mesh type)
        /// NOTE: Use of dynamic here as MeshRenderSection exists in the MeshSet plugin, however the fallback shader requires the MeshRenderSection
        ///       to populate the parameters correctly
        /// </summary>
        public void AssignFallbackParameters(RenderCreateState state, /*MeshRenderSection*/ dynamic section, MeshMaterial material)
        {
            float CustomParam1 = 0.0f;
            float CustomParam2 = 0.0f;
            float CustomParam3 = 0.0f;
            float SRGB = 0.0f;
            Vector4 TintColorA = (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst) ? Vector4.UnitW : Vector4.One;
            Vector4 TintColorB = (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst) ? Vector4.UnitW : Vector4.One;
            Vector4 TintColorC = (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst) ? Vector4.UnitW : Vector4.One;
            Vector4 TintColorD = (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst) ? Vector4.UnitW : Vector4.One;

            List<Vector4> AdditionalParams = new List<Vector4>();
            List<ShaderResourceView> AdditionalTextures = new List<ShaderResourceView>();

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
            {
                for (int i = 0; i < 4; i++)
                    AdditionalParams.Add(Vector4.Zero);
                for (int i = 0; i < 5; i++)
                    AdditionalTextures.Add(null);
            }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
            {
                AdditionalParams.Add(Vector4.Zero);
            }

            // vector params
            foreach (dynamic vectorParam in material.VectorParameters)
            {
                string paramName = vectorParam.ParameterName;

                // MEA / DAI
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition)
                {
                    if (paramName.Equals("Tint_Color_A", StringComparison.OrdinalIgnoreCase) || paramName.Equals("Tint_01", StringComparison.OrdinalIgnoreCase)
                        || paramName.Equals("TintColor1", StringComparison.OrdinalIgnoreCase) || paramName.Equals("OverridableTint1", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorA.X = vectorParam.Value.x;
                        TintColorA.Y = vectorParam.Value.y;
                        TintColorA.Z = vectorParam.Value.z;
                    }
                    else if (paramName.Equals("Tint_Color_B", StringComparison.OrdinalIgnoreCase) || paramName.Equals("Tint_02", StringComparison.OrdinalIgnoreCase)
                        || paramName.Equals("TintColor2", StringComparison.OrdinalIgnoreCase) || paramName.Equals("OverridableTint2", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorB.X = vectorParam.Value.x;
                        TintColorB.Y = vectorParam.Value.y;
                        TintColorB.Z = vectorParam.Value.z;
                    }
                    else if (paramName.Equals("Tint_Color_C", StringComparison.OrdinalIgnoreCase) || paramName.Equals("Tint_03", StringComparison.OrdinalIgnoreCase)
                        || paramName.Equals("TintColor3", StringComparison.OrdinalIgnoreCase) || paramName.Equals("OverridableTint3", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorC.X = vectorParam.Value.x;
                        TintColorC.Y = vectorParam.Value.y;
                        TintColorC.Z = vectorParam.Value.z;
                    }
                    else if (paramName.Equals("TintColor4", StringComparison.OrdinalIgnoreCase) || paramName.Equals("OverridableTint4", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorD.X = vectorParam.Value.x;
                        TintColorD.Y = vectorParam.Value.y;
                        TintColorD.Z = vectorParam.Value.z;
                    }
                    else if (paramName.Equals("TintWeights", StringComparison.OrdinalIgnoreCase) || paramName.Equals("OverridableTintWeights", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorA.W = vectorParam.Value.x;
                        TintColorB.W = vectorParam.Value.y;
                        TintColorC.W = vectorParam.Value.z;
                        TintColorD.W = vectorParam.Value.w;
                    }

                    // MEA Only
                    else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
                    { 
                        if (paramName.Equals("MP_Light", StringComparison.OrdinalIgnoreCase))
                        {
                            Vector4 v = AdditionalParams[0];

                            v.X = vectorParam.Value.x;
                            v.Y = vectorParam.Value.y;
                            v.Z = vectorParam.Value.z;

                            AdditionalParams[0] = v;
                        }
                        else if (paramName.Equals("Emissive_Intensity", StringComparison.OrdinalIgnoreCase) || paramName.Equals("EmissiveIntensity", StringComparison.OrdinalIgnoreCase))
                        {
                            Vector4 v = AdditionalParams[0];
                            v.W = vectorParam.Value.x;
                            AdditionalParams[0] = v;
                        }
                    }
                }

                else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst)
                {
                    // MEC
                    if (paramName.Equals("DiffuseColor", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorA = new SharpDX.Vector4(
                            vectorParam.Value.x, vectorParam.Value.y, vectorParam.Value.z, 0.0f
                            );
                        TintColorB.Z = 1.0f;
                    }
                    else if (paramName.Equals("NormalDetailStrength", StringComparison.OrdinalIgnoreCase))
                        TintColorB.X = vectorParam.Value.x;
                    else if (paramName.Equals("NormalDetailTiling", StringComparison.OrdinalIgnoreCase))
                        TintColorB.Y = vectorParam.Value.x;
                }

                else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    // SWBF2
                    if (paramName.Equals("Markings_ChannelSelection", StringComparison.OrdinalIgnoreCase))
                        TintColorA.W = vectorParam.Value.x;
                    else if (paramName.Equals("Markings_CamoTextureTiling", StringComparison.OrdinalIgnoreCase))
                        TintColorC.W = vectorParam.Value.x;
                    else if (paramName.Equals("Markings_Color", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorB.X = vectorParam.Value.x;
                        TintColorB.Y = vectorParam.Value.y;
                        TintColorB.Z = vectorParam.Value.z;
                    }
                    else if (paramName.Equals("Markings_Color2", StringComparison.OrdinalIgnoreCase))
                    {
                        TintColorC.X = vectorParam.Value.x;
                        TintColorC.Y = vectorParam.Value.y;
                        TintColorC.Z = vectorParam.Value.z;
                    }
                    else if (paramName.Equals("ReflectionAO"))
                        TintColorD.W = vectorParam.Value.x;
                    else if (paramName.Equals("Detail_Tiling"))
                        AdditionalParams[0] = SharpDXUtils.FromVec4(vectorParam.Value);
                    else if (paramName.Equals("NormalDetail_Intensity"))
                        AdditionalParams[1] = SharpDXUtils.FromVec4(vectorParam.Value);
                    else if (paramName.Equals("SmoothnessDetail_Intensity"))
                        AdditionalParams[2] = SharpDXUtils.FromVec4(vectorParam.Value);
                }
            }

            // bool params
            foreach (dynamic boolParam in material.BoolParameters)
            {
                string paramName = boolParam.ParameterName;

                // SWBF2
                if (paramName.Equals("Markings_Use2Masks", StringComparison.OrdinalIgnoreCase))
                    TintColorB.W = (boolParam.Value == true) ? 1.0f : 0.0f;
                else if (paramName.Equals("Markings_UseTexture", StringComparison.OrdinalIgnoreCase))
                    TintColorA.Z = (boolParam.Value == true) ? 1.0f : 0.0f;
                else if (paramName.Equals("UV_Switch"))
                    TintColorD.X = (boolParam.Value == true) ? 0.0f : 1.0f;
            }

            // Madden19/20
            if (section.MeshSection.TangentSpaceCompressionType == TangentSpaceCompressionType.TangentSpaceCompression_Matrix)
                CustomParam2 = 1;

            EbxAssetEntry shaderAsset = App.AssetManager.GetEbxEntry(material.Shader.External.FileGuid);
            if (shaderAsset == null)
                return;

            ShaderResourceView DiffuseTexture = null;
            ShaderResourceView NormTexture = null;
            ShaderResourceView MaskTexture = null;
            ShaderResourceView TintTexture = null;

            // texture params
            foreach (dynamic textureParam in material.TextureParameters)
            {
                string paramName = textureParam.ParameterName;
                PointerRef value = textureParam.Value;

                // MEA
                if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda))
                {
                    if (paramName.Equals("Diffuse") || paramName.Equals("BaseColor") || paramName.Equals("diff_mean")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("Norm") || paramName.Equals("Normal") || paramName.Equals("norm_mean"))
                    {
                        NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        if (NormTexture != null && NormTexture.Description.Format == SharpDX.DXGI.Format.BC5_UNorm)
                            CustomParam2 = 1;
                    }
                    else if (paramName.Equals("Tint") || paramName.Equals("Tint_Mask"))
                    {
                        TintTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = (CustomParam1 < 3) ? 2 : CustomParam1;
                    }
                    else if (paramName.Equals("Mask"))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = (CustomParam1 < 2) ? 1 : CustomParam1;
                    }
                    else if (paramName.Equals("ASM_Mask"))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = 3;
                    }

                    if (shaderAsset.Filename.Contains("body") || shaderAsset.Filename.Contains("skin"))
                        CustomParam1 = 0;
                }

                // MEC
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.MirrorsEdgeCatalyst))
                {
                    if (paramName.StartsWith("Diffuse")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); TintColorA.W = 1.0f; }
                    else if (paramName.StartsWith("Normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("RSM"))
                    {
                        if (MaskTexture == null)
                        {
                            MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                            CustomParam1 = 1;
                        }
                    }
                    else if (paramName.Equals("Opacity_SSS_Metal_Trans"))
                    {
                        TintTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam2 = 1;
                    }
                }

                // SWBF
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefront))
                {
                    paramName = paramName.ToLower();
                    if (shaderAsset != null)
                    {
                        CustomParam3 = 8000;
                        if (shaderAsset.Filename.Contains("ss_weapons") || shaderAsset.Filename.Contains("ss_characters"))
                            CustomParam3 = 0;
                        if (shaderAsset.Filename.Contains("2uv"))
                            CustomParam3 += 1;
                    }

                    if (paramName.Contains("cs") || paramName.StartsWith("cw") || paramName.Equals("basecolor")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("normal") || paramName.StartsWith("nw", StringComparison.OrdinalIgnoreCase) || paramName.StartsWith("ns", StringComparison.OrdinalIgnoreCase) || paramName.Contains("nrg"))
                    {
                        NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        if (NormTexture != null)
                        {
                            if (NormTexture.Description.Format == SharpDX.DXGI.Format.BC5_UNorm)
                                CustomParam2 = 1;
                            else
                            {
                                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(value.External.FileGuid);
                                if (entry != null)
                                {
                                    if (entry.Filename.Contains("_nw") || paramName.StartsWith("nw") || (shaderAsset.Filename.Contains("ss_characters") && entry.Filename.Contains("_nm")))
                                    {
                                        CustomParam2 = 1;
                                    }
                                    else if (entry.Filename.Contains("_rgb"))
                                    {
                                        CustomParam2 = 1;
                                    }
                                    else if (paramName.Contains("nrg"))
                                    {
                                        CustomParam2 = 1;
                                    }
                                    else if (entry.Filename.Contains("_nm"))
                                    {
                                        CustomParam2 = 3;
                                    }

                                    foreach (dynamic boolParam in material.BoolParameters)
                                    {
                                        if (boolParam.ParameterName == "SelectNormal" && boolParam.Value == true)
                                            CustomParam2 = 2;
                                    }
                                }
                            }
                        }
                    }
                    else if (paramName.Contains("msrao"))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = 3;
                    }
                    else if (paramName.Contains("msr"))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = 2;
                    }
                    else if (paramName.Contains("msw"))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = 1;
                    }
                }

                // Fifa
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17, ProfileVersion.Fifa18,
                    ProfileVersion.Fifa19, ProfileVersion.Fifa20,
                    ProfileVersion.NeedForSpeedHeat, ProfileVersion.Fifa21,
                    ProfileVersion.Fifa22, ProfileVersion.NeedForSpeedUnbound))
                {
                    paramName = paramName.ToLower();
                    if (paramName.StartsWith("colortexture") || paramName.StartsWith("diffuse") || paramName.Contains("basecolor"))
                    {
                        if (DiffuseTexture == null)
                            DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                    }
                    else if (paramName.StartsWith("seampatternnorm") || paramName.StartsWith("normalclamp") || paramName.StartsWith("normal") || paramName.StartsWith("basenormal") || paramName.StartsWith("nsm"))
                    {
                        if (NormTexture == null)
                        {
                            NormTexture = (shaderAsset.Filename.Equals("character_face_preset"))
                                ? state.TextureLibrary.LoadTextureAsset(App.AssetManager.GetEbxEntry("content/character/player/common/head_common_0_normal").Guid)
                                : state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                            if (shaderAsset.Filename.Equals("character_eye_preset"))
                                CustomParam1 = 1;
                        }
                    }
                    else if (paramName.StartsWith("coeff"))
                    {
                        MaskTexture = (shaderAsset.Filename.Equals("character_face_preset"))
                            ? state.TextureLibrary.LoadTextureAsset(App.AssetManager.GetEbxEntry("content/character/player/common/head_common_0_coeff").Guid)
                            : state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                    }
                    else if (paramName.StartsWith("specmask"))
                    {
                        TintTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam1 = 1;
                    }
                }

                // Madden
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Madden19, ProfileVersion.Madden20, ProfileVersion.Madden21, ProfileVersion.Madden22, ProfileVersion.Madden23))
                {
                    paramName = paramName.ToLower();
                    if (paramName.StartsWith("colortexture")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.StartsWith("seampatternnorm") || paramName.StartsWith("normalclamp") || paramName.StartsWith("normal") || paramName.StartsWith("basenormal") || paramName.StartsWith("nsm"))
                    {
                        if (NormTexture == null)
                        {
                            NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                            if (paramName.StartsWith("nsm"))
                                CustomParam1 = 2;
                        }
                    }
                    else if (paramName.StartsWith("rsm") || paramName.StartsWith("r_s_ssr_t"))
                    {
                        if (MaskTexture == null)
                        {
                            CustomParam1 = 1;
                            MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                            if (paramName.StartsWith("r_s_ssr_t"))
                                CustomParam1 = 0;
                        }
                    }
                }

                // BF1
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield1))
                {
                    if (paramName.StartsWith("BaseColor") || paramName.StartsWith("Diffuse")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("Normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // SWBF2
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.StarWarsSquadrons))
                {
                    string shaderFilename = (shaderAsset != null) ? shaderAsset.Filename.ToLower() : "";
                    if (shaderFilename.Contains("2uv") || shaderFilename.Contains("ss_vehicle") || shaderFilename.Contains("ss_weapons") || shaderFilename.Contains("ss_propspreset"))
                    {
                        CustomParam3 = 1;
                        foreach (dynamic condParam in material.ConditionalParameters)
                        {
                            EbxAssetEntry conditionalAsset = App.AssetManager.GetEbxEntry((Guid)condParam.ConditionalAsset.External.FileGuid);
                            if (conditionalAsset != null && conditionalAsset.Filename.Equals("ess_uvsetamount", StringComparison.OrdinalIgnoreCase))
                            {
                                if (condParam.Value == "1UV")
                                    CustomParam3 = 0;
                            }
                        }
                    }

                    if (!paramName.Contains("/") && ((paramName.StartsWith("C") || paramName.StartsWith("_C") || paramName.StartsWith("Base") || paramName.Contains("_CS") || paramName.Contains("_Base") || paramName.Contains("CS"))))
                    {
                        if (DiffuseTexture == null)
                        {
                            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(value.External.FileGuid);
                            if (entry != null)
                            {
                                string filename = entry.Filename.ToLower();

                                if (filename.EndsWith("_c") || filename.EndsWith("_cm"))
                                    AdditionalParams[3] = new Vector4(1.0f, AdditionalParams[3].Y, 0, 0);

                                DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                            }
                        }
                    }
                    else if ((paramName.StartsWith("N") || paramName.Contains("_NAM_texcoord0") || paramName.Contains("_Normal") || paramName.Contains("NAO")) && !paramName.Contains("Array"))
                    {
                        EbxAssetEntry entry = App.AssetManager.GetEbxEntry(value.External.FileGuid);
                        if (entry != null)
                        {
                            string filename = entry.Filename.ToLower();

                            if (filename.Contains("_nm"))
                            {
                                CustomParam1 = 1;
                            }
                            else if (filename.Contains("_nam"))
                            {
                                CustomParam1 = (shaderFilename.Contains("ss_weapons") || shaderFilename.Contains("ss_vehicle")) ? 1 : 2;
                                CustomParam2 = (shaderFilename.Contains("ss_weapons") || shaderFilename.Contains("ss_vehicle")) ? 2 : 0;
                            }
                            else if (filename.Contains("_nma"))
                            {
                                CustomParam2 = 3;
                            }
                            else if (filename.Contains("_naosl"))
                            {
                                if (shaderAsset != null)
                                {
                                    CustomParam2 = shaderFilename.Contains("nonmetallic") ? 2 : 0;
                                    CustomParam1 = (CustomParam2 == 0) ? 1 : 0;
                                }
                            }
                        }
                        NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                    }
                    else if ((paramName.Equals("AOSlice")))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        CustomParam2 = 1;
                    }
                    else if (paramName.Equals("Markings_Texture") || paramName.Equals("Markings_Texture2") || paramName.Equals("Markings_CamoTexture"))
                    {
                        ShaderResourceView srv = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        if (paramName.Equals("Markings_Texture")) { AdditionalTextures[0] = srv; TintColorA.X = 1.0f; }
                        else if (paramName.Equals("Markings_Texture2")) { AdditionalTextures[1] = srv; TintColorA.Y = 1.0f; }
                        else { AdditionalTextures[2] = srv; }
                    }
                    else if (paramName.Equals("NormalDetailTextureArray"))
                    {
                        AdditionalTextures[3] = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                    }
                    else if (paramName.Contains("RSSSAO"))
                    {
                        AdditionalTextures[4] = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        AdditionalParams[3] = new Vector4(AdditionalParams[3].X, 1.0f, 0, 0);
                    }
                }

                // PVZ2
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare2))
                {
                    if (paramName.Contains("Diffuse") || paramName.Contains("Color")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("Normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("ASM")) { MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // PVZ
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare))
                {
                    paramName = paramName.ToLower();
                    if (paramName.Equals("diffuse")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("psa")) { MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // DAI/BF4
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4))
                {
                    paramName = paramName.ToLower();
                    if (paramName.Equals("diffuse") || paramName.Equals("d1_diffuse") || paramName.Equals("diffuse_alpha")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("normal") || paramName.Equals("n1_normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Equals("specular") || paramName.Equals("s1_specular"))
                    {
                        MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                        if (shaderAsset.Filename.Contains("skin") || shaderAsset.Filename.Contains("nometal"))
                        {
                            CustomParam1 = 1;
                        }
                    }
                    else if (paramName.Equals("tint")) { TintTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // NFS Payback
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedPayback))
                {
                    paramName = paramName.ToLower();
                    if (paramName.StartsWith("color") || paramName.Contains("diffuse")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.StartsWith("normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.StartsWith("coeff")) { MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // Anthem
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem))
                {
                    if (paramName.Contains("Diffuse") || paramName.Contains("BaseColor")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("Normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("AMMS")) { MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // BFV
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
                {
                    if (paramName.Contains("BaseColor")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("Normal"))
                    {
                        CustomParam1 = 1;
                        EbxAssetEntry entry = App.AssetManager.GetEbxEntry(value.External.FileGuid);
                        if (entry.Name.ToLower().EndsWith("naosl"))
                            CustomParam1 = 0;
                        NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                    }
                }

                // NFS14
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedRivals))
                {
                    if (paramName.Contains("Diffuse")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("Normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                // PVZ3
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesBattleforNeighborville))
                {
                    paramName = paramName.ToLower();
                    if (paramName.Contains("color")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }

                else if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042))
                {
                    if (paramName.StartsWith("_CS") || paramName.Contains("Color")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("NX") || paramName.Contains("NMT") || paramName.Contains("Normal"))
                    {
                        EbxAssetEntry entry = App.AssetManager.GetEbxEntry(value.External.FileGuid);
                        if (entry != null)
                        {
                            string filename = entry.Filename;

                            if (filename.EndsWith("NMT"))
                            {
                                CustomParam1 = 1;
                            }
                            else if (filename.Contains("NX"))
                            {
                                CustomParam1 = 3;
                                CustomParam2 = 2;
                            }
                        }
                        NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid);
                    }
                }

                // Anything else
                else
                {
                    if (paramName.Contains("Diffuse")) { DiffuseTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("Normal")) { NormTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                    else if (paramName.Contains("Specular")) { MaskTexture = state.TextureLibrary.LoadTextureAsset(value.External.FileGuid); }
                }
            }

            if (DiffuseTexture == null)
            {
                ShaderParameter texParam = section.Permutation.PixelTextures[0];
                DiffuseTexture = state.TextureLibrary.LoadTextureAsset(texParam.StringValue);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    AdditionalParams[3] = new Vector4(1.0f, AdditionalParams[3].Y, 0, 0);
            }
            if (NormTexture == null)
            {
                NormTexture = state.TextureLibrary.LoadTextureAsset(App.AssetManager.GetEbxEntry(ProfilesLibrary.DefaultNormals).Guid);
            }
            if (MaskTexture == null)
            {
                MaskTexture = state.TextureLibrary.LoadTextureAsset(App.AssetManager.GetEbxEntry(ProfilesLibrary.DefaultMask).Guid);
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedHeat, ProfileVersion.NeedForSpeedUnbound))
            {
                bool tangentSpace = false;
                foreach (var elem in section.MeshSection.GeometryDeclDesc[0].Elements)
                {
                    if (elem.Usage == VertexElementUsage.TangentSpace)
                    {
                        tangentSpace = true;
                        break;
                    }
                }

                if (!tangentSpace)
                {
                    CustomParam2 = 1.0f;
                }
            }

            // handle SRGB switch
            if (DiffuseTexture != null)
                SRGB = SharpDX.DXGI.FormatHelper.IsSRgb(DiffuseTexture.Description.Format) ? 1.0f : 0.0f;

            ShaderResourceView[] srvs = section.PixelTextures.ToArray();

            section.PixelTextures.Clear();
            section.PixelTextures.Add(DiffuseTexture);
            section.PixelTextures.Add(NormTexture);
            section.PixelTextures.Add(MaskTexture);
            section.PixelTextures.Add(TintTexture);
            section.PixelTextures.AddRange(AdditionalTextures);

            int size = section.Permutation.PixelConstantsSize;
            size = ((size + 15) / 16) * 16;

            byte[] outData = null;
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(CustomParam1);
                writer.Write(CustomParam2);
                writer.Write(CustomParam3);
                writer.Write(SRGB);
                writer.Write(0.0f); writer.Write(0.0f); writer.Write(0.0f); writer.Write(0.0f);
                writer.Write(TintColorA.X); writer.Write(TintColorA.Y); writer.Write(TintColorA.Z); writer.Write(TintColorA.W);
                writer.Write(TintColorB.X); writer.Write(TintColorB.Y); writer.Write(TintColorB.Z); writer.Write(TintColorB.W);
                writer.Write(TintColorC.X); writer.Write(TintColorC.Y); writer.Write(TintColorC.Z); writer.Write(TintColorC.W);
                writer.Write(TintColorD.X); writer.Write(TintColorD.Y); writer.Write(TintColorD.Z); writer.Write(TintColorD.W);
                for (int i = 0; i < AdditionalParams.Count; i++)
                {
                    writer.Write(AdditionalParams[i].X);
                    writer.Write(AdditionalParams[i].Y);
                    writer.Write(AdditionalParams[i].Z);
                    writer.Write(AdditionalParams[i].W);
                }
                while (writer.Position < size)
                    writer.Write((byte)0x00);

                outData = writer.ToByteArray();;
            }

            if (section.PixelParameters != null && size == section.PixelParameters.Description.SizeInBytes)
            {
                DeviceContext context = state.Device.ImmediateContext;

                context.MapSubresource(section.PixelParameters, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                stream.Write(outData, 0, outData.Length);
                context.UnmapSubresource(section.PixelParameters, 0);
            }
            else
            {
                using (DataStream stream = new DataStream(size, false, true))
                {
                    stream.Write(outData, 0, outData.Length);
                    stream.Position = 0;

                    if (section.PixelParameters != null)
                        section.PixelParameters.Dispose();
                    section.PixelParameters = new D3D11.Buffer(state.Device, stream, size, ResourceUsage.Dynamic, BindFlags.ConstantBuffer,
                        CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
                }
            }

            state.TextureLibrary.UnloadTextures(srvs);
        }

        /// <summary>
        /// Creates the mandatory fallback shader to be used where a custom shader is not provided
        /// </summary>
        public static ShaderPermutation CreateFallback(Device device, Shader inParent)
        {
            ShaderPermutation perm = new ShaderPermutation(inParent);

            perm.vertexShader = FrostyShaderDb.GetShaderWithSignature<VertexShader>(device, ((int)ProfilesLibrary.DataVersion).ToString(), out ShaderSignature signature);
            perm.pixelShader = FrostyShaderDb.GetShader<PixelShader>(device, ((int)ProfilesLibrary.DataVersion).ToString());
            perm.inputLayout = new InputLayout(device, signature.Data, new InputElement[]
            {
                new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0),
                new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("TANGENT", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("BINORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 0),
                new InputElement("TEXCOORD", 1, SharpDX.DXGI.Format.R32G32_Float, 0),
                new InputElement("TEXCOORD", 2, SharpDX.DXGI.Format.R32G32_Float, 0),
                new InputElement("COLOR", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("COLOR", 1, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("BLENDINDICES", 0, SharpDX.DXGI.Format.R32G32B32A32_UInt, 0),
                new InputElement("BLENDINDICES", 1, SharpDX.DXGI.Format.R32G32B32A32_UInt, 0),
                new InputElement("BLENDWEIGHT", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("BLENDWEIGHT", 1, SharpDX.DXGI.Format.R32G32B32A32_Float, 0),
                new InputElement("TANGENTSPACE", 0, SharpDX.DXGI.Format.R32_UInt, 0)
            });
            perm.IsFallback = true;
            perm.IsSkinned = true;

            perm.PixelTextures.Add(new ShaderParameter("DiffuseTexture", ShaderParameterType.Tex2d, "Resources/Textures/DefaultDiffuse.dds"));
            perm.pixelSamplers.Add(D3DUtils.CreateSamplerState());

            perm.PixelParameters.Add(new ShaderParameter("CustomParam1", ShaderParameterType.Float));
            perm.PixelParameters.Add(new ShaderParameter("CustomParam2", ShaderParameterType.Float));
            perm.PixelParameters.Add(new ShaderParameter("CustomParam3", ShaderParameterType.Float));
            perm.PixelParameters.Add(new ShaderParameter("SRGB", ShaderParameterType.Float));
            perm.PixelParameters.Add(new ShaderParameter("OverlayColor", ShaderParameterType.Float4));
            perm.PixelParameters.Add(new ShaderParameter("TintColorA", ShaderParameterType.Float4));
            perm.PixelParameters.Add(new ShaderParameter("TintColorB", ShaderParameterType.Float4));
            perm.PixelParameters.Add(new ShaderParameter("TintColorC", ShaderParameterType.Float4));
            perm.PixelParameters.Add(new ShaderParameter("TintColorD", ShaderParameterType.Float4));
            perm.PixelConstantsSize = 160;

            perm.boneBuffer = new BoneBuffer(device, 100);

            return perm;
        }

        public void Dispose()
        {
            if (vertexShader != null)
            {
                vertexShader.Dispose();
                pixelShader.Dispose();
                inputLayout.Dispose();
                pixelSamplers.Clear();

                vertexShader = null;
                pixelShader = null;
                inputLayout = null;
            }
        }
    }
    public class Shader : IDisposable
    {
        private Dictionary<uint, ShaderPermutation> permutations = new Dictionary<uint, ShaderPermutation>();
        private Dictionary<string, uint> nameHashLookup = new Dictionary<string, uint>();
        public int RefCount = 0;

        public ShaderPermutation GetPermutation(GeometryDeclarationDesc geomDecl)
        {
            return GetPermutation(geomDecl.Hash);
        }

        public ShaderPermutation GetPermutation(uint hash)
        {
            if (!permutations.ContainsKey(hash))
                return null;
            return permutations[hash];
        }

        public void RemovePermutation(uint hash)
        {
            if (!permutations.ContainsKey(hash))
                return;
            permutations.Remove(hash);
        }

        /// <summary>
        /// Loads the xml descriptor for the specified shader and compiles all shader permutations if required
        /// </summary>
        public bool Load(string filename)
        {
            string path = "Shaders/" + filename;
            if (!File.Exists(path + ".xml"))
                return false;

            try
            {
                CultureInfo ci = new CultureInfo("en-US");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(new FileStream(path + ".xml", FileMode.Open, FileAccess.Read));

                XmlNode shaderNode = xmlDoc.FirstChild;
                if (shaderNode.Attributes["profile"] == null)
                    return false;

                string profile = shaderNode.Attributes["profile"].Value;
                if (!profile.Equals(ProfilesLibrary.ProfileName, StringComparison.OrdinalIgnoreCase) && profile != "*")
                    return false;

                XmlNode defPixelShader = shaderNode.SelectSingleNode("pixelshader");
                XmlNode permNodes = shaderNode.SelectSingleNode("permutations");

                foreach (XmlNode childNode in permNodes)
                {
                    string permName = "";
                    if (childNode.Attributes["name"] != null)
                        permName = childNode.Attributes["name"].Value;

                    // process vertex shader block
                    XmlNode vsNode = childNode.SelectSingleNode("vertexshader");
                    if (vsNode == null)
                        return false;

                    ShaderPermutation shaderPerm = ParseVertexLayouts(vsNode.SelectSingleNode("vertexlayout"), filename, permName);
                    if (childNode.Attributes["skinned"] != null)
                    {
                        shaderPerm.IsSkinned = bool.Parse(childNode.Attributes["skinned"].Value);
                        shaderPerm.MaxBonesPerVertex = 4;

                        if (childNode.Attributes["bonesPerVertex"] != null)
                            shaderPerm.MaxBonesPerVertex = int.Parse(childNode.Attributes["bonesPerVertex"].Value);
                    }

                    // process pixel shader block (either from permutation or default)
                    XmlNode psNode = childNode.SelectSingleNode("pixelshader") ?? defPixelShader;

                    if (psNode == null)
                        return false;

                    // @temp
                    if (psNode.Attributes["twoSided"] != null)
                        shaderPerm.IsTwoSided = bool.Parse(psNode.Attributes["twoSided"].Value);

                    XmlNode textureNode = psNode.SelectSingleNode("textures");
                    if (textureNode != null)
                    {
                        foreach (XmlNode texChild in textureNode.ChildNodes)
                        {
                            if (texChild.NodeType == XmlNodeType.Comment || texChild.NodeType == XmlNodeType.Whitespace)
                                continue;

                            string name = texChild.Attributes["name"].Value;
                            string type = texChild.Attributes["type"].Value;

                            string defValue = "";
                            if (texChild.Attributes["defaultValue"] != null)
                                defValue = texChild.Attributes["defaultValue"].Value;

                            ShaderParameterType paramType = ShaderParameterType.Tex2d;
                            if (type == "2darray")
                                paramType = ShaderParameterType.Tex2dArray;
                            else if (type == "cube")
                                paramType = ShaderParameterType.TexCube;

                            shaderPerm.PixelTextures.Add(new ShaderParameter(name, paramType, defValue));
                        }
                    }
                    XmlNode samplersNode = psNode.SelectSingleNode("samplers");
                    if (samplersNode != null)
                    {
                        foreach (XmlNode samplerNode in samplersNode)
                        {
                            if (samplerNode.NodeType == XmlNodeType.Comment || samplerNode.NodeType == XmlNodeType.Whitespace)
                                continue;

                            Filter filter = Filter.MinMagMipLinear;
                            TextureAddressMode addressU = TextureAddressMode.Wrap;
                            TextureAddressMode addressV = TextureAddressMode.Wrap;
                            TextureAddressMode addressW = TextureAddressMode.Wrap;
                            float mipLodBias = 0.0f;
                            int maxAniso = 16;
                            float minLod = 0;
                            float maxLod = 20;

                            if (samplerNode.Attributes["filter"] != null) filter = (Filter)Enum.Parse(typeof(Filter), samplerNode.Attributes["filter"].Value);
                            if (samplerNode.Attributes["addressU"] != null) addressU = (TextureAddressMode)Enum.Parse(typeof(TextureAddressMode), samplerNode.Attributes["addressU"].Value);
                            if (samplerNode.Attributes["addressV"] != null) addressV = (TextureAddressMode)Enum.Parse(typeof(TextureAddressMode), samplerNode.Attributes["addressV"].Value);
                            if (samplerNode.Attributes["addressW"] != null) addressW = (TextureAddressMode)Enum.Parse(typeof(TextureAddressMode), samplerNode.Attributes["addressW"].Value);
                            if (samplerNode.Attributes["mipLodBias"] != null) mipLodBias = float.Parse(samplerNode.Attributes["mipLodBias"].Value, ci);
                            if (samplerNode.Attributes["maxAnisotropy"] != null) maxAniso = int.Parse(samplerNode.Attributes["maxAnisotropy"].Value);
                            if (samplerNode.Attributes["minLod"] != null) minLod = float.Parse(samplerNode.Attributes["minLod"].Value, ci);
                            if (samplerNode.Attributes["maxLod"] != null) maxLod = float.Parse(samplerNode.Attributes["maxLod"].Value, ci);

                            SamplerStateDescription desc = new SamplerStateDescription()
                            {
                                Filter = filter,
                                AddressU = addressU,
                                AddressV = addressV,
                                AddressW = addressW,
                                MipLodBias = mipLodBias,
                                MaximumAnisotropy = maxAniso,
                                MinimumLod = minLod,
                                MaximumLod = maxLod,
                                BorderColor = new Color4(0, 0, 0, 0),
                                ComparisonFunction = Comparison.Always
                            };

                            shaderPerm.PixelSamplerDescs.Add(desc);
                        }
                    }
                    XmlNode paramsNode = psNode.SelectSingleNode("parameters");
                    if (paramsNode != null)
                    {
                        foreach (XmlNode paramNode in paramsNode.ChildNodes)
                        {
                            if (paramNode.NodeType == XmlNodeType.Comment || paramNode.NodeType == XmlNodeType.Whitespace)
                                continue;

                            string name = paramNode.Attributes["name"].Value;
                            string type = paramNode.Attributes["type"].Value;
                            ShaderParameterType paramType = (ShaderParameterType)Enum.Parse(typeof(ShaderParameterType), type);

                            string defValue = "";
                            switch (paramType)
                            {
                                case ShaderParameterType.Bool: defValue = "false"; break;
                                case ShaderParameterType.Float: defValue = "0"; break;
                                case ShaderParameterType.Float2: defValue = "0,0"; break;
                                case ShaderParameterType.Float3: defValue = "0,0,0"; break;
                                case ShaderParameterType.Float4: defValue = "0,0,0,0";  break;
                            }

                            if (paramNode.Attributes["defaultValue"] != null)
                                defValue = paramNode.Attributes["defaultValue"].Value;

                            string[] defValueArr = defValue.Split(',');
                            ShaderParameter param = null;

                            int size = 0;
                            switch (paramType)
                            {
                                case ShaderParameterType.Bool:
                                    size = 1;
                                    param = new ShaderParameter(name, paramType, bool.Parse(defValueArr[0].Trim()));
                                    break;
                                case ShaderParameterType.Float:
                                    size = 4;
                                    param = new ShaderParameter(name, paramType, float.Parse(defValueArr[0].Trim(), ci));
                                    break;
                                case ShaderParameterType.Float2:
                                    size = 8;
                                    param = new ShaderParameter(name, paramType,
                                        float.Parse(defValueArr[0].Trim(), ci),
                                        float.Parse(defValueArr[1].Trim(), ci)
                                        );
                                    break;
                                case ShaderParameterType.Float3:
                                    size = 12;
                                    param = new ShaderParameter(name, paramType,
                                        float.Parse(defValueArr[0].Trim(), ci),
                                        float.Parse(defValueArr[1].Trim(), ci),
                                        float.Parse(defValueArr[2].Trim(), ci)
                                        );
                                    break;
                                case ShaderParameterType.Float4:
                                    size = 16;
                                    param = new ShaderParameter(name, paramType,
                                        float.Parse(defValueArr[0].Trim(), ci),
                                        float.Parse(defValueArr[1].Trim(), ci),
                                        float.Parse(defValueArr[2].Trim(), ci),
                                        float.Parse(defValueArr[3].Trim(), ci)
                                        );
                                    break;
                            }

                            shaderPerm.PixelParameters.Add(param);
                            shaderPerm.PixelConstantsSize += size;
                        }
                    }
                }
            }
            catch (Exception)
            {
                App.Logger.Log("Failed to load xml descriptor for shader '{0}'", filename);
                return false;
            }

            // now that we have permutations, lets see if they need to be compiled (only if HLSL exists)
            if (File.Exists(path + ".hlsl"))
            {
                long lastCompileTime = 0;
                if (File.Exists(path + ".bin"))
                {
                    using (NativeReader reader = new NativeReader(new FileStream(path + ".bin", FileMode.Open, FileAccess.Read)))
                        lastCompileTime = reader.ReadLong();
                }

                FileInfo fi = new FileInfo(path + ".hlsl");
                if (fi.LastWriteTime.ToFileTimeUtc() > lastCompileTime)
                {
                    bool repeat = true;
                    bool failed = false;
                    string errorString = "";

                    while (repeat)
                    {
                        failed = false;
                        using (NativeWriter writer = new NativeWriter(new FileStream(path + ".bin", FileMode.Create)))
                        {
                            writer.Write((long)0x00);
                            writer.Write(permutations.Count);

                            long tocOffset = writer.Position;
                            long dataOffset = (permutations.Count * 12) + 12;
                            int permIdx = 0;

                            // shader has been modified, lets recompile all permutations
                            foreach (ShaderPermutation permutation in permutations.Values)
                            {
                                writer.Position = (permIdx++ * 12) + tocOffset;
                                writer.Write(Fnv1.HashString(permutation.Name));
                                writer.Write(dataOffset);
                                writer.Position = dataOffset;

                                // zero element permutations not supported
                                if (permutation.GeometryDeclaration.ElementCount == 0)
                                    continue;

                                // now compile
                                List<ShaderMacro> macros = new List<ShaderMacro>();
                                CompilationResult result = null;

                                macros.Add(new ShaderMacro(permutation.Name, null));
                                if (permutation.IsSkinned)
                                {
                                    macros.Add(new ShaderMacro("SHADER_SKINNED", 1));
                                    macros.Add(new ShaderMacro("SHADER_BONES_PER_VERTEX", permutation.MaxBonesPerVertex));
                                }

                                string shaderCode = ConstructShaderForRenderPass("ShaderTemplate", path, permutation);
                                byte[] vsBytecode = null;
                                byte[] psBytecode = null;

                                // try to compile the vertex shader
                                result = ShaderBytecode.Compile(shaderCode, "VSMain", "vs_5_0", ShaderFlags.None, EffectFlags.None, macros.ToArray(), new ShaderIncludeHandler());
                                if (result.Bytecode == null)
                                {
                                    failed = true;
                                    errorString = string.Format("Failed to compile the specified shader.\r\n\r\nShader: {0}\r\nPermutation: {1}\r\n\r\nWould you like to retry?\r\n\r\n{2}", filename, permutation.Name, result.Message);
                                }
                                else
                                {
                                    vsBytecode = result.Bytecode;
                                }

                                if (!failed)
                                {
                                    // now try to compile the pixel shader
                                    result = ShaderBytecode.Compile(shaderCode, "PSMain", "ps_5_0", ShaderFlags.None, EffectFlags.None, macros.ToArray(), new ShaderIncludeHandler());
                                    if (result.Bytecode == null)
                                    {
                                        failed = true;
                                        errorString = string.Format("Failed to compile the specified shader.\r\n\r\nShader: {0}\r\nPermutation: {1}\r\n\r\nWould you like to retry?\r\n\r\n{2}", filename, permutation.Name, result.Message);
                                    }
                                    else
                                    {
                                        psBytecode = result.Bytecode;
                                    }
                                }

                                if (!failed)
                                {
                                    writer.Write(vsBytecode.Length);
                                    writer.Write(psBytecode.Length);
                                    writer.Write(vsBytecode);
                                    writer.Write(psBytecode);

                                    dataOffset = writer.Position;
                                }
                                else
                                {
                                    if (FrostyMessageBox.Show(errorString, "Frosty Editor", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                        repeat = false;
                                    break;
                                }
                            }

                            if (!failed)
                            {
                                writer.Position = 0;
                                writer.Write(DateTime.Now.ToFileTimeUtc());
                                repeat = false;
                            }
                        }
                    }

                    if (failed)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Uses the specified template and hlsl to construct a full shader for a render pass
        /// </summary>
        private string ConstructShaderForRenderPass(string template, string path, ShaderPermutation permutation)
        {
            Uri uri = new Uri("pack://application:,,,/Shaders/" + template + ".txt");
            string shaderCode = "";

            // start with the autogenerated fields
            StringBuilder sb = new StringBuilder();
            {
                int streamIdx = 0;
                int totalStride = 0;

                // vertex shader inputs
                sb.AppendLine("struct VertexShaderInput {");
                foreach (GeometryDeclarationDesc.Stream stream in permutation.GeometryDeclaration.Streams)
                {
                    int currentStride = 0;
                    foreach (GeometryDeclarationDesc.Element elem in permutation.GeometryDeclaration.Elements)
                    {
                        if (currentStride >= totalStride && currentStride < (totalStride + stream.VertexStride))
                        {
                            if (elem.Usage == VertexElementUsage.Unknown)
                                continue;

                            string format = elem.Format.ToString().ToLower();
                            switch (elem.Format)
                            {
                                case VertexElementFormat.UByte4: format = "uint4"; break;
                                case VertexElementFormat.UShort4: format = "uint4"; break;

                                case VertexElementFormat.Short4N: format = "float4"; break;
                                case VertexElementFormat.UByte4N: format = "float4"; break;
                                case VertexElementFormat.Short2N: format = "float2"; break;
                            }

                            string semanticName = elem.Usage.ToString().ToUpper();
                            sb.AppendLine(string.Format("{0} {1} : {2};", format, elem.Usage.ToString(), semanticName));
                        }

                        currentStride += elem.Size;
                    }

                    totalStride += stream.VertexStride;
                    streamIdx++;
                }
                sb.AppendLine("};");

                // external pixel parameters
                sb.AppendLine("cbuffer ExternalPixelParameters : register(b2) {");
                foreach (ShaderParameter param in permutation.PixelParameters)
                {
                    sb.AppendLine(string.Format("{0} {1};", param.Type.ToString().ToLower(), param.Name));
                }
                sb.AppendLine("};");

                // pixel textures
                int textureId = 1;
                foreach (ShaderParameter param in permutation.PixelTextures)
                {
                    string textureFormat = "";
                    switch(param.Type)
                    {
                        case ShaderParameterType.Tex2d: textureFormat = "Texture2D"; break;
                        case ShaderParameterType.Tex2dArray: textureFormat = "Texture2DArray"; break;
                        case ShaderParameterType.TexCube: textureFormat = "TextureCube"; break;
                    }
                    sb.AppendLine(string.Format("{0} {1} : register(t{2});", textureFormat, param.Name, textureId++));
                }

                // pixel samplers
                int samplerId = 1;
                for (int i = 0; i < permutation.PixelSamplerDescs.Count; i++)
                {
                    sb.AppendLine(string.Format("SamplerState sampler{0}_s : register(s{0});", samplerId++));
                }
            }
            shaderCode = sb.ToString();

            // read in the template
            using (Stream stream = Application.GetResourceStream(uri).Stream)
            {
                using (TextReader reader = new StreamReader(stream))
                    shaderCode += reader.ReadToEnd();
            }

            // now read in the shader
            using (TextReader reader = new StreamReader(new FileStream(path + ".hlsl", FileMode.Open, FileAccess.Read)))
            {
                string tmp = reader.ReadToEnd();
                shaderCode = shaderCode.Replace("<ShaderCode>", tmp);
            }

            return shaderCode;
        }

        private ShaderPermutation ParseVertexLayouts(XmlNode vertexLayout, string filename, string perm)
        {
            GeometryDeclarationDesc geomDecl = new GeometryDeclarationDesc
            {
                Elements = new GeometryDeclarationDesc.Element[GeometryDeclarationDesc.MaxElements],
                Streams = new GeometryDeclarationDesc.Stream[GeometryDeclarationDesc.MaxStreams]
            };


            for (int i = 0; i < GeometryDeclarationDesc.MaxElements; i++)
                geomDecl.Elements[i].Offset = 0xFF;

            int index = 0;
            foreach (XmlNode layoutElement in vertexLayout.ChildNodes)
            {
                if (layoutElement.NodeType == XmlNodeType.Comment)
                    continue;

                GeometryDeclarationDesc.Element elem = new GeometryDeclarationDesc.Element
                {
                    Usage = (VertexElementUsage)Enum.Parse(typeof(VertexElementUsage), layoutElement.Attributes["usage"].Value),
                    Format = (VertexElementFormat)Enum.Parse(typeof(VertexElementFormat), layoutElement.Attributes["format"].Value),
                    StreamIndex = 0
                };

                if (layoutElement.Attributes["stream"] != null)
                    elem.StreamIndex = byte.Parse(layoutElement.Attributes["stream"].Value);

                elem.Offset = geomDecl.Streams[elem.StreamIndex].VertexStride;

                geomDecl.Elements[index++] = elem;
                geomDecl.ElementCount++;

                geomDecl.Streams[elem.StreamIndex].Classification = VertexElementClassification.PerVertex;
                geomDecl.Streams[elem.StreamIndex].VertexStride += (byte)elem.Size;

                if (elem.StreamIndex > geomDecl.StreamCount - 1)
                    geomDecl.StreamCount = (byte)(elem.StreamIndex + 1);                
            }

            ShaderPermutation permutation = new ShaderPermutation(perm, geomDecl, filename, this);

            permutations.Add(geomDecl.Hash, permutation);
            nameHashLookup.Add(perm, geomDecl.Hash);

            return permutation;
        }

        public static Shader CreateFallback(Device device)
        {
            Shader shader = new Shader();
            shader.permutations.Add(0, ShaderPermutation.CreateFallback(device, shader));
            return shader;
        }

        public void Dispose()
        {
            foreach (ShaderPermutation perm in permutations.Values)
                perm.Dispose();
        }
    }
    #endregion

    #region -- Meshes --
    public class RenderCreateState
    {
        public Device Device { get; private set; }
        public TextureLibrary TextureLibrary { get; private set; }
        public ShaderLibrary ShaderLibrary { get; private set; }

        public RenderCreateState(Device inDevice, TextureLibrary inTextureLibrary, ShaderLibrary inShaderLibrary)
        {
            Device = inDevice;
            TextureLibrary = inTextureLibrary;
            ShaderLibrary = inShaderLibrary;
        }
    }

    #region -- Mesh Materials --
    [EbxClassMeta(EbxFieldType.Struct)]
    public class MeshMaterial
    {
        [EbxFieldMeta(EbxFieldType.Pointer, "ShaderGraph")]
        public PointerRef BaseShader => Shader;
        public PointerRef Shader;
        [EbxFieldMeta(0x00, 0, typeof(object), true, 0)]
        public List<dynamic> VectorParameters { get; set; } = new List<dynamic>();
        [EbxFieldMeta(0x00, 0, typeof(object), true, 0)]
        public List<dynamic> TextureParameters { get; set; } = new List<dynamic>();
        [EbxFieldMeta(0x00, 0, typeof(object), true, 0)]
        public List<dynamic> BoolParameters { get; set; } = new List<dynamic>();
        [EbxFieldMeta(0x00, 0, typeof(object), true, 0)]
        public List<dynamic> ConditionalParameters { get; set; } = new List<dynamic>();
        public Guid Guid;

        /// <summary>
        /// Sets the specified texture parameter
        /// </summary>
        public void SetTextureParameter(string paramName, EbxAssetEntry texture)
        {
            dynamic texParam = TextureParameters.Find((dynamic a) => paramName.Equals(a.ParameterName, StringComparison.OrdinalIgnoreCase));
            if (texParam == null)
            {
                texParam = TypeLibrary.CreateObject("TextureShaderParameter");
                texParam.ParameterName = paramName;
                TextureParameters.Add(texParam);
            }
            if (texture == null)
                texParam.Value = new PointerRef();
            else
                texParam.Value = new PointerRef(new EbxImportReference() { FileGuid = texture.Guid });
        }

        /// <summary>
        /// Sets the specified vector parameter using a Vec4
        /// </summary>
        public void SetVectorParameter(string paramName, dynamic /* Vec4 */ vecValue)
        {
            dynamic param = VectorParameters.Find((dynamic a) => paramName.Equals(a.ParameterName, StringComparison.OrdinalIgnoreCase));
            if (param == null)
            {
                param = TypeLibrary.CreateObject("VectorShaderParameter");
                param.ParameterName = paramName;
                VectorParameters.Add(param);
            }
            param.Value = vecValue;
        }

        /// <summary>
        /// Sets the specified vector parameter using a Vector4
        /// </summary>
        //public void SetVectorParameter(string paramName, Vector4 vecValue)
        //{
        //    dynamic param = VectorParameters.Find((dynamic a) => paramName.Equals(a.ParameterName, StringComparison.OrdinalIgnoreCase));
        //    if (param == null)
        //    {
        //        param = TypeLibrary.CreateObject("VectorShaderParameter");
        //        param.ParameterName = paramName;
        //        VectorParameters.Add(param);
        //    }
        //    param.Value = TypeLibrary.CreateObject("Vec4");
        //    param.Value.x = vecValue.X;
        //    param.Value.y = vecValue.Y;
        //    param.Value.z = vecValue.Z;
        //    param.Value.w = vecValue.W;
        //}

        /// <summary>
        /// Sets the specified vector parameter using a series of floats
        /// </summary>
        public void SetVectorParameter(string paramName, float x, float y, float z, float w)
        {
            dynamic param = VectorParameters.Find((dynamic a) => paramName.Equals(a.ParameterName, StringComparison.OrdinalIgnoreCase));
            if (param == null)
            {
                param = TypeLibrary.CreateObject("VectorShaderParameter");
                param.ParameterName = paramName;
                VectorParameters.Add(param);
            }
            param.Value = TypeLibrary.CreateObject("Vec4");
            param.Value.x = x;
            param.Value.y = y;
            param.Value.z = z;
            param.Value.w = w;
        }
    }
    public class MeshMaterialCollection : IEnumerable<MeshMaterial>
    {
        [DisplayName("MeshMaterialCollection")]
        [EbxClassMeta(EbxFieldType.Struct)]
        public class Container
        {
            [EbxFieldMeta(0x02, 0, typeof(object), true, 0)]
            public List<MeshMaterial> Materials => parent.materials;
            private MeshMaterialCollection parent;

            public Container(MeshMaterialCollection inParent)
            {
                parent = inParent;
            }
        }

        private List<MeshMaterial> materials = new List<MeshMaterial>();

        public MeshMaterial this[int idx] => materials[idx];
        public int Count => materials.Count;

        /// <summary>
        /// Build the mesh material collection from a meshs material blocks (and optional variation)
        /// </summary>
        public MeshMaterialCollection(EbxAsset meshAsset, PointerRef inOptVariation)
        {
            MeshVariationDbEntry entry = MeshVariationDb.GetVariations(meshAsset.FileGuid);
            materials = new List<MeshMaterial>();

            MeshVariation mv = null;
            if (entry != null)
            {
                // load the base material variation from the database (if exists)
                mv = entry.GetVariation(MeshVariationDbEntry.ROOT_VARIATION);
            }

            // load base material properties first
            foreach (PointerRef matRef in ((dynamic)meshAsset.RootObject).Materials)
            {
                dynamic mat = ((dynamic)matRef.Internal).Shader;

                MeshMaterial material = new MeshMaterial();
                AssetClassGuid guid = ((dynamic)matRef.Internal).GetInstanceGuid();

                material.Guid = guid.ExportedGuid;

                PopulateMaterialFromShaderInstance(material, mat, out material.Shader);
                PopulateMaterialFromMeshVariation(material, mv);

                materials.Add(material);
            }

            // override with any variation properties
            if (inOptVariation.Type != PointerRefType.Null)
            {
                EbxAssetEntry ebx = App.AssetManager.GetEbxEntry(inOptVariation.External.FileGuid);
                EbxAsset varAsset = App.AssetManager.GetEbx(ebx);

                if (entry != null)
                {
                    // load the material variation from the database (if exists)
                    mv = entry.GetVariation((uint)Fnv1.HashString(ebx.Name.ToLower()));
                    if (mv != null && mv.Materials.Count > 0)
                    {
                        // use the mesh variation db entry variation (this may differ to the actual ObjectVariation defined)
                        if (mv.Materials[0].MaterialVariationAssetGuid != Guid.Empty && mv.Materials[0].MaterialVariationAssetGuid != ebx.Guid)
                            varAsset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(mv.Materials[0].MaterialVariationAssetGuid));
                    }
                }

                int index = 0;
                foreach (dynamic mat in varAsset.RootObjects)
                {
                    if (!TypeLibrary.IsSubClassOf((object)mat, "MeshMaterialVariation"))
                        continue;

                    MeshVariationMaterial material = null;
                    MeshMaterial baseMaterial = null;

                    if (mv != null)
                    {
                        AssetClassGuid guid = mat.GetInstanceGuid();
                        material = mv.Materials.Find((MeshVariationMaterial a) => a.MaterialVariationClassGuid == guid.ExportedGuid);
                    }

                    if (mv != null)
                    {
                        if (material != null)
                        {
                            // if we have a variation db, then match the base material guid
                            // with what is specified in the db

                            foreach (MeshMaterial curMaterial in materials)
                            {
                                if (curMaterial.Guid == material.MaterialGuid)
                                {
                                    baseMaterial = curMaterial;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // otherwise just go with an index match
                        if (index < materials.Count)
                            baseMaterial = materials[index];
                    }

                    if (baseMaterial == null)
                        continue;

                    // @temp
                    PointerRef shaderRef = mat.Shader.Shader;
                    if (shaderRef.Type != PointerRefType.Null)
                    {
                        baseMaterial.TextureParameters.Clear();
                        baseMaterial.VectorParameters.Clear();
                        baseMaterial.BoolParameters.Clear();
                        baseMaterial.ConditionalParameters.Clear();
                    }

                    PopulateMaterialFromShaderInstance(baseMaterial, mat.Shader, out baseMaterial.Shader);
                    PopulateMaterialFromMeshVariation(baseMaterial, mv);
                }
            }
        }

        /// <summary>
        /// Build the mesh material collection from a single material
        /// </summary>
        public MeshMaterialCollection(dynamic meshAsset, MeshMaterial material)
        {
            materials = new List<MeshMaterial>();
            foreach (PointerRef matRef in meshAsset.Materials)
                materials.Add(material);
        }

        /// <summary>
        /// Sets the specified texture parameter on all materials
        /// </summary>
        public void SetTextureParameter(string paramName, EbxAssetEntry texture)
        {
            foreach (MeshMaterial material in materials)
                material.SetTextureParameter(paramName, texture);
        }

        /// <summary>
        /// Sets the specified vector parameter on all materials using a Vec4
        /// </summary>
        public void SetVectorParameter(string paramName, dynamic /* Vec4 */ value)
        {
            foreach (MeshMaterial material in materials)
                material.SetVectorParameter(paramName, value);
        }

        /// <summary>
        /// Sets the specified texture parameter on all materials using a series of floats
        /// </summary>
        public void SetVectorParameter(string paramName, float x, float y, float z, float w)
        {
            foreach (MeshMaterial material in materials)
                material.SetVectorParameter(paramName, x, y, z, w);
        }

        /// <summary>
        /// Fill in texture/shader parameters from a shader instance structure
        /// </summary>
        private void PopulateMaterialFromShaderInstance(MeshMaterial material, dynamic /* SurfaceShaderInstanceDataStruct */ shader, out PointerRef shaderRef)
        {
            PointerRef tmpShaderRef = shader.Shader;
            if (tmpShaderRef.Type != PointerRefType.Null)
            {
                EbxAssetEntry shaderEntry = App.AssetManager.GetEbxEntry(tmpShaderRef.External.FileGuid);

                if (TypeLibrary.IsSubClassOf(shaderEntry.Type, "SurfaceShaderPreset"))
                {
                    EbxAsset shaderAsset = App.AssetManager.GetEbx(shaderEntry);
                    dynamic shaderPreset = ((dynamic)shaderAsset.RootObject).ShaderPreset;
                    PopulateMaterialFromShaderInstance(material, shaderPreset, out tmpShaderRef);
                }
            }
            else
                tmpShaderRef = material.Shader;
            shaderRef = tmpShaderRef;

            foreach (dynamic param in shader.BoolParameters)
            {
                int idx = material.BoolParameters.FindIndex((dynamic a) => a.ParameterName.Equals(param.ParameterName, StringComparison.OrdinalIgnoreCase));
                if (idx != -1) material.BoolParameters[idx] = param;
                else material.BoolParameters.Add(param);
            }
            foreach (dynamic param in shader.VectorParameters)
            {
                int idx = material.VectorParameters.FindIndex((dynamic a) => a.ParameterName.Equals(param.ParameterName, StringComparison.OrdinalIgnoreCase));
                if (idx != -1) material.VectorParameters[idx] = param;
                else material.VectorParameters.Add(param);
            }
            foreach (dynamic param in shader.TextureParameters)
            {
                int idx = material.TextureParameters.FindIndex((dynamic a) => a.ParameterName.Equals(param.ParameterName, StringComparison.OrdinalIgnoreCase));
                if (idx != -1) material.TextureParameters[idx] = param;
                else material.TextureParameters.Add(param);
            }
            if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Anthem, ProfileVersion.StarWarsSquadrons, ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound))
            {
                foreach (dynamic param in shader.ConditionalParameters)
                {
                    int idx = material.ConditionalParameters.FindIndex((dynamic a) => a.ConditionalAsset.Equals(param.ConditionalAsset));
                    if (idx != -1) material.ConditionalParameters[idx] = param;
                    else material.ConditionalParameters.Add(param);
                }
            }
        }

        /// <summary>
        /// Fill in texture parameters from a mesh variation entry
        /// </summary>
        private void PopulateMaterialFromMeshVariation(MeshMaterial material, MeshVariation mv)
        {
            if (mv != null)
            {
                int idx = mv.Materials.FindIndex((MeshVariationMaterial a) => a.MaterialGuid == material.Guid);
                if (idx != -1)
                {
                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield1 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Anthem && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield5 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa20
#if FROSTY_ALPHA || FROSTY_DEVELOPER
                        && ProfilesLibrary.DataVersion != (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville
#endif
                        )
                    {
                        dynamic texParams = mv.Materials[idx].TextureParameters;
                        if (texParams.Count > 0)
                        {
                            foreach (dynamic param in texParams)
                            {
                                idx = material.TextureParameters.FindIndex((dynamic a) => a.ParameterName.Equals(param.ParameterName, StringComparison.OrdinalIgnoreCase));
                                if (idx != -1) material.TextureParameters[idx] = param;
                                else material.TextureParameters.Add(param);
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator<MeshMaterial> GetEnumerator()
        {
            return materials.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return materials.GetEnumerator();
        }
    }
    #endregion

    public enum MeshRenderPath
    {
        Deferred,
        Forward,
        Shadows,
        Selection
    }

    public class MeshRenderBase
    {
        public virtual string DebugName { get; }
        public virtual void Render(DeviceContext context, MeshRenderPath renderPath)
        {
        }
    }
    public class MeshRenderShape : MeshRenderBase, IDisposable
    {
        private struct ShapeVertex
        {
            public Vector3 Pos;
            public Vector3 Normal;
            public Vector2 TexCoord;

            public ShapeVertex(Vector3 p, Vector3 n, Vector2 t)
            {
                Pos = p;
                Normal = n;
                TexCoord = t;
            }
        }

        public override string DebugName => name;

        private D3D11.Buffer vertexBuffer;
        private D3D11.Buffer indexBuffer;
        private D3D11.Buffer pixelParameters;
        private List<ShaderResourceView> pixelTextures = new List<ShaderResourceView>();
        private ShaderPermutation permutation;
        private int indexCount = 0;
        private string name;

        /// <summary>
        /// Constructs a renderable sphere
        /// </summary>
        public static MeshRenderShape CreateSphere(RenderCreateState state, string inName, string shaderName, float radius, int tessellation)
        {
            List<ShapeVertex> vertices = new List<ShapeVertex>();

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            for (int i = 0; i <= verticalSegments; i++)
            {
                float v = 1 - (float)i / verticalSegments;
                float latitude = (float)((i * Math.PI / verticalSegments) - (Math.PI / 2.0f));
                float dy = 0.0f, dxz = 0.0f;

                DirectXMathUtils.XMScalarSinCos(ref dy, ref dxz, latitude);

                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float u = (float)j / horizontalSegments;
                    float longitude = (float)(j * (Math.PI * 2) / horizontalSegments);
                    float dx = 0.0f, dz = 0.0f;

                    DirectXMathUtils.XMScalarSinCos(ref dx, ref dz, longitude);

                    dx *= dxz;
                    dz *= dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);
                    Vector3 pos = normal * radius;

                    vertices.Add(new ShapeVertex(pos, Vector3.TransformCoordinate(normal, Matrix.Scaling(-1, 1, -1)), Vector2.Zero));
                }
            }

            int stride = horizontalSegments + 1;
            List<ushort> indices = new List<ushort>();

            for (int i = 0; i < verticalSegments; i++)
            {
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % stride;

                    indices.Add((ushort)(i * stride + nextJ));
                    indices.Add((ushort)(i * stride + j));
                    indices.Add((ushort)(nextI * stride + j));


                    indices.Add((ushort)(nextI * stride + nextJ));
                    indices.Add((ushort)(i * stride + nextJ));
                    indices.Add((ushort)(nextI * stride + j));

                }
            }

            return new MeshRenderShape(state, inName, shaderName, vertices, indices);
        }

        /// <summary>
        /// Constructs a renderable box
        /// </summary>
        public static MeshRenderShape CreateCube(RenderCreateState state, string inName, string shaderName, int width, int height, int depth)
        {
            const int faceCount = 6;
            Vector3[] faceNormals = new Vector3[faceCount]
            {
                new Vector3(0,0,1),
                new Vector3(0,0,-1),
                new Vector3(1,0,0),
                new Vector3(-1,0,0),
                new Vector3(0,1,0),
                new Vector3(0,-1,0),
            };
            Vector2[] texCoords = new Vector2[4]
            {
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0)
            };

            Vector3 tsize = new Vector3(width, height, depth);
            tsize /= 2;

            List<ShapeVertex> vertices = new List<ShapeVertex>();
            List<ushort> indices = new List<ushort>();

            for (int i = 0; i < faceCount; i++)
            {
                Vector3 normal = faceNormals[i];
                Vector3 basis = (i >= 4) ? Vector3.UnitZ : Vector3.UnitY;
                Vector3 side1 = Vector3.Cross(normal, basis);
                Vector3 side2 = Vector3.Cross(normal, side1);

                int vbase = vertices.Count;
                indices.Add((ushort)(vbase + 2));
                indices.Add((ushort)(vbase + 1));
                indices.Add((ushort)(vbase + 0));

                indices.Add((ushort)(vbase + 3));
                indices.Add((ushort)(vbase + 2));
                indices.Add((ushort)(vbase + 0));

                vertices.Add(new ShapeVertex((normal - side1 - side2) * tsize, normal, texCoords[0]));
                vertices.Add(new ShapeVertex((normal - side1 + side2) * tsize, normal, texCoords[1]));
                vertices.Add(new ShapeVertex((normal + side1 + side2) * tsize, normal, texCoords[2]));
                vertices.Add(new ShapeVertex((normal + side1 - side2) * tsize, normal, texCoords[3]));
            }

            return new MeshRenderShape(state, inName, shaderName, vertices, indices);
        }

        /// <summary>
        /// Constructs a renderable plane along the X and Z axes
        /// </summary>
        public static MeshRenderShape CreatePlane(RenderCreateState state, string inName, string shaderName, int width, int depth)
        {
            List<ShapeVertex> vertices = new List<ShapeVertex>()
            {
                new ShapeVertex(new Vector3(-width, 0, depth), new Vector3(0, 1, 0), new Vector2(0, 0)),
                new ShapeVertex(new Vector3( width, 0, depth), new Vector3(0, 1, 0), new Vector2(1, 0)),
                new ShapeVertex(new Vector3( width, 0,-depth), new Vector3(0, 1, 0), new Vector2(1, 1)),
                new ShapeVertex(new Vector3(-width, 0,-depth), new Vector3(0, 1, 0), new Vector2(0, 1)),
            };
            List<ushort> indices = new List<ushort>() { 0, 1, 2, 0, 2, 3 };
            return new MeshRenderShape(state, inName, shaderName, vertices, indices);
        }

        /// <summary>
        /// Constructs a renderable shape from a list of vertices/indices
        /// </summary>
        private MeshRenderShape(RenderCreateState state, string inName, string shaderName, List<ShapeVertex> vertices, List<ushort> indices)
        {
            using (DataStream stream = new DataStream(indices.Count * 2, false, true))
            {
                stream.WriteRange<ushort>(indices.ToArray());
                stream.Position = 0;

                indexBuffer = new D3D11.Buffer(state.Device, stream, indices.Count * 2, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 2);
            }
            using (DataStream stream = new DataStream(vertices.Count * (4 * 8), false, true))
            {
                stream.WriteRange<ShapeVertex>(vertices.ToArray());
                stream.Position = 0;

                vertexBuffer = new D3D11.Buffer(state.Device, stream, vertices.Count * (4 * 8), ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, (4 * 8));
            }

            GeometryDeclarationDesc geomDecl = GeometryDeclarationDesc.Create(new GeometryDeclarationDesc.Element[]
            {
                new GeometryDeclarationDesc.Element() { Usage = VertexElementUsage.Pos, Format = VertexElementFormat.Float3 },
                new GeometryDeclarationDesc.Element() { Usage = VertexElementUsage.Normal, Format = VertexElementFormat.Float3 },
                new GeometryDeclarationDesc.Element() { Usage = VertexElementUsage.TexCoord0, Format = VertexElementFormat.Float2 },
            });

            permutation = state.ShaderLibrary.GetUserShader(shaderName, geomDecl);
            permutation.LoadShaders(state.Device);
            permutation.AssignParameters(state, ref pixelParameters, ref pixelTextures);

            indexCount = indices.Count;
            name = inName;
        }

        /// <summary>
        /// Renders this shape
        /// </summary>
        public override void Render(DeviceContext context, MeshRenderPath renderPath)
        {
            if (renderPath == MeshRenderPath.Shadows || renderPath == MeshRenderPath.Selection)
                return;

            context.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R16_UInt, 0);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 4 * 8, 0));

            permutation.SetState(context, renderPath);
            context.PixelShader.SetConstantBuffer(2, pixelParameters);
            context.PixelShader.SetShaderResources(1, pixelTextures.ToArray());

            context.DrawIndexed(indexCount, 0, 0);
        }

        public void Dispose()
        {
            pixelParameters?.Dispose();
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
            pixelTextures.Clear();
        }
    }
    #endregion
}
