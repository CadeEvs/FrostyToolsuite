using System;

namespace Frosty.Sdk.Managers.Infos;

[Flags]
public enum InstallChunkType
{
    Default = 1 << 0,
    Split = 1 << 1
}