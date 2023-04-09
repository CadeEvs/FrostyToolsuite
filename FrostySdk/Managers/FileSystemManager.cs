using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Infos;

namespace Frosty.Sdk.Managers;

public static class FileSystemManager
{
    public static bool IsInitialized;

    public static string BasePath = string.Empty;
    public static string CacheName = string.Empty;

    public static uint Base;
    public static uint Head;

    public static DbObject? Manifest;

    private static readonly List<string> s_paths = new();

    private static readonly List<SuperBundleInfo> s_superBundles = new();
    private static readonly List<InstallChunkInfo> s_installChunks = new();
    private static readonly List<string> s_casFiles = new();
    
    private static Type? s_deobfuscator;
    private static readonly Dictionary<string, byte[]> s_memoryFs = new();

    public static bool Initialize(string basePath)
    {
        if (!Directory.Exists(basePath))
        {
            return false;
        }

        BasePath = basePath;
        if (!BasePath.EndsWith("\\") || !BasePath.EndsWith("/"))
        {
            BasePath = $"{BasePath}\\";
        }

        CacheName = $"Caches/{ProfilesLibrary.CacheName}";
        s_deobfuscator = Type.GetType($"Frosty.Sdk.Deobfuscators.{ProfilesLibrary.Deobfuscator}Deobfuscator");

        foreach (FileSystemSource source in ProfilesLibrary.Sources)
        {
            AddSource(source.Path, source.SubDirs);
        }

        if (!ProcessLayouts())
        {
            return false;
        }
        
        IsInitialized = true;
        return true;
    }

    public static string ResolvePath(string filename)
    {
        if (filename.StartsWith("native_patch/") && s_paths.Count == 1)
        {
            return string.Empty;
        }

        int startCount = 0;
        int endCount = s_paths.Count;

        if (filename.StartsWith("native_data/") && s_paths.Count > 1)
        {
            startCount = 1;
        }
        else if (filename.StartsWith("native_patch/"))
        {
            endCount = 1;
        }

        filename = filename.Replace("native_data/", string.Empty);
        filename = filename.Replace("native_patch/", string.Empty);
        filename = filename.Trim('/');

        for (int i = startCount; i < endCount; i++)
        {
            if (File.Exists($"{BasePath}{s_paths[i]}{filename}") || Directory.Exists($"{BasePath}{s_paths[i]}{filename}"))
            {
                return $"{BasePath}{s_paths[i]}{filename}".Replace("\\\\", "\\");
            }
        }
        return "";
    }

    public static string GetFilePath(ICasFileInfo casFileInfo)
    {
        InstallChunkInfo installChunkInfo = s_installChunks[casFileInfo.GetInstallChunkIndex()];
        return $"{(casFileInfo.GetIsPatch() ? "native_patch/" : "native_data/")}{installChunkInfo.InstallBundle}/cas_{casFileInfo.GetCasIndex():D2}.cas";
    }
    
    public static string GetFilePath(int casIndex)
    {
        return s_casFiles[casIndex];
    }

    public static IDeobfuscator? CreateDeobfuscator() => s_deobfuscator != null ? (IDeobfuscator?)Activator.CreateInstance(s_deobfuscator) : null;

    public static IEnumerable<SuperBundleInfo> EnumerateSuperBundles()
    {
        foreach (SuperBundleInfo sbInfo in s_superBundles)
        {
            yield return sbInfo;
        }
    }
    
    public static IEnumerable<InstallChunkInfo> EnumerateInstallChunks()
    {
        foreach (InstallChunkInfo installChunkInfo in s_installChunks)
        {
            yield return installChunkInfo;
        }
    }

    public static InstallChunkInfo GetInstallChunkInfo(int index)
    {
        return s_installChunks[index];
    }
    
    public static bool HasFileInMemoryFs(string name) => s_memoryFs.ContainsKey(name);
    public static byte[] GetFileFromMemoryFs(string name) => s_memoryFs[name];

