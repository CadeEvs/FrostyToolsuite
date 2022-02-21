using System;
using FrostySdk.Interfaces;
using System.Windows;
using System.Runtime.InteropServices;
using Frosty.Core.Viewport;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using Frosty.Core.Controls;
using Frosty.Core;

namespace IesResourcePlugin
{
    #region -- Screen --
    class IesResourceScreen : Screen
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex
        {
            public SharpDX.Vector3 Position;
            public SharpDX.Vector2 TexCoord;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Constants
        {
            public SharpDX.Vector2 ViewportDim;
            public SharpDX.Vector2 TextureDim;
            public SharpDX.Matrix ChannelMask;
            public float SrgbEnabled;
            public float MipLevel;
            public float SliceLevel;
            public float Padding;
        }

        private D3D11.Texture2D texture;
        private D3D11.ShaderResourceView textureSRV;
        private D3D11.SamplerState samplerState;
        private D3D11.RasterizerState rasterizerState;
        private D3D11.DepthStencilState depthStencilState;
        private D3D11.BlendState blendState;
        private D3D11.InputLayout inputLayout;
        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;
        private D3D11.Buffer vertexBuffer;
        private D3D11.Buffer constantBuffer;

        private IesResource resource;

        public IesResourceScreen(IesResource inResource)
        {
            resource = inResource;
        }

        public override void CreateBuffers()
        {
            CreateGlobalResources();
            CreateTextureResources();
            CreateMeshResources();
        }

        public override void Update(double timestep)
        {
        }

        public override void Render()
        {
            SharpDX.ViewportF[] viewports = Viewport.Context.Rasterizer.GetViewports<SharpDX.ViewportF>();

            Constants constants = new Constants
            {
                TextureDim =
                {
                    X = resource.Size,
                    Y = resource.Size
                },
                ViewportDim =
                {
                    X = viewports[0].Width,
                    Y = viewports[0].Height
                },
                ChannelMask = SharpDX.Matrix.Identity,
                SrgbEnabled = 0.0f,
                MipLevel = 0,
                SliceLevel = 0
            };

            Viewport.Context.ClearRenderTargetView(Viewport.ColorBufferRTV, new SharpDX.Color4(0, 0, 0, 0));
            Viewport.Context.UpdateSubresource(ref constants, constantBuffer, 0);

            Viewport.Context.OutputMerger.BlendState = blendState;
            Viewport.Context.OutputMerger.DepthStencilState = depthStencilState;
            Viewport.Context.Rasterizer.State = rasterizerState;

            Viewport.Context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            Viewport.Context.InputAssembler.InputLayout = inputLayout;
            Viewport.Context.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, SharpDX.Utilities.SizeOf<Vertex>(), 0));

            Viewport.Context.VertexShader.SetConstantBuffer(0, constantBuffer);
            Viewport.Context.PixelShader.SetConstantBuffer(0, constantBuffer);

