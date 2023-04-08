using System;
using System.Collections.Generic;

namespace Frosty.Sdk.Managers.Infos;

public class InstallChunkInfo
{
    public Guid Id;
    public string Name = string.Empty;
    public string InstallBundle = string.Empty;
    public bool AlwaysInstalled;
    public List<Guid> RequiredCatalogs = new();
    public List<string> SuperBundles = new();
    public List<string> SplitSuperBundles = new();
}