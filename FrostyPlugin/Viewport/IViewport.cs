using SharpDX.Direct3D11;

namespace Frosty.Core.Viewport
{
    public interface IViewport
    {
        SharpDX.DXGI.SwapChain SwapChain { get; }
        Device Device { get; }
        DeviceContext Context { get; }
        Texture2D ColorBuffer { get; }
        RenderTargetView ColorBufferRTV { get; }
        Texture2D DepthBuffer { get; }
        DepthStencilView DepthBufferDSV { get; }
        ShaderResourceView DepthBufferSRV { get; }

        int ViewportWidth { get; }
        int ViewportHeight { get; }
        float LastFrameTime { get; }
        float TotalTime { get; }

        Screen Screen { get; set; }

        System.Windows.Point TranslateMousePointToScreen(System.Windows.Point point);
    }
}
