using System;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Interfaces;

public interface IDeobfuscator
{
    public void Deobfuscate(Span<byte> header, Block<byte> data);
}