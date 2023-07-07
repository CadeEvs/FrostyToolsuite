using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Frosty.Sdk.DbObjectElements;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers;

public static class FileSystemManager
{
    public static bool IsInitialized { get; private set; }

    public static string BasePath = string.Empty;
    public static string CacheName = string.Empty;

    public static uint Base;
    public static uint Head;

    public static BundleFormat BundleFormat;

    public static DbObjectDict? Manifest;

    private static readonly List<string> s_paths = new();

    private static readonly List<SuperBundleInfo> s_superBundles = new();
    private static readonly List<InstallChunkInfo> s_installChunks = new();
    private static readonly List<string> s_casFiles = new();
    
    private static Type? s_deobfuscator;
    private static readonly Dictionary<string, byte[]> s_memoryFs = new();

    public static bool Initialize(string basePath)
    {
        if (IsInitialized)
        {
            return true;
        }

        if (!ProfilesLibrary.IsInitialized)
        {
            return false;
        }

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

    /// <summary>
    /// Resolves the path of a file inside the games data directories.
    /// </summary>
    /// <param name="filename">
    /// <para>The relative path of the file prefixed with native_data or native_patch.</para>
    /// If there is no prefix it will look through all data directories starting with the patch ones.
    /// </param>
    /// <returns>The full path to the file or an empty string if the file doesnt exist.</returns>
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
        return string.Empty;
    }

    public static string GetFilePath(CasFileIdentifier casFileIdentifier)
    {
        InstallChunkInfo installChunkInfo = s_installChunks[casFileIdentifier.InstallChunkIndex];
        return $"{(casFileIdentifier.IsPatch ? "native_patch/" : "native_data/")}{installChunkInfo.InstallBundle}/cas_{casFileIdentifier.CasIndex:D2}.cas";
    }
    
    public static string GetFilePath(int casIndex)
    {
        return s_casFiles[casIndex];
    }

