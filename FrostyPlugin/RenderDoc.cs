using System;
using System.Runtime.InteropServices;

namespace Frosty.Core
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryEx", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);

        [DllImport("kernel32", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    internal class LoadLibraryHandle
    {
        IntPtr handle;
        public LoadLibraryHandle(string lib)
        {
            handle = Kernel32.LoadLibraryEx(lib, IntPtr.Zero, 0);
        }
        public static implicit operator IntPtr(LoadLibraryHandle value) { return value.handle; }
        ~LoadLibraryHandle()
        {
            Kernel32.FreeLibrary(handle);
        }
    }

    internal enum RENDERDOC_CaptureOption
    {
        // Allow the application to enable vsync
        //
        // Default - enabled
        //
        // 1 - The application can enable or disable vsync at will
        // 0 - vsync is force disabled
        eRENDERDOC_Option_AllowVSync = 0,

        // Allow the application to enable fullscreen
        //
        // Default - enabled
        //
        // 1 - The application can enable or disable fullscreen at will
        // 0 - fullscreen is force disabled
        eRENDERDOC_Option_AllowFullscreen = 1,

        // Record API debugging events and messages
        //
        // Default - disabled
        //
        // 1 - Enable built-in API debugging features and records the results into
        //     the capture logfile, which is matched up with events on replay
        // 0 - no API debugging is forcibly enabled
        eRENDERDOC_Option_APIValidation = 2,
        eRENDERDOC_Option_DebugDeviceMode = 2,    // deprecated name of this enum

        // Capture CPU callstacks for API events
        //
        // Default - disabled
        //
        // 1 - Enables capturing of callstacks
        // 0 - no callstacks are captured
        eRENDERDOC_Option_CaptureCallstacks = 3,

        // When capturing CPU callstacks, only capture them from drawcalls.
        // This option does nothing without the above option being enabled
        //
        // Default - disabled
        //
        // 1 - Only captures callstacks for drawcall type API events.
        //     Ignored if CaptureCallstacks is disabled
        // 0 - Callstacks, if enabled, are captured for every event.
        eRENDERDOC_Option_CaptureCallstacksOnlyDraws = 4,

        // Specify a delay in seconds to wait for a debugger to attach, after
        // creating or injecting into a process, before continuing to allow it to run.
        //
        // 0 indicates no delay, and the process will run immediately after injection
        //
        // Default - 0 seconds
        //
        eRENDERDOC_Option_DelayForDebugger = 5,

        // Verify any writes to mapped buffers, by checking the memory after the
        // bounds of the returned pointer to detect any modification.
        //
        // Default - disabled
        //
        // 1 - Verify any writes to mapped buffers
        // 0 - No verification is performed, and overwriting bounds may cause
        //     crashes or corruption in RenderDoc
        eRENDERDOC_Option_VerifyMapWrites = 6,

        // Hooks any system API calls that create child processes, and injects
        // RenderDoc into them recursively with the same options.
        //
        // Default - disabled
        //
        // 1 - Hooks into spawned child processes
        // 0 - Child processes are not hooked by RenderDoc
        eRENDERDOC_Option_HookIntoChildren = 7,

        // By default RenderDoc only includes resources in the final logfile necessary
        // for that frame, this allows you to override that behaviour.
        //
        // Default - disabled
        //
        // 1 - all live resources at the time of capture are included in the log
        //     and available for inspection
        // 0 - only the resources referenced by the captured frame are included
        eRENDERDOC_Option_RefAllResources = 8,

        // By default RenderDoc skips saving initial states for resources where the
        // previous contents don't appear to be used, assuming that writes before
        // reads indicate previous contents aren't used.
        //
        // Default - disabled
        //
        // 1 - initial contents at the start of each captured frame are saved, even if
        //     they are later overwritten or cleared before being used.
        // 0 - unless a read is detected, initial contents will not be saved and will
        //     appear as black or empty data.
        eRENDERDOC_Option_SaveAllInitials = 9,

        // In APIs that allow for the recording of command lists to be replayed later,
        // RenderDoc may choose to not capture command lists before a frame capture is
        // triggered, to reduce overheads. This means any command lists recorded once
        // and replayed many times will not be available and may cause a failure to
        // capture.
        //
        // Note this is only true for APIs where multithreading is difficult or
        // discouraged. Newer APIs like Vulkan and D3D12 will ignore this option
        // and always capture all command lists since the API is heavily oriented
        // around it and the overheads have been reduced by API design.
        //
        // 1 - All command lists are captured from the start of the application
        // 0 - Command lists are only captured if their recording begins during
        //     the period when a frame capture is in progress.
        eRENDERDOC_Option_CaptureAllCmdLists = 10,

        // Mute API debugging output when the API validation mode option is enabled
        //
        // Default - enabled
        //
        // 1 - Mute any API debug messages from being displayed or passed through
        // 0 - API debugging is displayed as normal
        eRENDERDOC_Option_DebugOutputMute = 11,

    }

    internal enum RENDERDOC_InputButton
    {
        // '0' - '9' matches ASCII values
        eRENDERDOC_Key_0 = 0x30,
        eRENDERDOC_Key_1 = 0x31,
        eRENDERDOC_Key_2 = 0x32,
        eRENDERDOC_Key_3 = 0x33,
        eRENDERDOC_Key_4 = 0x34,
        eRENDERDOC_Key_5 = 0x35,
        eRENDERDOC_Key_6 = 0x36,
        eRENDERDOC_Key_7 = 0x37,
        eRENDERDOC_Key_8 = 0x38,
        eRENDERDOC_Key_9 = 0x39,

        // 'A' - 'Z' matches ASCII values
        eRENDERDOC_Key_A = 0x41,
        eRENDERDOC_Key_B = 0x42,
        eRENDERDOC_Key_C = 0x43,
        eRENDERDOC_Key_D = 0x44,
        eRENDERDOC_Key_E = 0x45,
        eRENDERDOC_Key_F = 0x46,
        eRENDERDOC_Key_G = 0x47,
        eRENDERDOC_Key_H = 0x48,
        eRENDERDOC_Key_I = 0x49,
        eRENDERDOC_Key_J = 0x4A,
        eRENDERDOC_Key_K = 0x4B,
        eRENDERDOC_Key_L = 0x4C,
        eRENDERDOC_Key_M = 0x4D,
        eRENDERDOC_Key_N = 0x4E,
        eRENDERDOC_Key_O = 0x4F,
        eRENDERDOC_Key_P = 0x50,
        eRENDERDOC_Key_Q = 0x51,
        eRENDERDOC_Key_R = 0x52,
        eRENDERDOC_Key_S = 0x53,
        eRENDERDOC_Key_T = 0x54,
        eRENDERDOC_Key_U = 0x55,
        eRENDERDOC_Key_V = 0x56,
        eRENDERDOC_Key_W = 0x57,
        eRENDERDOC_Key_X = 0x58,
        eRENDERDOC_Key_Y = 0x59,
        eRENDERDOC_Key_Z = 0x5A,

        // leave the rest of the ASCII range free
        // in case we want to use it later
        eRENDERDOC_Key_NonPrintable = 0x100,

        eRENDERDOC_Key_Divide,
        eRENDERDOC_Key_Multiply,
        eRENDERDOC_Key_Subtract,
        eRENDERDOC_Key_Plus,

        eRENDERDOC_Key_F1,
        eRENDERDOC_Key_F2,
        eRENDERDOC_Key_F3,
        eRENDERDOC_Key_F4,
        eRENDERDOC_Key_F5,
        eRENDERDOC_Key_F6,
        eRENDERDOC_Key_F7,
        eRENDERDOC_Key_F8,
        eRENDERDOC_Key_F9,
        eRENDERDOC_Key_F10,
        eRENDERDOC_Key_F11,
        eRENDERDOC_Key_F12,

        eRENDERDOC_Key_Home,
        eRENDERDOC_Key_End,
        eRENDERDOC_Key_Insert,
        eRENDERDOC_Key_Delete,
        eRENDERDOC_Key_PageUp,
        eRENDERDOC_Key_PageDn,

        eRENDERDOC_Key_Backspace,
        eRENDERDOC_Key_Tab,
        eRENDERDOC_Key_PrtScrn,
        eRENDERDOC_Key_Pause,

        eRENDERDOC_Key_Max,
    }

    internal enum RENDERDOC_OverlayBits : uint
    {
        // This single bit controls whether the overlay is enabled or disabled globally
        eRENDERDOC_Overlay_Enabled = 0x1,

        // Show the average framerate over several seconds as well as min/max
        eRENDERDOC_Overlay_FrameRate = 0x2,

        // Show the current frame number
        eRENDERDOC_Overlay_FrameNumber = 0x4,

        // Show a list of recent captures, and how many captures have been made
        eRENDERDOC_Overlay_CaptureList = 0x8,

        // Default values for the overlay mask
        eRENDERDOC_Overlay_Default = (eRENDERDOC_Overlay_Enabled | eRENDERDOC_Overlay_FrameRate |
                                      eRENDERDOC_Overlay_FrameNumber | eRENDERDOC_Overlay_CaptureList),

        // Enable all bits
        eRENDERDOC_Overlay_All = 0xFFFFFFFF,

        // Disable all bits
        eRENDERDOC_Overlay_None = 0,
    }

    internal static class RenderDoc
    {
        private static LoadLibraryHandle handle;
        private delegate int GetApiFuncPtr(int version, out IntPtr ptrs);

        public static Api GetAPI(int version)
        {
            handle = new LoadLibraryHandle("thirdparty/renderdoc.dll");
            if (handle == IntPtr.Zero)
                return null;

            GetApiFuncPtr func = Marshal.GetDelegateForFunctionPointer<GetApiFuncPtr>(Kernel32.GetProcAddress(handle, "RENDERDOC_GetAPI"));

            func(version, out IntPtr ptrs);

            return new Api(ptrs);
        }

        public class Api
        {
            private IntPtr ptr;
            public Api(IntPtr inPtr)
            {
                ptr = inPtr;
            }

            /*
              pRENDERDOC_GetAPIVersion GetAPIVersion; = 0x00

              pRENDERDOC_SetCaptureOptionU32 SetCaptureOptionU32; = 0x08
              pRENDERDOC_SetCaptureOptionF32 SetCaptureOptionF32; = 0x10

              pRENDERDOC_GetCaptureOptionU32 GetCaptureOptionU32; = 0x18
              pRENDERDOC_GetCaptureOptionF32 GetCaptureOptionF32; = 0x20

              pRENDERDOC_SetFocusToggleKeys SetFocusToggleKeys; = 0x28
              pRENDERDOC_SetCaptureKeys SetCaptureKeys; = 0x30

              pRENDERDOC_GetOverlayBits GetOverlayBits; = 0x38
              pRENDERDOC_MaskOverlayBits MaskOverlayBits; = 0x40

              pRENDERDOC_Shutdown Shutdown; = 0x48
              pRENDERDOC_UnloadCrashHandler UnloadCrashHandler; = 0x50

              pRENDERDOC_SetLogFilePathTemplate SetLogFilePathTemplate; = 0x58 
              pRENDERDOC_GetLogFilePathTemplate GetLogFilePathTemplate; = 0x60

              pRENDERDOC_GetNumCaptures GetNumCaptures; = 0x68
              pRENDERDOC_GetCapture GetCapture; = 0x70

              pRENDERDOC_TriggerCapture TriggerCapture; = 0x78

              pRENDERDOC_IsTargetControlConnected IsTargetControlConnected; = 0x80
              pRENDERDOC_LaunchReplayUI LaunchReplayUI; = 0x88

              pRENDERDOC_SetActiveWindow SetActiveWindow; = 0x90

              pRENDERDOC_StartFrameCapture StartFrameCapture; = 0x98
              pRENDERDOC_IsFrameCapturing IsFrameCapturing; = 0xA0
              pRENDERDOC_EndFrameCapture EndFrameCapture; = 0xA8
            */

            private delegate void GetAPIVersionFuncPtr(out int major, out int minor, out int rev);
            public void GetAPIVersion(out int major, out int minor, out int rev)
            {
                GetAPIVersionFuncPtr func = Marshal.GetDelegateForFunctionPointer<GetAPIVersionFuncPtr>(Marshal.ReadIntPtr(ptr, 0));
                func(out major, out minor, out rev);
            }

            private delegate void SetCaptureOptionU32FuncPtr(RENDERDOC_CaptureOption option, uint value);
            public void SetCaptureOptionU32(RENDERDOC_CaptureOption option, uint value)
            {
                SetCaptureOptionU32FuncPtr func = Marshal.GetDelegateForFunctionPointer<SetCaptureOptionU32FuncPtr>(Marshal.ReadIntPtr(ptr, 0x08));
                func(option, value);
            }

            private delegate uint GetOverlayBitsFuncPtr();
            public RENDERDOC_OverlayBits GetOverlayBits()
            {
                GetOverlayBitsFuncPtr func = Marshal.GetDelegateForFunctionPointer<GetOverlayBitsFuncPtr>(Marshal.ReadIntPtr(ptr, 0x38));
                return (RENDERDOC_OverlayBits)func();
            }

            private delegate void SetLogFilePathTemplateFuncPtr([MarshalAs(UnmanagedType.LPStr)] string path);
            public void SetLogFilePathTemplate(string path)
            {
                SetLogFilePathTemplateFuncPtr func = Marshal.GetDelegateForFunctionPointer<SetLogFilePathTemplateFuncPtr>(Marshal.ReadIntPtr(ptr, 0x58));
                func(path);
            }

            [return: MarshalAs(UnmanagedType.LPStr)]
            private delegate string GetLogFilePathTemplateFuncPtr();
            public string GetLogFilePathTemplate()
            {
                GetLogFilePathTemplateFuncPtr func = Marshal.GetDelegateForFunctionPointer<GetLogFilePathTemplateFuncPtr>(Marshal.ReadIntPtr(ptr, 0x60));
                return func();
            }

            private delegate void LaunchReplayUIFuncPtr(int connectTargetControl, [MarshalAs(UnmanagedType.LPStr)] string cmdLine);
            public void LaunchReplayUI(bool connectTC, string cmdLine)
            {
                LaunchReplayUIFuncPtr func = Marshal.GetDelegateForFunctionPointer<LaunchReplayUIFuncPtr>(Marshal.ReadIntPtr(ptr, 0x88));
                func((connectTC) ? 1 : 0, cmdLine);
            }

            private delegate void SetActiveWindowFuncPtr(IntPtr device, IntPtr wndHandle);
            public void SetActiveWindow(SharpDX.Direct3D11.Device device, IntPtr hWnd)
            {
                SetActiveWindowFuncPtr func = Marshal.GetDelegateForFunctionPointer<SetActiveWindowFuncPtr>(Marshal.ReadIntPtr(ptr, 0x90));
                func((device != null) ? device.NativePointer : IntPtr.Zero, hWnd);
            }

            private delegate void StartFrameCaptureFuncPtr(IntPtr device, IntPtr handle);
            public void StartFrameCapture(SharpDX.Direct3D11.Device device, IntPtr handle)
            {
                StartFrameCaptureFuncPtr func = Marshal.GetDelegateForFunctionPointer<StartFrameCaptureFuncPtr>(Marshal.ReadIntPtr(ptr, 0x98));
                func((device != null) ? device.NativePointer : IntPtr.Zero, handle);
            }

            private delegate void EndFrameCaptureFuncPtr(IntPtr device, IntPtr handle);
            public void EndFrameCapture(SharpDX.Direct3D11.Device device, IntPtr handle)
            {
                EndFrameCaptureFuncPtr func = Marshal.GetDelegateForFunctionPointer<EndFrameCaptureFuncPtr>(Marshal.ReadIntPtr(ptr, 0xa8));
                func((device != null) ? device.NativePointer : IntPtr.Zero, handle);
            }

            private delegate void TriggerCaptureFuncPtr();
            public void TriggerCapture()
            {
                TriggerCaptureFuncPtr func = Marshal.GetDelegateForFunctionPointer<TriggerCaptureFuncPtr>(Marshal.ReadIntPtr(ptr, 0x78));
                func();
            }
        }
    }
}
