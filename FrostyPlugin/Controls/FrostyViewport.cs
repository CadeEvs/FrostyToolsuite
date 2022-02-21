using Frosty.Core.Viewport;
using System;
using System.Windows.Controls;
using D3D11=SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.ComponentModel;
using System.Windows;
using SharpDX.Direct3D;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;

namespace Frosty.Core.Controls
{
    public class FrostyViewport : Image, IViewport
    {
        public const double FixedFrameTime = 1 / 120.0;

        public int ViewportWidth => viewportWidth;
        public int ViewportHeight => viewportHeight;
        public float LastFrameTime => lastFrameTime;
        public float TotalTime => totalTime;

        public SwapChain SwapChain => null;
        public D3D11.Device Device => device ?? (device = FrostyDeviceManager.Current.GetDevice());
        public D3D11.DeviceContext Context => Device.ImmediateContext;
        public D3D11.Texture2D ColorBuffer => backBuffer;
        public D3D11.RenderTargetView ColorBufferRTV => backBufferRTV;
        public D3D11.Texture2D DepthBuffer => depthBuffer;
        public D3D11.DepthStencilView DepthBufferDSV => depthBufferDSV;
        public D3D11.ShaderResourceView DepthBufferSRV => depthBufferSRV;

        public Screen Screen
        {
            get => screen;
            set
            {
                screen?.DisposeBuffers();
                screen?.Closed();
                screen?.SetViewport(null);
                screen = value;
                screen?.SetViewport(this);
                if (device != null)
                {
                    screen?.Opened();
                    screen?.CreateBuffers();
                }
            }
        }

        private D3D11.Device device;
        private D3D11.Texture2D backBuffer;
        private D3D11.Texture2D backBufferCopy;
        private D3D11.RenderTargetView backBufferRTV;
        private D3D11.Texture2D depthBuffer;
        private D3D11.DepthStencilView depthBufferDSV;
        private D3D11.ShaderResourceView depthBufferSRV;

        private Screen screen;
        private FrostyRenderImage imageSource;
        private Stopwatch stopwatch = new Stopwatch();
        private float lastFrameTime = 0.0f;
        private float totalTime = 0.0f;

        private int viewportHeight;
        private int viewportWidth;

        private bool bShutdownRenderThread = false;
        private bool bResize = false;
        private bool bDisposed = false;
        private bool bFirstLoad = true;
        private bool bPaused = false;

        private Thread renderThread;

