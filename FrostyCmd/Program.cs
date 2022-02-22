using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Reflection;

namespace FrostyCmd
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string text, params object[] vars)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]: " + text, vars);
        }

        public void LogError(string text, params object[] vars)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string text, params object[] vars)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        public delegate void FunctionDelegate(FileSystem fs, AssetManager am, string[] args);
        public static readonly ConsoleLogger logger = new ConsoleLogger();

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
        internal static class Oodle
        {
            public delegate int DecompressFunc(IntPtr srcBuffer, long srcSize, IntPtr dstBuffer, long dstSize, int a5 = 0, int a6 = 0, long a7 = 0, long a8 = 0, long a9 = 0, long a10 = 0, long a11 = 0, long a12 = 0, long a13 = 0, int a14 = 3);
            public static DecompressFunc Decompress;

            public delegate long CompressFunc(int cmpCode, IntPtr srcBuffer, long srcSize, IntPtr cmpBuffer, long cmpSize, long dict = 0, long dictSize = 0);
            public static CompressFunc Compress;

            public delegate long MemorySizeNeededFunc(int a1, long a2);
            public static MemorySizeNeededFunc MemorySizeNeeded;

            internal static LoadLibraryHandle handle;
            internal static void Bind(string basePath)
            {
                string dllPath = basePath + "oo2core_6_win64.dll";

                handle = new LoadLibraryHandle(dllPath);
                if (handle == IntPtr.Zero)
                    return;

                Decompress = Marshal.GetDelegateForFunctionPointer<DecompressFunc>(Kernel32.GetProcAddress(handle, "OodleLZ_Decompress"));
                Compress = Marshal.GetDelegateForFunctionPointer<CompressFunc>(Kernel32.GetProcAddress(handle, "OodleLZ_Compress"));
                MemorySizeNeeded = Marshal.GetDelegateForFunctionPointer<MemorySizeNeededFunc>(Kernel32.GetProcAddress(handle, "OodleLZDecoder_MemorySizeNeeded"));
            }
        }
        internal static class LZ4
        {
            [DllImport("thirdparty/liblz4.so.1.8.0.dll", EntryPoint = "LZ4_decompress_fast")]
            public static extern int Decompress(IntPtr src, IntPtr dst, int outputSize);

            [DllImport("thirdparty/liblz4.so.1.8.0.dll", EntryPoint = "LZ4_compressBound")]
            public static extern int CompressBound(int inputSize);

            [DllImport("thirdparty/liblz4.so.1.8.0.dll", EntryPoint = "LZ4_compress_default")]
            public static extern int Compress(IntPtr src, IntPtr dst, int sourceSize, int maxDestSize);
        }

        internal class BitReader : IDisposable
        {
            public bool EndOfStream => atEnd;

            private Stream stream;
            private byte[] buffer = new byte[4];
            private int value;
            private int shift;
            private bool atEnd;

            public BitReader(Stream inStream)
            {
                stream = inStream;
            }

            ~BitReader()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            public bool GetBit()
            {
                if (shift >= 32)
                {
                    if (stream.Position >= stream.Length)
                    {
                        atEnd = true;
                        return false;
                    }

                    shift = 0;
                    FillBuffer();
                }

                return ((value >> (shift++)) & 1) == 1;
            }

            public void SetPosition(int pos)
            {
                stream.Position = (pos >> 5) * 4;
                shift = pos & 0x1f;

                FillBuffer();
            }

            private void FillBuffer()
            {
                stream.Read(buffer, 0, 4);
                value = BitConverter.ToInt32(buffer, 0);
            }

            private void Dispose(bool disposing = false)
            {
                if (disposing)
                {
                    stream.Dispose();
                    stream = null;

                    GC.SuppressFinalize(this);
                }
            }
        }

        static void Main(string[] args)
        {
            //if (args.Length < 2)
            //    return;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // create profile bin
            ProfileCreator profileCreator = new ProfileCreator();
            profileCreator.CreateProfiles();

            //string basePath = args[0];
            //string command = args[1].ToLower();

            //if (command == "profile")
            //{
            //    ProfileCreator profileCreator = new ProfileCreator();
            //    profileCreator.CreateProfiles();
            //    return;
            //}

            //FileInfo baseFile = new FileInfo(basePath);
            //ProfilesLibrary.Initialize(baseFile.Name.Replace(baseFile.Extension, ""));

            //byte[] keyData = null;
            //if (ProfilesLibrary.RequiresKey)
            //{
            //    using (NativeReader reader = new NativeReader(new FileStream(ProfilesLibrary.CacheName + ".key", FileMode.Open, FileAccess.Read)))
            //        keyData = reader.ReadToEnd();

            //    // add primary encryption key
            //    byte[] key = new byte[0x10];
            //    Array.Copy(keyData, key, 0x10);
            //    KeyManager.Instance.AddKey("Key1", key);

            //    if (keyData.Length > 0x10)
            //    {
            //        // add additional encryption keys
            //        key = new byte[0x10];
            //        Array.Copy(keyData, 0x10, key, 0, 0x10);
            //        KeyManager.Instance.AddKey("Key2", key);

            //        key = new byte[0x4000];
            //        Array.Copy(keyData, 0x20, key, 0, 0x4000);
            //        KeyManager.Instance.AddKey("Key3", key);
            //    }
            //}
            //{
            //    FileSystem fs = new FileSystem(baseFile.DirectoryName);
            //    foreach (FileSystemSource source in ProfilesLibrary.Sources)
            //        fs.AddSource(source.Path, source.SubDirs);
            //    fs.Initialize(KeyManager.Instance.GetKey("Key1"));

            //    ResourceManager rm = new ResourceManager(fs);
            //    rm.SetLogger(logger);
            //    rm.Initialize();

            //    AssetManager am = new AssetManager(fs, rm);
            //    am.SetLogger(logger);
            //    am.Initialize(false);
            //}
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllname = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name;
            if (dllname.Equals("EbxClasses"))
            {
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                return Assembly.LoadFile(fi.DirectoryName + "/Profiles/" + ProfilesLibrary.SDKFilename + ".dll");
            }
            return null;
        }
    }
}
