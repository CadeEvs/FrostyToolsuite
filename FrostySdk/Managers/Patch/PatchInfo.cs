using System;
using System.Collections.Generic;

namespace Frosty.Sdk.Managers.Patch;

public class PatchInfo
{
    public List<string> Ebx { get; set; } = new();
    public List<string> Res { get; set; } = new();
    public List<Guid> Chunks { get; set; } = new();
}