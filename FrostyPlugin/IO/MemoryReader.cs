using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Frosty.Core.IO
{
    struct PatternType
    {
        public bool isWildcard;
        public byte value;
    }

    public class MemoryReader : IDisposable
    {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualProtectEx(IntPtr hProcess, long lpAddress, UIntPtr dwSize, uint flNewProtect, ref uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        public virtual long Position
        {
            get => position;
            set => position = value;
        }

        private IntPtr handle;
        protected byte[] buffer = new byte[20];
        protected long position;

        internal MemoryReader()
        {
        }

        public MemoryReader(Process process, long initialAddr)
        {
            handle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            position = initialAddr;
        }

        public virtual void Dispose()
        {
            CloseHandle(handle);
        }

        public byte ReadByte()
        {
            FillBuffer(1);
            return buffer[0];
        }

        public short ReadShort()
        {
            FillBuffer(2);
            return (short)(buffer[0] | buffer[1] << 8);
        }

        public ushort ReadUShort()
        {
            FillBuffer(2);
            return (ushort)(buffer[0] | buffer[1] << 8);
        }

        public int ReadInt()
        {
            FillBuffer(4);
            return buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24;
        }

        public uint ReadUInt()
        {
            FillBuffer(4);
            return (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
        }

        public long ReadLong()
        {
            FillBuffer(8);
            return (long)(uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24) << 32 |
                   (long)(uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
        }

        public ulong ReadULong()
        {
            FillBuffer(8);
            return (ulong)(uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24) << 32 |
                   (ulong)(uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
        }

        public Guid ReadGuid()
        {
            FillBuffer(16);
            return new Guid(new byte[] {
                        buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7],
                        buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]
                    });
        }

        public string ReadNullTerminatedString()
        {
            long offset = ReadLong();
            long orig = Position;
            Position = offset;

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char c = (char)ReadByte();
                if (c == 0x00)
                    break;

                sb.Append(c);
            }

            Position = orig;
            return sb.ToString();
        }

        public byte[] ReadBytes(int numBytes)
        {
            byte[] outBuffer = new byte[numBytes];

            uint oldProtect = 0;
            int bytesRead = 0;

            VirtualProtectEx(handle, position, new UIntPtr((uint)numBytes), 0x02, ref oldProtect);
            if (!ReadProcessMemory(handle, position, outBuffer, numBytes, ref bytesRead))
                return null;
            VirtualProtectEx(handle, position, new UIntPtr((uint)numBytes), oldProtect, ref oldProtect);

            position += numBytes;
            return outBuffer;
        }

        public unsafe IList<long> scan(string pattern)
        {
            List<long> retList = new List<long>();
            pattern = pattern.Replace(" ", "");

            PatternType[] bytePattern = new PatternType[pattern.Length / 2];
            for (int i = 0; i < bytePattern.Length; i++)
            {
                string str = pattern.Substring(i * 2, 2);
                bytePattern[i] = new PatternType() { isWildcard = (str == "??"), value = (str != "??") ? byte.Parse(pattern.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber) : (byte)0x00 };
            }

            bool bFound = false;

            long pos = Position;
            byte[] buf = ReadBytes(1024 * 1024);
            byte* startPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
            byte* ptr = startPtr;
            byte* endPtr = ptr + (1024 * 1024);
            byte* tmpPtr = ptr;

            while (buf != null)
            {
                if (*ptr == bytePattern[0].value)
                {
                    tmpPtr = ptr;
                    bFound = true;

                    for (int i = 0; i < bytePattern.Length; i++)
                    {
                        if (!bytePattern[i].isWildcard && *tmpPtr != bytePattern[i].value)
                        {
                            bFound = false;
                            break;
                        }

                        tmpPtr++;
                    }

                    if (bFound)
                    {
                        retList.Add(tmpPtr - startPtr - bytePattern.Length + pos);
                        bFound = false;
                    }
                }

                ptr++;
                if (ptr == endPtr)
                {
                    pos = Position;
                    buf = ReadBytes(1024 * 1024);
                    if (buf == null)
                        break;

                    startPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
                    ptr = startPtr;
                    endPtr = ptr + (1024 * 1024);
                }
            }

            return retList;
        }

        protected virtual void FillBuffer(int numBytes)
        {
            uint oldProtect = 0;
            int bytesRead = 0;

            VirtualProtectEx(handle, position, new UIntPtr((uint)numBytes), 0x02, ref oldProtect);
            ReadProcessMemory(handle, position, buffer, numBytes, ref bytesRead);
            VirtualProtectEx(handle, position, new UIntPtr((uint)numBytes), oldProtect, ref oldProtect);

            position += numBytes;
        }
    }
}
