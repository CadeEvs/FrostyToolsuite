using System;
using System.IO;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Interfaces;

public interface IDeobfuscator
{
    /// <summary>
    /// Deobfuscates stream.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> which gets deobfuscated.</param>
    /// <returns>True if it is a obfuscated stream.</returns>
    bool Initialize(DataStream stream);

    long GetPosition(long position);
    
    long SetPosition(long position);

    void Deobfuscate(long position, byte[] buffer, int count);
    
    void Deobfuscate(long position, Span<byte> buffer, int count);
}