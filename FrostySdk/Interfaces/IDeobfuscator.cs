using System.IO;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Interfaces;

public interface IDeobfuscator
{
    /// <summary>
    /// Deobfuscates stream.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> which gets deobfuscated.</param>
    /// <returns>The deobfuscated stream without the Deobfusctation header.</returns>
    Stream? Initialize(DataStream stream);
}