            {
                Viewport.Context.VertexShader.Set(vertexShader);
                Viewport.Context.PixelShader.Set(pixelShader);
                Viewport.Context.PixelShader.SetShaderResource(0, textureSRV);
                Viewport.Context.PixelShader.SetSampler(0, samplerState);

                Viewport.Context.Draw(6, 0);
            }
        }

        public override void DisposeBuffers()
        {
            vertexShader.Dispose();
            pixelShader.Dispose();
            inputLayout.Dispose();

            textureSRV.Dispose();
            texture.Dispose();
            vertexBuffer.Dispose();
            constantBuffer.Dispose();

            samplerState.Dispose();
            blendState.Dispose();
            rasterizerState.Dispose();
            depthStencilState.Dispose();
        }

        private void CreateGlobalResources()
        {
            rasterizerState = new D3D11.RasterizerState(Viewport.Device, new D3D11.RasterizerStateDescription()
            {
                CullMode = D3D11.CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = D3D11.FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            });

            D3D11.BlendStateDescription desc = new D3D11.BlendStateDescription();
            desc.RenderTarget[0].IsBlendEnabled = true;
            desc.RenderTarget[0].SourceBlend = D3D11.BlendOption.SourceAlpha;
            desc.RenderTarget[0].DestinationBlend = D3D11.BlendOption.InverseSourceAlpha;
            desc.RenderTarget[0].BlendOperation = D3D11.BlendOperation.Add;
            desc.RenderTarget[0].SourceAlphaBlend = D3D11.BlendOption.One;
            desc.RenderTarget[0].DestinationAlphaBlend = D3D11.BlendOption.One;
            desc.RenderTarget[0].AlphaBlendOperation = D3D11.BlendOperation.Add;
            desc.RenderTarget[0].RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All;
            blendState = new D3D11.BlendState(Viewport.Device, desc);

            samplerState = new D3D11.SamplerState(Viewport.Device, new D3D11.SamplerStateDescription()
            {
                AddressU = D3D11.TextureAddressMode.Wrap,
                AddressV = D3D11.TextureAddressMode.Wrap,
                AddressW = D3D11.TextureAddressMode.Wrap,
                BorderColor = new SharpDX.Color(0, 0, 0),
                ComparisonFunction = D3D11.Comparison.Always,
                Filter = D3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16,
                MaximumLod = 20,
                MinimumLod = 0,
                MipLodBias = 0
            });
            depthStencilState = new D3D11.DepthStencilState(Viewport.Device, new D3D11.DepthStencilStateDescription()
            {
                IsDepthEnabled = false,
                IsStencilEnabled = false
            });
        }

        private void CreateTextureResources()
        {
            texture = LoadTexture(Viewport.Device, resource);

            // all texture types are represented by a 2D array (even a single T2D, just has one slice)
            textureSRV = new D3D11.ShaderResourceView(Viewport.Device, texture, new D3D11.ShaderResourceViewDescription()
            {
                Format = texture.Description.Format,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DArray,
                Texture2DArray = new D3D11.ShaderResourceViewDescription.Texture2DArrayResource()
                {
                    ArraySize = texture.Description.ArraySize,
                    FirstArraySlice = 0,
                    MipLevels = -1,
                    MostDetailedMip = 0
                }
            });
        }

        private D3D11.Texture2D LoadTexture(D3D11.Device device, IesResource resource)
        {
            resource.Data.Position = 0;

            byte[] buffer = new byte[resource.Data.Length];
            resource.Data.Read(buffer, 0, (int)resource.Data.Length);

            D3D11.Texture2DDescription desc = new D3D11.Texture2DDescription()
            {
                BindFlags = D3D11.BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.R16_Float,
                Width = resource.Size,
                Height = resource.Size,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                ArraySize = 1
            };

            D3D11.Texture2D texture = new D3D11.Texture2D(device, desc);
            int tmp = 0;

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            {
                IntPtr bufferPtr = handle.AddrOfPinnedObject();
                SharpDX.DataBox box = new SharpDX.DataBox(bufferPtr, resource.Size * SharpDX.DXGI.FormatHelper.SizeOfInBytes(desc.Format), 0);

                device.ImmediateContext.UpdateSubresource(box, texture, texture.CalculateSubResourceIndex(0, 0, out tmp));
            }
            handle.Free();

            return texture;
        }

        private void CreateMeshResources()
        {
            Vertex[] vertices = new Vertex[]
            {
                new Vertex() { Position = new SharpDX.Vector3(-1.0f, -1.0f, 0.0f), TexCoord = new SharpDX.Vector2(0, 1) },
                new Vertex() { Position = new SharpDX.Vector3( 1.0f, -1.0f, 0.0f), TexCoord = new SharpDX.Vector2(1, 1) },
                new Vertex() { Position = new SharpDX.Vector3(-1.0f,  1.0f, 0.0f), TexCoord = new SharpDX.Vector2(0, 0) },

                new Vertex() { Position = new SharpDX.Vector3( 1.0f,  1.0f, 0.0f), TexCoord = new SharpDX.Vector2(1, 0) },
                new Vertex() { Position = new SharpDX.Vector3(-1.0f,  1.0f, 0.0f), TexCoord = new SharpDX.Vector2(0, 0) },
                new Vertex() { Position = new SharpDX.Vector3( 1.0f, -1.0f, 0.0f), TexCoord = new SharpDX.Vector2(1, 1) }
            };
            vertexBuffer = D3D11.Buffer.Create(Viewport.Device, D3D11.BindFlags.VertexBuffer, vertices);

            vertexShader = FrostyShaderDb.GetShaderWithSignature<D3D11.VertexShader>(Viewport.Device, "Texture", out ShaderSignature signature);
            pixelShader = FrostyShaderDb.GetShader<D3D11.PixelShader>(Viewport.Device, "IesResource");

            D3D11.InputElement[] elements = new D3D11.InputElement[]
            {
                new D3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
                new D3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 12, 0, D3D11.InputClassification.PerVertexData, 0)
            };
            inputLayout = new D3D11.InputLayout(Viewport.Device, signature, elements);
            signature.Dispose();

            constantBuffer = new D3D11.Buffer(Viewport.Device, SharpDX.Utilities.SizeOf<Constants>(), D3D11.ResourceUsage.Default,
                D3D11.BindFlags.ConstantBuffer, D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
        }
    }
    #endregion

    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyViewport))]
    public class FrostyIesResourceEditor : FrostyAssetEditor
    {
        private const string PART_Renderer = "PART_Renderer";

        private FrostyViewport renderer;
        private IesResource resource;
        private IesResourceScreen screen;
        private bool firstTimeLoad = true;

        public FrostyIesResourceEditor(ILogger inLogger) 
            : base(inLogger)
        {
        }

        static FrostyIesResourceEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyIesResourceEditor), new FrameworkPropertyMetadata(typeof(FrostyIesResourceEditor)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            renderer = GetTemplateChild(PART_Renderer) as FrostyViewport;
            Loaded += FrostyAtlasTextureEditor_Loaded;
        }

        private void FrostyAtlasTextureEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                ulong resRid = ((dynamic)RootObject).SourceResource;

                resource = App.AssetManager.GetResAs<IesResource>(App.AssetManager.GetResEntry(resRid));
                screen = new IesResourceScreen(resource);

                firstTimeLoad = false;
            }

            renderer.Screen = screen;
            renderer.Width = 1024;
            renderer.Height = 1024;
        }
    }
}
