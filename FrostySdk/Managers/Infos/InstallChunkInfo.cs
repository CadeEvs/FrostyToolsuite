using System;
using System.Collections.Generic;

namespace Frosty.Sdk.Managers.Infos;

public class InstallChunkInfo
{
    public Guid Id;
    public string Name = string.Empty;
    public string InstallBundle = string.Empty;
    public bool AlwaysInstalled;
    public readonly HashSet<Guid> RequiredCatalogs = new();
    public readonly HashSet<string> SuperBundles = new();
    public readonly HashSet<string> SplitSuperBundles = new();

    public bool RequiresInstallChunk(InstallChunkInfo b)
    {
        if (RequiredCatalogs.Count == 0)
        {
            return false;
        }
        
        if (RequiredCatalogs.Contains(b.Id))
        {
            return true;
        }

        foreach (Guid id in RequiredCatalogs)
        {
            InstallChunkInfo c = FileSystemManager.GetInstallChunkInfo(id);
            if (c.RequiresInstallChunk(b))
            {
                return true;
            }
        }

        return false;
    }
}