        static FrostyViewport()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyViewport), new FrameworkPropertyMetadata(typeof(FrostyViewport)));
        }

        public FrostyViewport()
        {
            Focusable = true;
            Stretch = Stretch.Fill;
            SnapsToDevicePixels = true;

            Loaded += FrostyViewport_Loaded;
            Unloaded += FrostyViewport_Unloaded;
            SizeChanged += FrostyViewport_SizeChanged;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            MouseDown += FrostyViewport_MouseDown;
            MouseUp += FrostyViewport_MouseUp;
            MouseMove += FrostyViewport_MouseMove;
            MouseLeave += FrostyViewport_MouseLeave;
            MouseWheel += FrostyViewport_MouseWheel;

            KeyDown += FrostyViewport_KeyDown;
            KeyUp += FrostyViewport_KeyUp;
            TextInput += FrostyViewport_TextInput;            
        }

        public void SetPaused(bool newPause)
        {
            bPaused = newPause;
        }

        public void Shutdown()
        {
            if (bDisposed)
                return;

            StopRenderThread();

            DisposeBuffers();
            bDisposed = true;

            // dipose of UI
            imageSource.Dispose();
            Source = null;

            // remove current viewport from manager
            if (FrostyDeviceManager.Current.CurrentViewport == this)
                FrostyDeviceManager.Current.CurrentViewport = null;
        }

        private void ThreadCallback()
        {
            try
            {
                // create any viewport dependent buffers
                CreateSizeDependentBuffers();
                bResize = false;

                if (bFirstLoad)
                {
                    // load screen buffers
                    screen?.CreateBuffers();
                    bFirstLoad = false;
                }

                while (!bShutdownRenderThread)
                {
                    double frameTime = stopwatch.Elapsed.TotalSeconds;
                    lastFrameTime = (float)frameTime;
                    totalTime += lastFrameTime;
                    stopwatch.Restart();

                    if (!bPaused)
                    {
                        if (bResize)
                        {
                            DisposeSizeDependentBuffers();
                            CreateSizeDependentBuffers();
                            bResize = false;
                        }

                        Update(frameTime);
                        Render();
                    }

                    double elapsedTime = stopwatch.Elapsed.TotalSeconds;
                    if (elapsedTime < FixedFrameTime)
                        Thread.Sleep((int)((FixedFrameTime - elapsedTime) * 1000));
                }
            }
            catch (ThreadAbortException)
            {
                // dispose of any buffers that are dependent on viewport
                DisposeSizeDependentBuffers();
            }
        }

        private void Update(double frameTime)
        {
            screen?.Update(frameTime);
        }

        private void Render()
        {
            if (backBuffer == null || screen == null)
                return;

            screen?.Render();
            Device.ImmediateContext.Flush();
            Device.ImmediateContext.ResolveSubresource(backBuffer, 0, backBufferCopy, 0, Format.B8G8R8A8_UNorm);

            Dispatcher.Invoke(() =>
            {
                imageSource.Invalidate();
            });
        }

        public Point TranslateMousePointToScreen(Point point)
        {
            return PointToScreen(point);
        }

        private void FrostyViewport_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text))
                return;
            screen?.CharTyped(e.Text[0]);
        }

        private void FrostyViewport_KeyUp(object sender, KeyEventArgs e)
        {
            screen?.KeyUp((int)e.Key);
        }

        private void FrostyViewport_KeyDown(object sender, KeyEventArgs e)
        {
            screen?.KeyDown((int)e.Key);
        }

        private void FrostyViewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            screen?.MouseScroll(e.Delta);
        }

        private void FrostyViewport_MouseLeave(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
                screen?.MouseUp((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Left);
            else if (e.MiddleButton == MouseButtonState.Pressed)
                screen?.MouseUp((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Middle);
            else if (e.RightButton == MouseButtonState.Pressed)
                screen?.MouseUp((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Right);

            screen?.MouseLeave();
        }

        private void FrostyViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
            Point mousePos = e.GetPosition(this);
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                Screen?.MouseDown((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Left);
            else if (e.ChangedButton == System.Windows.Input.MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                Screen?.MouseDown((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Middle);
            else if (e.ChangedButton == System.Windows.Input.MouseButton.Right && e.ButtonState == MouseButtonState.Pressed)
                Screen?.MouseDown((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Right);
        }

        private void FrostyViewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.ButtonState == MouseButtonState.Released)
                Screen?.MouseUp((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Left);
            else if (e.ChangedButton == System.Windows.Input.MouseButton.Middle && e.ButtonState == MouseButtonState.Released)
                Screen?.MouseUp((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Middle);
            else if (e.ChangedButton == System.Windows.Input.MouseButton.Right && e.ButtonState == MouseButtonState.Released)
                Screen?.MouseUp((int)mousePos.X, (int)mousePos.Y, Viewport.MouseButton.Right);
        }

        private void FrostyViewport_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            screen?.MouseMove((int)mousePos.X, (int)mousePos.Y);
        }

        public bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(this);

        private void FrostyViewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeBuffers();
        }

        private void FrostyViewport_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            // notify screen we are returning
            screen?.Opened();

            // start the render thread
            StartRenderThread();

            // assign viewport to manager
            FrostyDeviceManager.Current.CurrentViewport = this;
        }

        private void FrostyViewport_Unloaded(object sender, RoutedEventArgs e)
        {
            if (bDisposed)
                return;

            // notify screen that we are closing (not necessarily shutting down)
            screen?.Closed();

            // stop the render thread
            StopRenderThread();
            
            // dipose of UI
            imageSource.Dispose();
            Source = null;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void StopRenderThread()
        {
            renderThread.Abort();
            while (renderThread.IsAlive)
                Thread.Sleep(1);
        }

        private void StartRenderThread()
        {
            bShutdownRenderThread = false;
            renderThread = new Thread(new ThreadStart(ThreadCallback));
            renderThread.Start();
        }

        private void ResizeBuffers()
        {
            bResize = true;
        }

        private void CreateSizeDependentBuffers()
        {
            if (backBuffer != null)
                return;

            viewportWidth = (ActualWidth < 1.0) ? 1 : (int)ActualWidth;
            viewportHeight = (ActualHeight < 1.0) ? 1 : (int)ActualHeight;

            backBuffer = new D3D11.Texture2D(Device, new D3D11.Texture2DDescription
            {
                BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = viewportWidth,
                Height = viewportHeight,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                ArraySize = 1
            }) {DebugName = "BackBuffer"};

            backBufferCopy = new D3D11.Texture2D(Device, new D3D11.Texture2DDescription()
            {
                BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = viewportWidth,
                Height = viewportHeight,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                OptionFlags = D3D11.ResourceOptionFlags.Shared,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                ArraySize = 1
            });

            backBufferRTV = new D3D11.RenderTargetView(Device, backBuffer) {DebugName = "BackBuffer (RTV)"};

            depthBuffer = new D3D11.Texture2D(Device, new D3D11.Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = D3D11.BindFlags.DepthStencil | D3D11.BindFlags.ShaderResource,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                Format = Format.R24G8_Typeless,
                Width = viewportWidth,
                Height = viewportHeight,
                MipLevels = 1,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default
            }) {DebugName = "DepthBuffer"};

            depthBufferDSV = new D3D11.DepthStencilView(Device, depthBuffer, new D3D11.DepthStencilViewDescription()
            {
                Dimension = D3D11.DepthStencilViewDimension.Texture2D,
                Flags = D3D11.DepthStencilViewFlags.None,
                Format = Format.D24_UNorm_S8_UInt,
                Texture2D = new D3D11.DepthStencilViewDescription.Texture2DResource {MipSlice = 0}
            }) {DebugName = "DepthBuffer (DSV)"};

            depthBufferSRV = new D3D11.ShaderResourceView(Device, depthBuffer, new D3D11.ShaderResourceViewDescription()
            {
                Dimension = ShaderResourceViewDimension.Texture2D,
                Format = Format.R24_UNorm_X8_Typeless,
                Texture2D = new D3D11.ShaderResourceViewDescription.Texture2DResource()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            }) {DebugName = "DepthBuffer (SRV)"};

            Device.ImmediateContext.OutputMerger.SetRenderTargets(depthBufferDSV, backBufferRTV);
            Device.ImmediateContext.Rasterizer.SetViewport(new SharpDX.Viewport(0, 0, viewportWidth, viewportHeight));

            screen?.CreateSizeDependentBuffers();

            Dispatcher.Invoke(() =>
            {
                imageSource = new FrostyRenderImage();
                imageSource.SetBackBuffer(backBuffer);
                //imageSource.SetBackBuffer(backBufferCopy);
                Source = imageSource.ImageSource;
            });
        }

        private void DisposeSizeDependentBuffers()
        {
            if (backBuffer == null)
                return;

            screen?.DisposeSizeDependentBuffers();

            depthBufferDSV.Dispose();
            depthBuffer.Dispose();
            backBufferRTV.Dispose();
            backBuffer.Dispose();
            backBufferCopy.Dispose();
            backBuffer = null;
        }
        private void DisposeBuffers()
        {
            screen?.DisposeBuffers();
            DisposeSizeDependentBuffers();
        }
    }
}