    private static void AddSource(string path, bool iterateSubPaths)
    {
        if (!Directory.Exists($"{BasePath}{path}"))
        {
            return;
        }

        if (iterateSubPaths)
        {
            foreach (string subPath in Directory.EnumerateDirectories($"{BasePath}{path}", "*", SearchOption.AllDirectories))
            {
                if (subPath.Contains("\\Patch", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!File.Exists($"{subPath}\\package.mft"))
                {
                    continue;
                }

                s_paths.Add($"\\{subPath.Replace(BasePath, "")}\\Data\\");
            }
        }
        else
        {
            s_paths.Add($"\\{path}\\");
        }
    }
    
    private static bool LoadInitFs(string name)
    {
        string path = ResolvePath(name);
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        
        using (DataStream stream = new(new FileStream(path, FileMode.Open, FileAccess.Read), true))
        {
            DbObject? initFs = DbObject.Deserialize(stream);

            if (initFs == null)
            {
                return false;
            }

            if (initFs.GetDbType() == DbType.Object && initFs.ContainsKey("encrypted"))
            {
                if (!KeyManager.HasKey("InitFsKey"))
                {
                    return false;
                }
                byte[] key = KeyManager.GetKey("InitFsKey");
                byte[] buffer = initFs["encrypted"].As<byte[]>();
                byte[] decrypted = new byte[buffer.Length];

                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = key;

                ICryptoTransform transform = aes.CreateDecryptor();
                using CryptoStream cryptoStream = new(new MemoryStream(buffer), transform, CryptoStreamMode.Read);
                int bytesRead = cryptoStream.Read(decrypted, 0, buffer.Length);

                using (DataStream subStream = new(new MemoryStream(decrypted)))
                {
                    initFs = DbObject.Deserialize(subStream);
                }
            }

            foreach (DbObject fileStub in initFs)
            {
                DbObject file = (fileStub["$file"] as DbObject)!;
                string fileName = (file["name"] as string)!;

                if (!s_memoryFs.ContainsKey(fileName))
                {
                    s_memoryFs.Add(fileName, (file["payload"] as byte[])!);
                }
            }
        }
        return true;
    }
    
    private static bool ProcessLayouts()
    {
        string baseLayoutPath = ResolvePath("native_data/layout.toc");
        string patchLayoutPath = ResolvePath("native_patch/layout.toc");
        
        // Process base layout.toc
        DbObject? baseLayout;
        using (DataStream stream = new(new FileStream(baseLayoutPath, FileMode.Open, FileAccess.Read), true))
        {
            baseLayout = DbObject.Deserialize(stream);
        }

        if (baseLayout == null)
        {
            return false;
        }
        
        foreach (DbObject superBundle in baseLayout["superBundles"].As<DbObject>())
        {
            s_superBundles.Add(new SuperBundleInfo(superBundle["name"].As<string>()));
        }

        if (patchLayoutPath != "")
        {
            // Process patch layout.toc
            DbObject? patchLayout;
            using (DataStream stream = new(new FileStream(patchLayoutPath, FileMode.Open, FileAccess.Read), true))
            {
                patchLayout = DbObject.Deserialize(stream);
            }

            if (patchLayout == null)
            {
                return false;
            }
            
            foreach (DbObject superBundle in patchLayout["superBundles"].As<DbObject>())
            {
                // Merge super bundles
                string superBundleName = superBundle["name"].As<string>();
                if (s_superBundles.FindIndex(si => si.Name.Equals(superBundleName, StringComparison.OrdinalIgnoreCase)) == -1)
                {
                    s_superBundles.Add(new SuperBundleInfo(superBundleName));
                }
            }

            Base = patchLayout["base"].As<uint>();
            Head = patchLayout["head"].As<uint>();

            if (!ProcessInstallChunks(patchLayout))
            {
                return false;
            }

            if (patchLayout.ContainsKey("manifest"))
            {
                Manifest = patchLayout["manifest"].As<DbObject>();
            }
            
            if (!LoadInitFs(patchLayout["fs"].As<DbObject>()[0].As<string>()))
            {
                return false;
            }
        }
        else
        {
            if (baseLayout.ContainsKey("base"))
            {
                // TODO: is base only in patch ???
                Base = baseLayout["base"].As<uint>();
            }
            Head = baseLayout["head"].As<uint>();

            if (!ProcessInstallChunks(baseLayout))
            {
                return false;
            }

            if (baseLayout.ContainsKey("manifest"))
            {
                Manifest = baseLayout["manifest"].As<DbObject>();
            }
            
            if (!LoadInitFs(baseLayout["fs"].As<DbObject>()[0].As<string>()))
            {
                return false;
            }
        }
        return true;
    }

    private static bool ProcessInstallChunks(DbObject layout)
    {
        // Only if an install manifest exists
        if (layout.ContainsKey("installManifest"))
        {
            DbObject installManifest = layout["installManifest"].As<DbObject>();
            foreach (DbObject installChunk in installManifest["installChunks"].As<DbObject>())
            {
                if (installChunk.ContainsKey("testDLC") && installChunk["testDLC"].As<bool>())
                {
                    continue;
                }

                InstallChunkInfo info = new()
                {
                    Id = installChunk["id"].As<Guid>(),
                    Name = installChunk["name"].As<string>(),
                    InstallBundle = installChunk.ContainsKey("installBundle") ? installChunk["installBundle"].As<string>() : string.Empty,
                    AlwaysInstalled = installChunk.ContainsKey("alwaysInstalled") && installChunk["alwaysInstalled"].As<bool>()
                };
                    
                int index = installChunk.ContainsKey("persistentIndex") ? installChunk["persistentIndex"].As<int>() : s_installChunks.Count;
                while (s_installChunks.Count <= index)
                {
                    s_installChunks.Add(new InstallChunkInfo());
                }

                s_installChunks[index] = info;
                
                foreach (string superBundle in installChunk["superBundles"].As<DbObject>())
                {
                    info.SuperBundles.Add(superBundle);
                        
                    SuperBundleInfo sb = s_superBundles.Find(si => si.Name.Equals(superBundle, StringComparison.OrdinalIgnoreCase))!;
                    if (!sb.InstallChunks.ContainsKey(index))
                    {
                        sb.InstallChunks.Add(index, InstallChunkType.Default);
                    }
                    else
                    {
                        sb.InstallChunks[index] |= InstallChunkType.Default;
                    }
                }

                foreach (Guid requiredChunk in installChunk["requiredChunks"].As<DbObject>())
                {
                    info.RequiredCatalogs.Add(requiredChunk);
                }

                if (installChunk.ContainsKey("files"))
                {
                    foreach (DbObject fileObj in installChunk["files"].As<DbObject>())
                    {
                        int casId = fileObj["id"].As<int>();
                        while (s_casFiles.Count <= casId)
                        {
                            s_casFiles.Add("");
                        }

                        string casPath = fileObj["path"].As<string>().Trim('/');
                        casPath = casPath.Replace("native_data/Data", "native_data");
                        casPath = casPath.Replace("native_data/Patch", "native_patch");

                        s_casFiles[casId] = casPath;
                    }
                }

                if (installChunk.ContainsKey("splitSuperBundles"))
                {
                    foreach (DbObject superBundleContainer in installChunk["splitSuperBundles"].As<DbObject>())
                    {
                        string superBundle = superBundleContainer["superBundle"].As<string>();
                        info.SplitSuperBundles.Add(superBundle);

                        SuperBundleInfo sb = s_superBundles.Find(si => si.Name.Equals(superBundle, StringComparison.OrdinalIgnoreCase))!;
                        if (!sb.InstallChunks.ContainsKey(index))
                        {
                            sb.InstallChunks.Add(index, InstallChunkType.Split);
                        }
                        else
                        {
                            sb.InstallChunks[index] |= InstallChunkType.Split;
                        }
                    }
                }

                if (installChunk.ContainsKey("splitTocs"))
                {
                    throw new NotImplementedException("splitTocs");
                }
            }
        }
        else
        {
            // Older games dont have an InstallManifest, they have one InstallChunk in the data/patch folder
            InstallChunkInfo ci = new();
            foreach (SuperBundleInfo si in s_superBundles)
            {
                ci.SuperBundles.Add(si.Name);

                si.InstallChunks.Add(0, InstallChunkType.Default);
            }
            s_installChunks.Add(ci);
        }
        return true;
    }
}