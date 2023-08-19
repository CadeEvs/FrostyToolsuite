using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Frosty.Sdk.DbObjectElements;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Infos.FileInfos;
using Frosty.Sdk.Managers.Loaders.Helpers;

namespace Frosty.Sdk.Managers.Loaders;

public class DbObjectAssetLoader : IAssetLoader
{
    public void Load()
    {
        foreach (SuperBundleInfo sbInfo in FileSystemManager.EnumerateSuperBundles())
        {
            bool isPatched = true;
            string tocPath = FileSystemManager.ResolvePath(true, $"{sbInfo.Name}.toc");
            if (string.IsNullOrEmpty(tocPath))
            {
                isPatched = false;
                tocPath = FileSystemManager.ResolvePath(false, $"{sbInfo.Name}.toc");
                if (string.IsNullOrEmpty(tocPath))
                {
                    continue;
                }
            }
            DbObjectDict? toc = DbObject.Deserialize(tocPath)?.AsDict();
            if (toc is null)
            {
                continue;
            }

            List<BundleInfo> bundles = new();
            Dictionary<int, BundleInfo> baseBundleDic = new();

            int superBundleId = AssetManager.AddSuperBundle(sbInfo.Name);

            if (!LoadToc(sbInfo.Name, superBundleId, toc, ref bundles, ref baseBundleDic, isPatched))
            {
                continue;
            }
            
            LoadSb(bundles, baseBundleDic, superBundleId);
        }
    }

    private bool LoadToc(string sbName, int superBundleId, DbObjectDict toc, ref List<BundleInfo> bundles, ref Dictionary<int, BundleInfo> baseBundleDic, bool isPatched)
    {
        // flag for if the assets are stored in cas files or in the superbundle directly
        bool isCas = toc.AsBoolean("cas");
        // flag for das files (used in NFS Edge)
        bool isDas = toc.AsBoolean("das");

        // process toc chunks
        if (toc.ContainsKey("chunks"))
        {
            string path = $"{(isPatched ? "native_patch" : "native_data")}/{sbName}.sb";

            foreach (DbObject chunkObj in toc.AsList("chunks"))
            {
                DbObjectDict chunk = chunkObj.AsDict();
                
                long size = isCas ? ResourceManager.GetSize(chunk.AsSha1("sha1")) : chunk.AsLong("size");

                ChunkAssetEntry entry = new(chunk.AsGuid("id"), chunk.AsSha1("sha1", Sha1.Zero), size, 0, 0,
                    superBundleId);

                if (!isCas)
                {
                    entry.FileInfos.Add(new PathFileInfo(path, chunk.AsUInt("offset"), (uint)size, 0));
                }
                else
                {
                    IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(entry.Sha1);
                    if (fileInfos is not null)
                    {
                        entry.FileInfos.UnionWith(fileInfos);   
                    }
                }
                
                AssetManager.AddSuperBundleChunk(entry);
            }
        }

        bool processBaseBundles = false;

        // process bundles
        if (toc.ContainsKey("bundles"))
        {
            // das TOC - stores bundles as a dict
            if (isDas)
            {
                DbObjectDict dasBundlesDict = toc.AsDict("bundles");

                DbObjectList dasBundleNames = dasBundlesDict.AsList("names");
                DbObjectList dasBundleOffsets = dasBundlesDict.AsList("offsets");
                DbObjectList dasBundleSizes = dasBundlesDict.AsList("sizes");

                for (int bundleIter = 0; bundleIter < dasBundleNames.Count; bundleIter++)
                {
                    string name = dasBundleNames[bundleIter].AsString();
                    int offset = dasBundleOffsets[bundleIter].AsInt();
                    int size = dasBundleSizes[bundleIter].AsInt();

                    bundles.Add(new BundleInfo()
                    {
                        Name = name,
                        SbName = sbName,
                        Offset = offset,
                        Size = size,
                        IsDelta = false,
                        IsPatch = isPatched
                    });
                }
            }
            // standard TOC, stores bundles as a list
            else
            {
                foreach (DbObject bundleInfo in toc.AsList("bundles"))
                {
                    DbObjectDict bundleInfoDict = bundleInfo.AsDict();

                    string name = bundleInfoDict.AsString("id");

                    bool isDelta = bundleInfoDict.AsBoolean("delta");
                    bool isBase = bundleInfoDict.AsBoolean("base");

                    long offset = bundleInfoDict.AsLong("offset");
                    long size = bundleInfoDict.AsLong("size");

                    bundles.Add(new BundleInfo()
                    {
                        Name = name,
                        SbName = sbName,
                        Offset = offset,
                        Size = size,
                        IsDelta = isDelta,
                        IsPatch = isPatched && !isBase,
                        IsNonCas = !isCas,
                    });

                    if (isDelta)
                    {
                        processBaseBundles = true;
                    }
                }
            }
        }

        if (processBaseBundles)
        {
            string tocPath = FileSystemManager.ResolvePath($"native_data/{sbName}.toc");
            DbObjectDict? baseToc = DbObject.Deserialize(tocPath)?.AsDict();
            if (baseToc == null)
            {
                return false;
            }

            isCas = baseToc.AsBoolean("cas");
                
            if (!baseToc.ContainsKey("bundles"))
            {
                return false;
            }
                
            foreach (DbObject bundleInfo in baseToc.AsList("bundles"))
            {
                string name = bundleInfo.AsDict().AsString("id");
                    
                long offset = bundleInfo.AsDict().AsLong("offset");
                long size = bundleInfo.AsDict().AsLong("size");
                
                baseBundleDic.Add(Utils.Utils.HashString(name, true), new BundleInfo()
                {
                    Name = name,
                    SbName = sbName,
                    Offset = offset,
                    Size = size,
                    IsPatch = false,
                    IsNonCas = !isCas,
                });
            }
        }
        
        return true;
    }

