using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.CatResources;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Infos.FileInfos;
using Frosty.Sdk.Utils.CompressionTypes;

namespace Frosty.Sdk.Managers;

public static class ResourceManager
{
    public static bool IsInitialized { get; private set; }
    
    private static readonly Dictionary<Sha1, List<IFileInfo>> s_resourceEntries = new();

    private static readonly List<CatPatchEntry> s_patchEntries = new();

    public static void LoadInstallChunks()
    {
        foreach (InstallChunkInfo installChunkInfo in FileSystemManager.EnumerateInstallChunks())
        {
            LoadInstallChunk(installChunkInfo);
        }

        foreach (CatPatchEntry entry in s_patchEntries)
        {
            List<IFileInfo> baseEntry = s_resourceEntries[entry.BaseSha1];
            List<IFileInfo> deltaEntry = s_resourceEntries[entry.DeltaSha1];
            
            for (int j = 0; j < baseEntry.Count; j++)
            {
                PatchFileInfo fileInfo = new(deltaEntry[j], baseEntry[j]);
                
                s_resourceEntries.TryAdd(entry.Sha1, new List<IFileInfo>());
                s_resourceEntries[entry.Sha1].Add(fileInfo);
            }
        }
    }
    
    private static void LoadInstallChunk(InstallChunkInfo info)
    {
        LoadEntries(info, true);
        LoadEntries(info, false);
    }

    private static void LoadEntries(InstallChunkInfo info, bool patch)
    {
        string filePath = FileSystemManager.ResolvePath($"{(patch ? "native_patch" : "native_data")}/{info.InstallBundle}/cas.cat");

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }
        
        int installChunkIndex = FileSystemManager.GetInstallChunkIndex(info);
        
        using (CatStream stream = new(filePath))
        {
            for (int i = 0; i < stream.ResourceCount; i++)
            {
                CatResourceEntry entry = stream.ReadResourceEntry();
                CasFileIdentifier casFileIdentifier = new(patch, installChunkIndex, entry.ArchiveIndex);

                CasFileInfo fileInfo = new(casFileIdentifier, entry.Offset, entry.Size, entry.LogicalOffset);
                
                s_resourceEntries.TryAdd(entry.Sha1, new List<IFileInfo>());
                s_resourceEntries[entry.Sha1].Add(fileInfo);
            }
        
            for (int i = 0; i < stream.EncryptedCount; i++)
            {
                CatResourceEntry entry = stream.ReadEncryptedEntry();
                CasFileIdentifier casFileIdentifier = new(patch, installChunkIndex, entry.ArchiveIndex);

                CryptoCasFileInfo fileInfo = new(casFileIdentifier, entry.Offset, entry.Size, entry.LogicalOffset,
                    entry.KeyId, entry.Checksum);
                
                s_resourceEntries.TryAdd(entry.Sha1, new List<IFileInfo>());
                s_resourceEntries[entry.Sha1].Add(fileInfo);
            }
        
            for (int i = 0; i < stream.PatchCount; i++)
            {
                s_patchEntries.Add(stream.ReadPatchEntry());
            }
        }
    }

    public static void CLearInstallChunks()
    {
        s_resourceEntries.Clear();
        s_patchEntries.Clear();
    }
    
    public static bool Initialize()
    {
        if (IsInitialized)
        {
            return true;
        }

        if (!FileSystemManager.IsInitialized)
        {
            return false;
        }

        if (FileSystemManager.HasFileInMemoryFs("Dictionaries/ebx.dict"))
        {
            // load dictionary from memoryFs (used for decompressing ebx)
            ZStd.SetDictionary(FileSystemManager.GetFileFromMemoryFs("Dictionaries/ebx.dict"));
        }
        
        if (FileSystemManager.HasFileInMemoryFs("Scripts/CasEncrypt.yaml"))
        {
            // load casencrypt.yaml from memoryFs
            using (TextReader stream = new StreamReader(new MemoryStream(FileSystemManager.GetFileFromMemoryFs("Scripts/CasEncrypt.yaml"))))
            {
                byte[]? key = null;
                while (stream.Peek() != -1)
                {
                    string line = stream.ReadLine()!;
                    if (line.Contains("keyid:"))
                    {
                        string[] arr = line.Split(':');
                        KeyManager.AddKey(arr[1].Trim(), key!);
                    }
                    else if (line.Contains("key:"))
                    {
                        string[] arr = line.Split(':');
                        string keyStr = arr[1].Trim();

                        key = new byte[keyStr.Length / 2];
                        for(int i = 0; i < keyStr.Length / 2; i++)
                        {
                            key[i] = Convert.ToByte(keyStr.Substring(i * 2, 2), 16);
                        }
                    }
                }
            }
        }
        
        IsInitialized = true;
        return true;
    }

    public static long GetSize(Sha1 sha1)
    {
        if (!s_resourceEntries.TryGetValue(sha1, out List<IFileInfo>? fileInfos))
        {
            return -1;
        }

        return fileInfos.First(fi => fi.IsComplete()).GetSize();
    }

    public static IEnumerable<IFileInfo>? GetPatchFileInfos(Sha1 sha1, Sha1 deltaSha1, Sha1 baseSha1)
    {
        if (s_resourceEntries.TryGetValue(sha1, out List<IFileInfo>? _))
        {
            return null;
        }
        
        List<IFileInfo> baseInfos = s_resourceEntries[baseSha1];
        List<IFileInfo> deltaEntry = s_resourceEntries[deltaSha1];

        for (int j = 0; j < baseInfos.Count; j++)
        {
            PatchFileInfo fileInfo = new(deltaEntry[j], baseInfos[j]);
                    
            s_resourceEntries.TryAdd(sha1, new List<IFileInfo>());
            s_resourceEntries[sha1].Add(fileInfo);
        }

        return s_resourceEntries[sha1];
    }

    public static IEnumerable<IFileInfo>? GetFileInfos(Sha1 sha1)
    {
        if (!s_resourceEntries.TryGetValue(sha1, out List<IFileInfo>? fileInfos))
        {
            return null;
        }

        s_resourceEntries.Remove(sha1);
        return fileInfos;
    }
}