    public static string GetPatchPath()
    {
        return s_paths[0];
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
    
    public static InstallChunkInfo GetInstallChunkInfo(Guid id)
    {
        return s_installChunks.FirstOrDefault(ic => ic.Id == id) ?? throw new KeyNotFoundException();
    }

    public static int GetInstallChunkIndex(InstallChunkInfo info)
    {
        return s_installChunks.IndexOf(info);
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
        
        DbObject? initFs = DbObject.Deserialize(path);

        if (initFs is null)
        {
            return false;
        }

        if (initFs.IsDict())
        {
            if (!initFs.AsDict().ContainsKey("encrypted"))
            {
                return false;
            }
            
            if (!KeyManager.HasKey("InitFsKey"))
            {
                return false;
            }
            
            using (BlockStream stream = new(new Block<byte>(initFs.AsDict().AsBlob("encrypted"))))
            {
                stream.Decrypt(KeyManager.GetKey("InitFsKey"), PaddingMode.PKCS7);
                
                initFs = DbObject.Deserialize(stream);

                if (initFs is null)
                {
                    return false;
                }
            }
        }

        foreach (DbObject fileStub in initFs.AsList())
        {
            DbObjectDict file = fileStub.AsDict().AsDict("$file");
            string fileName = file.AsString("name");

            s_memoryFs.TryAdd(fileName, file.AsBlob("payload"));
        }
        return true;
    }
    
    private static bool ProcessLayouts()
    {
        string baseLayoutPath = ResolvePath("native_data/layout.toc");
        string patchLayoutPath = ResolvePath("native_patch/layout.toc");
        
        // Process base layout.toc
        DbObjectDict? baseLayout = DbObject.Deserialize(baseLayoutPath)?.AsDict();

        if (baseLayout is null)
        {
            return false;
        }

        string kelvinPath = ResolvePath("kelvin.toc");
        if (!string.IsNullOrEmpty(kelvinPath))
        {
            BundleFormat = BundleFormat.Kelvin;
        }
        
        foreach (DbObject superBundle in baseLayout.AsDict().AsList("superBundles"))
        {
            s_superBundles.Add(new SuperBundleInfo(superBundle.AsDict().AsString("name")));
        }

        if (patchLayoutPath != "")
        {
            // Process patch layout.toc
            DbObjectDict? patchLayout = DbObject.Deserialize(patchLayoutPath)?.AsDict();

            if (patchLayout is null)
            {
                return false;
            }
            
            foreach (DbObject superBundle in patchLayout.AsList("superBundles"))
            {
                // Merge super bundles
                string superBundleName = superBundle.AsDict().AsString("name");
                if (s_superBundles.FindIndex(si => si.Name.Equals(superBundleName, StringComparison.OrdinalIgnoreCase)) == -1)
                {
                    s_superBundles.Add(new SuperBundleInfo(superBundleName));
                }
            }

            Base = patchLayout.AsUInt("base");
            Head = patchLayout.AsUInt("head");

            if (!ProcessInstallChunks(patchLayout.AsDict("installManifest", null)))
            {
                return false;
            }

            Manifest = patchLayout.AsDict("manifest", null);
            if (Manifest is not null)
            {
                BundleFormat = BundleFormat.SuperBundleManifest;
            }
            
            if (!LoadInitFs(patchLayout.AsList("fs")[0].AsString()))
            {
                return false;
            }
        }
        else
        {
            Base = baseLayout.AsUInt("base");
            Head = baseLayout.AsUInt("head");

            if (!ProcessInstallChunks(baseLayout.AsDict("installManifest", null)))
            {
                return false;
            }


            Manifest = baseLayout.AsDict("manifest", null);
            if (Manifest is not null)
            {
                BundleFormat = BundleFormat.SuperBundleManifest;
            }
            
            if (!LoadInitFs(baseLayout.AsList("fs")[0].AsString()))
            {
                return false;
            }
        }
        return true;
    }

    private static bool ProcessInstallChunks(DbObjectDict? installManifest)
    {
        // Only if an install manifest exists
        if (installManifest is null)
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
        else
        {
            foreach (DbObject installChunk in installManifest.AsList("installChunks"))
            {
                if (installChunk.AsDict().AsBoolean("testDLC"))
                {
                    continue;
                }

                InstallChunkInfo info = new()
                {
                    Id = installChunk.AsDict().AsGuid("id"),
                    Name = installChunk.AsDict().AsString("name"),
                    InstallBundle = installChunk.AsDict().AsString("installBundle"),
                    AlwaysInstalled = installChunk.AsDict().AsBoolean("alwaysInstalled")
                };

                int index = installChunk.AsDict().AsInt("persistentIndex", s_installChunks.Count);
                while (s_installChunks.Count <= index)
                {
                    s_installChunks.Add(new InstallChunkInfo());
                }

                s_installChunks[index] = info;

                foreach (DbObject superBundle in installChunk.AsDict().AsList("superbundles"))
                {
                    info.SuperBundles.Add(superBundle.AsString());

                    SuperBundleInfo sb = s_superBundles.Find(si =>
                        si.Name.Equals(superBundle.AsString(), StringComparison.OrdinalIgnoreCase))!;
                    if (!sb.InstallChunks.ContainsKey(index))
                    {
                        sb.InstallChunks.Add(index, InstallChunkType.Default);
                    }
                    else
                    {
                        sb.InstallChunks[index] |= InstallChunkType.Default;
                    }
                }

                foreach (DbObject requiredChunk in installChunk.AsDict().AsList("requiredChunks"))
                {
                    info.RequiredCatalogs.Add(requiredChunk.AsGuid());
                }

                if (installChunk.AsDict().ContainsKey("files"))
                {
                    foreach (DbObject fileObj in installChunk.AsDict().AsList("files"))
                    {
                        int casId = fileObj.AsDict().AsInt("id");
                        while (s_casFiles.Count <= casId)
                        {
                            s_casFiles.Add(string.Empty);
                        }

                        string casPath = fileObj.AsDict().AsString("path").Trim('/');
                        casPath = casPath.Replace("native_data/Data", "native_data");
                        casPath = casPath.Replace("native_data/Patch", "native_patch");

                        s_casFiles[casId] = casPath;
                    }
                }

                if (installChunk.AsDict().ContainsKey("splitSuperbundles"))
                {
                    foreach (DbObject superBundleContainer in installChunk.AsDict().AsList("splitSuperbundles"))
                    {
                        string superBundle = superBundleContainer.AsDict().AsString("superbundle");
                        info.SplitSuperBundles.Add(superBundle);

                        SuperBundleInfo sb = s_superBundles.Find(si =>
                            si.Name.Equals(superBundle, StringComparison.OrdinalIgnoreCase))!;
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

                if (installChunk.AsDict().ContainsKey("splitTocs"))
                {
                    throw new NotImplementedException("splitTocs");
                }
            }

            if (installManifest.ContainsKey("settings"))
            {
                BundleFormat = (BundleFormat)installManifest.AsDict("settings").AsLong("bundleFormat");
            }
        }

        return true;
    }
}