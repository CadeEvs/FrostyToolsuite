using System;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Deobfuscators;

public class LegacyDeobfuscator : IDeobfuscator
{
    public void Deobfuscate(Span<byte> header, Block<byte> data)
    {
        if (header[3] != 0x01)
        {
            return;
        }
        for (int i = 0; i < data.Size; i++)
        {
            data[i] ^= (byte)(0x7B ^ header[0x128 + i % 0x101]);
        }
    }
}