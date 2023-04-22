using System;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Deobfuscators;

public class LegacyDeobfuscator : IDeobfuscator
{
    private byte[]? m_key;

    public bool Initialize(DataStream stream)
    {
        uint magic = stream.ReadUInt32();
        if (magic != 0x00CED100 && magic != 0x01CED100 && magic != 0x03CED100)
        {
            stream.Position = 0;
            return false;
        }
        
        if (magic == 0x01CED100)
        {
            stream.Position = 0x128;

            m_key = stream.ReadBytes(0x101);
        }
        
        stream.Position = 0x22C;

        return true;
    }

    public long GetPosition(long position) => position - 0x22C;

    public long SetPosition(long position) => position + 0x22C;

    public void Deobfuscate(long position, byte[] buffer, int count)
    {
        if (m_key is null)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            buffer[i] ^= (byte)(0x7B ^ m_key[(position + i) % 0x101]);
        }
    }

    public void Deobfuscate(long position, Span<byte> buffer, int count)
    {
        if (m_key is null)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            buffer[i] ^= (byte)(0x7B ^ m_key[(position + i) % 0x101]);
        }
    }
}