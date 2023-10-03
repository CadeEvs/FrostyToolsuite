using System;

namespace Frosty.Sdk.IO.Compression;

[Flags]
public enum CompressionType : byte
{
    None = 0x00,
    ZLib = 0x02,
    // ReSharper disable once InconsistentNaming
    LZ4 = 0x09,
    ZStd = 0x0F,
    OodleKraken = 0x11,
    OodleSelkie = 0x15,
    OodleLeviathan = 0x19,

    Obfuscated = 1 << 7
}