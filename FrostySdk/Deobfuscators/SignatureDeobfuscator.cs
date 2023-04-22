using System;
using System.Drawing;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Deobfuscators;

public class SignatureDeobfuscator : IDeobfuscator
{
    public bool Initialize(DataStream stream)
    {
        // | magic | unused |      signature      |      unused      |
        uint magic = stream.ReadUInt32();
        if (magic != 0x00CED100 && magic != 0x01CED100 && magic != 0x03CED100)
        {
            stream.Position = 0;
            return false;
        }
        
        stream.Position = 0x22C;

        return true;
    }
    
    public long GetPosition(long position) => position - 0x22C;

    public long SetPosition(long position) => position + 0x22C;

    public void Deobfuscate(long position, byte[] buffer, int count)
    {
    }

    public void Deobfuscate(long position, Span<byte> buffer, int count)
    {
    }
}