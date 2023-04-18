using System;
using System.Collections.Generic;
using System.IO;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.CatResources;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Utils.CompressionTypes;
using FileInfo = Frosty.Sdk.Managers.Infos.FileInfo;

namespace Frosty.Sdk.Managers;

public static class ResourceManager
{
    public static bool IsInitialized;

    private static readonly Dictionary<Sha1, CatResourceEntry> s_resourceEntries = new();
    private static readonly Dictionary<Sha1, CatPatchEntry> s_patchEntries = new();
    private static readonly Dictionary<int, string> s_casFiles = new();
    
    public static bool Initialize()
    {
        if (!FileSystemManager.IsInitialized)
        {
            return false;
        }

        foreach (InstallChunkInfo installChunkInfo in FileSystemManager.EnumerateInstallChunks())
        {
            LoadCatalog($"native_data/{installChunkInfo.InstallBundle}");
            LoadCatalog($"native_patch/{installChunkInfo.InstallBundle}");
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

        // load oodle library if there is one
        Oodle.TryBind();
        
        IsInitialized = true;
        return true;
    }

    #region -- GetResourceData --

    public static Stream GetResourceData(Sha1 sha1)
    {
        // newer games store patch entries in the cat and not in the bundle
        if (s_patchEntries.TryGetValue(sha1, out CatPatchEntry patchEntry))
        {
            return GetResourceData(patchEntry.BaseSha1, patchEntry.DeltaSha1);
        }

        if (!s_resourceEntries.TryGetValue(sha1, out CatResourceEntry entry))
        {
            throw new Exception();
        }

        if (entry.IsEncrypted && !KeyManager.HasKey(entry.KeyId))
        {
            throw new Exception("Missing Key");
        }

        using (DataStream stream = new(new FileStream(s_casFiles[entry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
        {
            using (CasStream reader = new(stream.CreateViewStream(entry.Offset, entry.Size),
                       (entry.IsEncrypted) ? KeyManager.GetKey(entry.KeyId) : null, entry.EncryptedSize))
            {
                return new MemoryStream(reader.Read());
            }
        }
    }
    
    public static Stream GetResourceData(Sha1 baseSha1, Sha1 deltaSha1)
    {
        if (!s_resourceEntries.TryGetValue(baseSha1, out CatResourceEntry baseEntry) || !s_resourceEntries.TryGetValue(deltaSha1, out CatResourceEntry deltaEntry))
        {
            throw new Exception();
        }

        using (DataStream baseReader = new (new FileStream(s_casFiles[baseEntry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
        {
            using (DataStream deltaReader = (deltaEntry.ArchiveIndex == baseEntry.ArchiveIndex) ? baseReader : new(new FileStream(s_casFiles[deltaEntry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
            {
                byte[]? baseKey = (baseEntry.IsEncrypted && KeyManager.HasKey(baseEntry.KeyId)) ? KeyManager.GetKey(baseEntry.KeyId) : null;
                byte[]? deltaKey = (deltaEntry.IsEncrypted && KeyManager.HasKey(deltaEntry.KeyId)) ? KeyManager.GetKey(deltaEntry.KeyId) : null;

                if (baseEntry.IsEncrypted && baseKey == null || deltaEntry.IsEncrypted && deltaKey == null)
                {
                    throw new Exception("Missing Key");
                }

                using (CasStream reader =
                       new(baseReader.CreateViewStream(baseEntry.Offset, baseEntry.Size), baseKey,
                           baseEntry.EncryptedSize,
                           deltaReader.CreateViewStream(deltaEntry.Offset, deltaEntry.Size), deltaKey,
                           deltaEntry.EncryptedSize))
                {
                    return new MemoryStream(reader.Read());
                }
            }
        }
    }

    public static Stream GetResourceData(FileInfo fileInfo)
    {
        using (DataStream stream = new(new FileStream(fileInfo.Path, FileMode.Open, FileAccess.Read)))
        {
            using (CasStream casStream = new(stream.CreateViewStream(fileInfo.Offset, fileInfo.Size)))
            {
                return new MemoryStream(casStream.Read());
            }
        }
    }

    #endregion

    #region -- GetRawResourceData --

    public static Stream GetRawResourceData(Sha1 sha1)
    {
        // newer games store patch entries in the cat and not in the bundle
        if (s_patchEntries.TryGetValue(sha1, out CatPatchEntry patchEntry))
        {
            return GetRawResourceData(patchEntry.BaseSha1, patchEntry.DeltaSha1);
        }

        if (!s_resourceEntries.TryGetValue(sha1, out CatResourceEntry entry))
        {
            throw new Exception();
        }

        if (entry.IsEncrypted/* && !KeyManager.HasKey(entry.KeyId)*/)
        {
            //throw new Exception("Missing Key");
            throw new NotImplementedException();
        }

        using (DataStream stream = new(new FileStream(s_casFiles[entry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
        {
            stream.Position = entry.Offset;
            return new MemoryStream(stream.ReadBytes((int)entry.Size));
        }
    }
    
    public static Stream GetRawResourceData(Sha1 baseSha1, Sha1 deltaSha1)
    {
        if (!s_resourceEntries.TryGetValue(baseSha1, out CatResourceEntry baseEntry) || !s_resourceEntries.TryGetValue(deltaSha1, out CatResourceEntry deltaEntry))
        {
            throw new Exception();
        }

        using (DataStream baseReader = new (new FileStream(s_casFiles[baseEntry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
        {
            using (DataStream deltaReader = (deltaEntry.ArchiveIndex == baseEntry.ArchiveIndex) ? baseReader : new(new FileStream(s_casFiles[deltaEntry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
            {
                byte[]? baseKey = (baseEntry.IsEncrypted && KeyManager.HasKey(baseEntry.KeyId)) ? KeyManager.GetKey(baseEntry.KeyId) : null;
                byte[]? deltaKey = (deltaEntry.IsEncrypted && KeyManager.HasKey(deltaEntry.KeyId)) ? KeyManager.GetKey(deltaEntry.KeyId) : null;

                if (baseEntry.IsEncrypted && baseKey == null || deltaEntry.IsEncrypted && deltaKey == null)
                {
                    throw new Exception("Missing Key");
                }

                throw new NotImplementedException();
            }
        }
    }

    public static Stream GetRawResourceData(FileInfo fileInfo)
    {
        using (DataStream stream = new(new FileStream(fileInfo.Path, FileMode.Open, FileAccess.Read)))
        {
            stream.Position = fileInfo.Offset;
            return new MemoryStream(stream.ReadBytes((int)fileInfo.Size));
        }
    }
    
    #endregion

    /// <summary>
    /// Loads all entries from the cas.cat file if it exists.
    /// </summary>
    /// <param name="installBundle">Name of the InstallBundle with data/patch prefix.</param>
    private static void LoadCatalog(string installBundle)
    {
        string fullPath = FileSystemManager.ResolvePath($"{installBundle}/cas.cat");
        if (!File.Exists(fullPath))
        {
            return;
        }

        using (CatStream stream = new(new FileStream(fullPath, FileMode.Open, FileAccess.Read)))
        {
            // make sure the dicts are big enough
            s_resourceEntries.EnsureCapacity((int)(s_resourceEntries.Count + stream.ResourceCount + stream.EncryptedCount));
            s_patchEntries.EnsureCapacity((int)(s_patchEntries.Count + stream.PatchCount));
            
            for (int i = 0; i < stream.ResourceCount; i++)
            {
                CatResourceEntry entry = stream.ReadResourceEntry();
                entry.ArchiveIndex = AddCas(installBundle, entry.ArchiveIndex);
        
                if (entry.LogicalOffset == 0)
                {
                    s_resourceEntries.TryAdd(entry.Sha1, entry);
                }
            }
        
            for (int i = 0; i < stream.EncryptedCount; i++)
            {
                CatResourceEntry entry = stream.ReadEncryptedEntry();
                entry.ArchiveIndex = AddCas(installBundle, entry.ArchiveIndex);
        
                if (entry.LogicalOffset == 0)
                {
                    s_resourceEntries.TryAdd(entry.Sha1, entry);
                }
            }
        
            for (int i = 0; i < stream.PatchCount; i++)
            {
                CatPatchEntry entry = stream.ReadPatchEntry();
                s_patchEntries.TryAdd(entry.Sha1, entry);
            }
        }
    }
    
    /// <summary>
    /// Adds a cas file to a hashmap for lookup.
    /// </summary>
    /// <param name="installBundle">The name of the InstallBundle of the InstallChunk with data/patch prefix.</param>
    /// <param name="archiveIndex">The index of the cas.</param>
    /// <returns></returns>
    private static int AddCas(string installBundle, int archiveIndex)
    {
        string casFilename = $"{installBundle}/cas_{archiveIndex:d2}.cas";
        int hash = Utils.Utils.HashString(casFilename, true);

        s_casFiles.TryAdd(hash, FileSystemManager.ResolvePath(casFilename));

        return hash;
    }
}