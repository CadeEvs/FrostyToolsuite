using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace FrostyModManager.Compression
{
    public static class UnRar
    {
        public enum RARRetCode : int
        {
            Success,
            EndArchive = 10,
            NoMemory = 11,
            BadData = 12,
            BadArchive = 13,
            UnknownFormat = 14,
            EOpen = 15,
            ECreate = 16,
            EClose = 17,
            ERead = 18,
            EWrite = 19,
            SmallBuf = 20,
            Unknown = 21,
            MissingPassword = 22,
            EReference = 23,
            BadPassword = 24
        };

        public enum RAROperation : uint
        {
            Skip = 0,
            Test = 1,
            Extract = 2
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAROpenArchiveDataEx
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string ArcName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ArcNameW;
            public uint OpenMode;
            public uint OpenResult;
            [MarshalAs(UnmanagedType.LPStr)]
            public string CmtBuf;
            public uint CmtBufSize;
            public uint CmtSize;
            public uint CmtState;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public uint[] Reserved;

            public void Initialize()
            {
                CmtBuf = new string((char)0, 65536);
                CmtBufSize = 65536;
                Reserved = new uint[32];
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RARHeaderDataEx
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string ArcName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string ArcNameW;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string FileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string FileNameW;
            public uint Flags;
            public uint PackSize;
            public uint PackSizeHigh;
            public uint UnpSize;
            public uint UnpSizeHigh;
            public uint HostOS;
            public uint FileCRC;
            public uint FileTime;
            public uint UnpVer;
            public uint Method;
            public uint FileAttr;
            [MarshalAs(UnmanagedType.LPStr)]
            public string CmtBuf;
            public uint CmtBufSize;
            public uint CmtSize;
            public uint CmtState;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public uint[] Reserved;

            public void Initialize()
            {
                CmtBuf = new string((char)0, 65536);
                CmtBufSize = 65536;
            }
        }

        public delegate int UNRARCALLBACK(uint msg, int UserData, IntPtr p1, int p2);
        public delegate int PROCESSDATAPROC(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            byte[] data,
            int size);

        [DllImport("ThirdParty/UnRAR.dll", EntryPoint = "RAROpenArchiveEx")]
        public static extern IntPtr OpenArchiveEx(ref RAROpenArchiveDataEx ArchiveData);

        [DllImport("ThirdParty/UnRAR.dll", EntryPoint = "RARSetCallback")]
        public static extern void SetCallback(IntPtr hArcData, UNRARCALLBACK callback, int userData);

        [DllImport("ThirdParty/UnRAR.dll", EntryPoint = "RARReadHeaderEx")]
        public static extern RARRetCode ReadHeaderEx(IntPtr hArcData, ref RARHeaderDataEx headerData);

        [DllImport("ThirdParty/UnRAR.dll", EntryPoint = "RARProcessFile")]
        public static extern RARRetCode ProcessFile(IntPtr hArcData, RAROperation operation,
            [MarshalAs(UnmanagedType.LPStr)] string destPath,
            [MarshalAs(UnmanagedType.LPStr)] string destName);

        [DllImport("ThirdParty/UnRAR.dll", EntryPoint = "RARSetProcessDataProc")]
        public static extern void SetProcessDataProc(IntPtr hArcData, PROCESSDATAPROC processDataProc);

        [DllImport("ThirdParty/UnRAR.dll", EntryPoint = "RARCloseArchive")]
        public static extern int CloseArchive(IntPtr hArcData);
    }

    public class RarDecompressor : IDecompressor
    {
        private IntPtr handle;
        UnRar.RARHeaderDataEx currentHeader;

        public bool OpenArchive(string filename)
        {
            UnRar.RAROpenArchiveDataEx archiveData = new UnRar.RAROpenArchiveDataEx();
            archiveData.Initialize();
            archiveData.ArcName = filename + "\0";
            archiveData.ArcNameW = filename + "\0";
            archiveData.CmtBuf = null;
            archiveData.CmtBufSize = 0;
            archiveData.OpenMode = 1;

            handle = UnRar.OpenArchiveEx(ref archiveData);
            return archiveData.OpenResult == 0;
        }

        public void CloseArchive()
        {
            if (handle == IntPtr.Zero)
                return;

            UnRar.SetProcessDataProc(handle, null);
            UnRar.CloseArchive(handle);

            processDataProc = null;
            handle = IntPtr.Zero;
        }

        public IEnumerable<CompressedFileInfo> EnumerateFiles()
        {
            currentHeader = new UnRar.RARHeaderDataEx();
            currentHeader.Initialize();

            UnRar.RARRetCode retCode = UnRar.ReadHeaderEx(handle, ref currentHeader);
            if (retCode == UnRar.RARRetCode.Success)
            {
                while (retCode != UnRar.RARRetCode.EndArchive)
                {
                    if ((currentHeader.Flags & 0x20) == 0)
                    {
                        yield return new CompressedFileInfo(currentHeader.FileNameW, ((ulong)currentHeader.PackSizeHigh << 16) | currentHeader.PackSize, ((ulong)currentHeader.UnpSizeHigh << 16) | currentHeader.UnpSize);
                    }

                    // move onto next file
                    if (UnRar.ProcessFile(handle, UnRar.RAROperation.Skip, string.Empty, string.Empty) != UnRar.RARRetCode.Success)
                        break;

                    // read next header
                    retCode = UnRar.ReadHeaderEx(handle, ref currentHeader);
                    if (retCode == UnRar.RARRetCode.BadData)
                    {
                        // uh-oh error
                        throw new InvalidDataException();
                    }
                }
            }
            else
                // uh-oh error
                throw new InvalidDataException();
        }

        private UnRar.PROCESSDATAPROC processDataProc;
        public byte[] DecompressToMemory()
        {
            ulong decompressedSize = ((ulong)currentHeader.UnpSizeHigh << 16) | currentHeader.UnpSize;

            using (MemoryStream ms = new MemoryStream())
            {
                processDataProc = new UnRar.PROCESSDATAPROC((a, b) =>
                {
                    ms.Write(a, 0, b);
                    return (ms.Length != (long)decompressedSize) ? 1 : 0;
                });

                UnRar.SetProcessDataProc(handle, processDataProc);
                UnRar.ProcessFile(handle, UnRar.RAROperation.Extract, string.Empty, string.Empty);

                return ms.ToArray();
            }
        }

        public void DecompressToFile(string filename)
        {
            byte[] buffer = DecompressToMemory();
            using (NativeWriter writer = new NativeWriter(new FileStream(filename, FileMode.Create)))
                writer.Write(buffer);
        }
    }
}
