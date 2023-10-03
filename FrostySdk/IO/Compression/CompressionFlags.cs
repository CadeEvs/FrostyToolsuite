using System;

namespace Frosty.Sdk.IO.Compression;

[Flags]
public enum CompressionFlags
{
    None = 0,
    ZStdUseDicts = 1 << 0,
    OodleKraken = 1 << 1,
    OodleSelkie = 1 << 2,
    OodleLeviathan = 1 << 3,
}