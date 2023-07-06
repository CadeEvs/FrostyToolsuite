using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Frosty.Sdk.IO;

struct PatternType
{
    public bool IsWildcard;
    public byte Value;
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
        get => m_position;
        set => m_position = value;
    }

    private IntPtr m_handle;
    protected readonly byte[] m_buffer = new byte[20];
    protected long m_position;

    public MemoryReader(Process process, long initialAddr)
    {
        m_handle = OpenProcess(PROCESS_WM_READ, false, process.Id);
        m_position = initialAddr;
    }

    public virtual void Dispose()
    {
        CloseHandle(m_handle);
    }

    public void Pad(int alignment)
    {
        while (Position % alignment != 0)
        {
            Position++;
        }
    }

    public byte ReadByte()
    {
        FillBuffer(1);
        return m_buffer[0];
    }

    public short ReadShort(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(short));
        }

        FillBuffer(2);
        return (short)(m_buffer[0] | m_buffer[1] << 8);
    }

    public ushort ReadUShort(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(ushort));
        }

        FillBuffer(2);
        return (ushort)(m_buffer[0] | m_buffer[1] << 8);
    }

    public int ReadInt(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(int));
        }

        FillBuffer(4);
        return m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24;
    }

    public uint ReadUInt(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(uint));
        }

        FillBuffer(4);
        return (uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);
    }

    public long ReadLong(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(long));
        }

        FillBuffer(8);
        return (long)(uint)(m_buffer[4] | m_buffer[5] << 8 | m_buffer[6] << 16 | m_buffer[7] << 24) << 32 |
               (long)(uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);
    }

    public ulong ReadULong(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(ulong));
        }

        FillBuffer(8);
        return (ulong)(uint)(m_buffer[4] | m_buffer[5] << 8 | m_buffer[6] << 16 | m_buffer[7] << 24) << 32 |
               (ulong)(uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);
    }

    public Guid ReadGuid(bool pad = true)
    {
        if (pad)
        {
            Pad(4);
        }

        FillBuffer(16);
        return new Guid(new byte[] {
                    m_buffer[0], m_buffer[1], m_buffer[2], m_buffer[3], m_buffer[4], m_buffer[5], m_buffer[6], m_buffer[7],
                    m_buffer[8], m_buffer[9], m_buffer[10], m_buffer[11], m_buffer[12], m_buffer[13], m_buffer[14], m_buffer[15]
                });
    }

    public string ReadNullTerminatedString(bool pad = true)
    {
        if (pad)
        {
            Pad(sizeof(long));
        }

        long offset = ReadLong();
        long orig = Position;
        Position = offset;

        StringBuilder sb = new();
        while (true)
        {
            char c = (char)ReadByte();
            if (c == 0x00)
            {
                break;
            }

            sb.Append(c);
        }

        Position = orig;
        return sb.ToString();
    }

    public byte[]? ReadBytes(int numBytes)
    {
        byte[] outBuffer = new byte[numBytes];

        uint oldProtect = 0;
        int bytesRead = 0;

        VirtualProtectEx(m_handle, m_position, new UIntPtr((uint)numBytes), 0x02, ref oldProtect);
        if (!ReadProcessMemory(m_handle, m_position, outBuffer, numBytes, ref bytesRead))
        {
            return null;
        }

        VirtualProtectEx(m_handle, m_position, new UIntPtr((uint)numBytes), oldProtect, ref oldProtect);

        m_position += numBytes;
        return outBuffer;
    }

    public unsafe IList<long> Scan(string pattern)
    {
        List<long> retList = new List<long>();
        pattern = pattern.Replace(" ", "");

        PatternType[] bytePattern = new PatternType[pattern.Length / 2];
        for (int i = 0; i < bytePattern.Length; i++)
        {
            string str = pattern.Substring(i * 2, 2);
            bytePattern[i] = new PatternType() { IsWildcard = (str == "??"), Value = (str != "??") ? byte.Parse(pattern.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber) : (byte)0x00 };
        }

        bool bFound = false;

        long pos = Position;
        byte[]? buf = ReadBytes(1024 * 1024);
        byte* startPtr = buf == null ? null : (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
        byte* ptr = startPtr;
        byte* endPtr = ptr + (1024 * 1024);
        byte* tmpPtr = ptr;

        while (buf != null)
        {
            if (*ptr == bytePattern[0].Value)
            {
                tmpPtr = ptr;
                bFound = true;

                for (int i = 0; i < bytePattern.Length; i++)
                {
                    if (!bytePattern[i].IsWildcard && *tmpPtr != bytePattern[i].Value)
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
                {
                    break;
                }

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

        VirtualProtectEx(m_handle, m_position, new UIntPtr((uint)numBytes), 0x02, ref oldProtect);
        ReadProcessMemory(m_handle, m_position, m_buffer, numBytes, ref bytesRead);
        VirtualProtectEx(m_handle, m_position, new UIntPtr((uint)numBytes), oldProtect, ref oldProtect);

        m_position += numBytes;
    }
}