    private void LoadSb(List<BundleInfo> bundles, Dictionary<int, BundleInfo> baseBundleDic, int superBundleId)
    {
        string patchSbPath = string.Empty;
        string baseSbPath = string.Empty;
        BlockStream? patchStream = null;
        BlockStream? baseStream = null;

        foreach (BundleInfo bundleInfo in bundles)
        {
            // get correct stream for this bundle
            BlockStream stream;
            if (bundleInfo.IsPatch)
            {
                if (patchStream == null || patchSbPath != bundleInfo.SbName)
                {
                    patchSbPath = bundleInfo.SbName;
                    patchStream?.Dispose();
                    patchStream = BlockStream.FromFile(
                        FileSystemManager.ResolvePath(true, $"{patchSbPath}.sb"), false);
                }
                stream = patchStream;
            }
            else
            {
                if (baseStream == null || baseSbPath != bundleInfo.SbName)
                {
                    baseSbPath = bundleInfo.SbName;
                    baseStream?.Dispose();
                    baseStream = BlockStream.FromFile(
                        FileSystemManager.ResolvePath(false, $"{baseSbPath}.sb"), false);
                }
                stream = baseStream;
            }
            
            int bundleId = AssetManager.AddBundle(bundleInfo.Name, superBundleId);

            // load bundle from sb file
            if (bundleInfo.IsNonCas)
            {
                if (bundleInfo.IsDelta)
                {
                    int hash = Utils.Utils.HashString(bundleInfo.Name, true);
                    bool hasBase = baseBundleDic.TryGetValue(hash, out BundleInfo baseBundleInfo);
                    if (hasBase)
                    {
                        if (baseStream == null || baseSbPath != bundleInfo.SbName)
                        {
                            baseSbPath = bundleInfo.SbName;
                            baseStream?.Dispose();
                            baseStream = BlockStream.FromFile(
                                FileSystemManager.ResolvePath(false, $"{baseSbPath}.sb"), false);
                        }
                        baseStream!.Position = baseBundleInfo.Offset;
                    }

                    stream.Position = bundleInfo.Offset;

                    BinaryBundle bundle = DeserializeDeltaBundle(baseStream, stream);

                    // TODO: get asset refs from sb file similar to this (https://github.com/GreyDynamics/Frostbite3_Editor/blob/develop/src/tk/greydynamics/Resource/Frostbite3/Cas/NonCasBundle.java)
                    // or with a cache like before
                    // this is just so u can load those games for now
                    foreach (EbxAssetEntry ebx in bundle.EbxList)
                    {
                        AssetManager.AddEbx(ebx, bundleId);
                    }

                    foreach (ResAssetEntry res in bundle.ResList)
                    {
                        AssetManager.AddRes(res, bundleId);
                    }

                    foreach (ChunkAssetEntry chunk in bundle.ChunkList)
                    {
                        AssetManager.AddChunk(chunk, bundleId);
                    }
                }
                else
                {
                    stream.Position = bundleInfo.Offset;
                    BinaryBundle bundle = BinaryBundle.Deserialize(stream);

                    string path = $"{(bundleInfo.IsPatch ? "native_patch" : "native_data")}/{bundleInfo.Name}.sb";

                    // read data
                    foreach (EbxAssetEntry ebx in bundle.EbxList)
                    {
                        uint offset = (uint)stream.Position;
                        ebx.Size = GetSize(stream, ebx.OriginalSize);
                        ebx.FileInfos.Add(new PathFileInfo(path, offset, (uint)ebx.Size, 0));

                        AssetManager.AddEbx(ebx, bundleId);
                    }

                    foreach (ResAssetEntry res in bundle.ResList)
                    {
                        uint offset = (uint)stream.Position;
                        res.Size = GetSize(stream, res.OriginalSize);
                        res.FileInfos.Add(new PathFileInfo(path, offset, (uint)res.Size, 0));

                        AssetManager.AddRes(res, bundleId);
                    }

                    foreach (ChunkAssetEntry chunk in bundle.ChunkList)
                    {
                        uint offset = (uint)stream.Position;
                        chunk.Size = GetSize(stream, (chunk.LogicalOffset & 0xFFFF) | chunk.LogicalSize);
                        chunk.FileInfos.Add(new PathFileInfo(path, offset, (uint)chunk.Size, chunk.LogicalOffset));

                        AssetManager.AddChunk(chunk, bundleId);
                    }

                    Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);
                }
            }
            else
            {
                stream.Position = bundleInfo.Offset;
                DbObjectDict bundle = DbObject.Deserialize(stream)!.AsDict();
                Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);

                DbObjectList? ebxList = bundle.AsList("ebx", null);
                DbObjectList? resList = bundle.AsList("res", null);
                DbObjectList? chunkList = bundle.AsList("chunks", null);

                for (int i = 0; i < ebxList?.Count; i++)
                {
                    DbObjectDict ebx = ebxList[i].AsDict();

                    EbxAssetEntry entry = new(ebx.AsString("name"), ebx.AsSha1("sha1"), ebx.AsLong("size"),
                        ebx.AsLong("originalSize"));

                    if (ebx.AsInt("casPatchType") == 2)
                    {
                        Sha1 baseSha1 = ebx.AsSha1("baseSha1");
                        Sha1 deltaSha1 = ebx.AsSha1("deltaSha1");

                        IEnumerable<IFileInfo>? fileInfos =
                            ResourceManager.GetPatchFileInfos(entry.Sha1, deltaSha1, baseSha1);

                        if (fileInfos is not null)
                        {
                            entry.FileInfos.UnionWith(fileInfos);
                        }
                    }
                    else
                    {
                        IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(entry.Sha1);
                        if (fileInfos is not null)
                        {
                            entry.FileInfos.UnionWith(fileInfos);
                        }
                    }

                    AssetManager.AddEbx(entry, bundleId);
                }

                for (int i = 0; i < resList?.Count; i++)
                {
                    DbObjectDict res = resList[i].AsDict();

                    ResAssetEntry entry = new(res.AsString("name"), res.AsSha1("sha1"), res.AsLong("size"),
                        res.AsLong("originalSize"), res.AsULong("resRid"), res.AsUInt("resType"),
                        res.AsBlob("resMeta"));

                    if (res.AsInt("casPatchType") == 2)
                    {
                        Sha1 baseSha1 = res.AsSha1("baseSha1");
                        Sha1 deltaSha1 = res.AsSha1("deltaSha1");

                        IEnumerable<IFileInfo>? fileInfos =
                            ResourceManager.GetPatchFileInfos(entry.Sha1, deltaSha1, baseSha1);

                        if (fileInfos is not null)
                        {
                            entry.FileInfos.UnionWith(fileInfos);
                        }
                    }
                    else
                    {
                        IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(entry.Sha1);
                        if (fileInfos is not null)
                        {
                            entry.FileInfos.UnionWith(fileInfos);
                        }
                    }

                    AssetManager.AddRes(entry, bundleId);
                }

                for (int i = 0; i < chunkList?.Count; i++)
                {
                    DbObjectDict chunk = chunkList[i].AsDict();

                    ChunkAssetEntry entry = new(chunk.AsGuid("id"), chunk.AsSha1("sha1"), chunk.AsLong("size"),
                        chunk.AsUInt("logicalOffset"), chunk.AsUInt("logicalSize"));

                    if (chunk.AsInt("casPatchType") == 2)
                    {
                        Sha1 baseSha1 = chunk.AsSha1("baseSha1");
                        Sha1 deltaSha1 = chunk.AsSha1("deltaSha1");

                        IEnumerable<IFileInfo>? fileInfos =
                            ResourceManager.GetPatchFileInfos(entry.Sha1, deltaSha1, baseSha1);

                        if (fileInfos is not null)
                        {
                            entry.FileInfos.UnionWith(fileInfos);
                        }
                    }
                    else
                    {
                        IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(entry.Sha1);
                        if (fileInfos is not null)
                        {
                            entry.FileInfos.UnionWith(fileInfos);
                        }
                    }

                    AssetManager.AddChunk(entry, bundleId);
                }
            }
        }
        
        patchStream?.Dispose();
        baseStream?.Dispose();
    }

    private BinaryBundle DeserializeDeltaBundle(DataStream? baseStream, DataStream deltaStream)
    {
        ulong magic = deltaStream.ReadUInt64();
        if (magic != 0x0000000001000000)
        {
            throw new InvalidDataException();
        }

        uint bundleSize = deltaStream.ReadUInt32(Endian.Big);
        deltaStream.ReadUInt32(Endian.Big); // size of data after binary bundle
            
        long startOffset = deltaStream.Position;

        int patchedBundleSize = deltaStream.ReadInt32(Endian.Big);
        uint baseBundleSize = baseStream?.ReadUInt32(Endian.Big) ?? 0;
        long baseBundleOffset = baseStream?.Position ?? -1;
        
        using (BlockStream stream = new(patchedBundleSize + 4))
        {
            stream.WriteInt32(patchedBundleSize, Endian.Big);
            
            while (deltaStream.Position < bundleSize + startOffset)
            {
                uint packed = deltaStream.ReadUInt32(Endian.Big);
                uint instructionType = (packed & 0xF0000000) >> 28;
                int blockData = (int)(packed & 0x0FFFFFFF);

                switch (instructionType)
                {
                    // read base block
                    case 0:
                        stream.Write(baseStream!.ReadBytes(blockData), 0, blockData);
                        break;
                    // skip base block
                    case 4:
                        baseStream!.Position += blockData;
                        break;
                    // read delta block
                    case 8:
                        stream.Write(deltaStream.ReadBytes(blockData), 0, blockData);
                        break;
                }
            }

            if (baseStream is not null)
            {
                baseStream.Position = baseBundleOffset + baseBundleSize;
            }
            
            stream.Position = 0;
            return BinaryBundle.Deserialize(stream);
        }
    }

    public static long GetSize(DataStream stream, long originalSize)
    {
        long size = 0;
        while (originalSize > 0)
        {
            ReadBlock(stream, ref originalSize, ref size);
        }

        return size;
    }
    
    private static void ReadBlock(DataStream stream, ref long originalSize, ref long size)
    {
        ulong packed = stream.ReadUInt64(Endian.Big);

        int decompressedSize = (int)((packed >> 32) & 0x00FFFFFF);
        byte compressionType = (byte)((packed >> 24) & 0x7F);
        int bufferSize = (int)(packed & 0x000FFFFF);

        originalSize -= decompressedSize;

        if (compressionType == 0)
        {
            bufferSize = decompressedSize;
        }

        size += bufferSize + 8;
        stream.Position += bufferSize;
    }
}