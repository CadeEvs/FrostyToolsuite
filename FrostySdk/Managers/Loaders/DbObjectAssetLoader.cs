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
            string tocPath = FileSystemManager.ResolvePath($"native_patch/{sbInfo.Name}.toc");
            if (string.IsNullOrEmpty(tocPath))
            {
                isPatched = false;
                tocPath = FileSystemManager.ResolvePath($"native_data/{sbInfo.Name}.toc");
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
        {
            if (toc.ContainsKey("bundles"))
            {
                // das TOC - stores bundles as a dict
                if(isDas)
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
                            IsPatch = false, // Edge has no patch or update folder
                            IsCas = isCas,
                            IsDas = isDas
                        });
                    }
                }
                // not a das, bundles is a list
                else if(isCas && !isDas)
                {
                    foreach (DbObject bundleInfo in toc.AsList("bundles"))
                    {
                        string name = bundleInfo.AsDict().AsString("id");

                        bool isDelta = bundleInfo.AsDict().AsBoolean("delta");
                        bool isBase = bundleInfo.AsDict().AsBoolean("base");

                        long offset = bundleInfo.AsDict().AsLong("offset");
                        long size = bundleInfo.AsDict().AsLong("size");

                        bundles.Add(new BundleInfo()
                        {
                            Name = name,
                            SbName = sbName,
                            Offset = offset,
                            Size = size,
                            IsDelta = isDelta,
                            IsPatch = isPatched && !isBase,
                            IsCas = isCas,
                            IsDas = isDas
                        });

                        if (isDelta)
                        {
                            processBaseBundles = true;
                        }
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
                    IsCas = isCas,
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
                        FileSystemManager.ResolvePath($"native_patch/{patchSbPath}.sb"), false);
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
                        FileSystemManager.ResolvePath($"native_data/{baseSbPath}.sb"), false);
                }
                stream = baseStream;
            }
            
            int bundleId = AssetManager.AddBundle(bundleInfo.Name, superBundleId);

            // load bundle from sb file
            if (bundleInfo.IsCas)
            {
                stream.Position = bundleInfo.Offset;
                DbObjectDict bundle = DbObject.Deserialize(stream)!.AsDict();
                Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);

                DbObjectList? ebxList = bundle.AsList("ebx", null);
                DbObjectList? resList = bundle.AsList("res", null);
                DbObjectList? chunkList = bundle.AsList("chunks", null);
                
                for(int i = 0; i < ebxList?.Count; i++)
                {
                    DbObjectDict ebx = ebxList[i].AsDict();

                    EbxAssetEntry entry = new(ebx.AsString("name"), ebx.AsSha1("sha1"), ebx.AsLong("size"),
                        ebx.AsLong("originalSize"));
                    
                    if (ebx.AsInt("casPatchType") == 2)
                    {
                        Sha1 baseSha1 = ebx.AsSha1("baseSha1");
                        Sha1 deltaSha1 = ebx.AsSha1("deltaSha1");

                        IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetPatchFileInfos(entry.Sha1, deltaSha1, baseSha1);

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
                
                for(int i = 0; i < resList?.Count; i++)
                {
                    DbObjectDict res = resList[i].AsDict();

                    ResAssetEntry entry = new(res.AsString("name"), res.AsSha1("sha1"), res.AsLong("size"),
                        res.AsLong("originalSize"), res.AsULong("resRid"), res.AsUInt("resType"),
                        res.AsBlob("resMeta"));
                    
                    if (res.AsInt("casPatchType") == 2)
                    {
                        Sha1 baseSha1 = res.AsSha1("baseSha1");
                        Sha1 deltaSha1 = res.AsSha1("deltaSha1");

                        IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetPatchFileInfos(entry.Sha1, deltaSha1, baseSha1);

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
                
                for(int i = 0; i < chunkList?.Count; i++)
                {
                    DbObjectDict chunk = chunkList[i].AsDict();

                    ChunkAssetEntry entry = new(chunk.AsGuid("id"), chunk.AsSha1("sha1"), chunk.AsLong("size"),
                        chunk.AsUInt("logicalOffset"), chunk.AsUInt("logicalSize"));
                    
                    if (chunk.AsInt("casPatchType") == 2)
                    {
                        Sha1 baseSha1 = chunk.AsSha1("baseSha1");
                        Sha1 deltaSha1 = chunk.AsSha1("deltaSha1");

                        IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetPatchFileInfos(entry.Sha1, deltaSha1, baseSha1);

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
            else
            {
                if (bundleInfo.IsDelta)
                {
                    int hash = Utils.Utils.HashString(bundleInfo.Name, true);
                    bool hasBase = baseBundleDic.TryGetValue(hash, out BundleInfo baseBundleInfo);
                    if (hasBase)
                    {
                        baseStream!.Position = baseBundleInfo.Offset;
                    }

                    stream.Position = bundleInfo.Offset;
                    
                    throw new NotImplementedException("delta sb storing");
                    
                    BinaryBundle bundle = DeserializeDeltaBundle(baseStream, stream);

                    
                    Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);

                    if (hasBase)
                    {
                        Debug.Assert(baseStream!.Position == baseBundleInfo.Offset + baseBundleInfo.Size);
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
        }
        
        patchStream?.Dispose();
        baseStream?.Dispose();
    }

    private BinaryBundle DeserializeDeltaBundle(DataStream? baseStream, DataStream deltaStream)
    {
        using (DataStream stream = new(new MemoryStream()))
        {
            // maybe magic always 0x0000000000000001
            deltaStream.Position += 8;

            uint bundleSize = deltaStream.ReadUInt32(Endian.Big);
            uint dataSize = deltaStream.ReadUInt32(Endian.Big);
            
            long startOffset = deltaStream.Position;
            
            stream.WriteUInt32(deltaStream.ReadUInt32(Endian.Big), Endian.Big);

            baseStream?.ReadUInt32(Endian.Big);
            
            while (deltaStream.Position < bundleSize + startOffset)
            {
                uint tmpVal = deltaStream.ReadUInt32(Endian.Big);
                uint blockType = (tmpVal & 0xFF000000) >> 24;
                int blockData = (int)(tmpVal & 0x00FFFFFF);

                switch (blockType)
                {
                    // read base block
                    case 0x00:
                        stream.Write(baseStream!.ReadBytes(blockData), 0, blockData);
                        break;
                    // skip base block
                    case 0x40:
                        baseStream!.Position += blockData;
                        break;
                    // read delta block
                    case 0x80:
                        stream.Write(deltaStream.ReadBytes(blockData), 0, blockData);
                        break;
                }
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
        int decompressedSize = stream.ReadInt32(Endian.Big);
        ushort compressionType = stream.ReadUInt16();
        int bufferSize = stream.ReadUInt16(Endian.Big);

        int flags = ((compressionType & 0xFF00) >> 8);

        if ((flags & 0x0F) != 0)
        {
            bufferSize = ((flags & 0x0F) << 0x10) + bufferSize;
        }

        if ((decompressedSize & 0xFF000000) != 0)
        {
            decompressedSize &= 0x00FFFFFF;
        }

        originalSize -= decompressedSize;

        compressionType = (ushort)(compressionType & 0x7F);
        if (compressionType == 0x00)
        {
            bufferSize = decompressedSize;
        }

        size += bufferSize + 8;
        stream.Position += bufferSize;
    }
    
    private (long, long) GetSize(DataStream? baseStream, DataStream deltaStream, long originalSize)
    {
        // TODO: there can be multiple assets in one block, so probably easier to create a cache like before
        // or just store the base offset if its not changed, and delta if its changed
        // this might be hacky, but it only has to work for the pvz games and bf4
        long baseSize = 0;
        long deltaSize = 0;
        while (originalSize > 0)
        {
            uint tmpVal = deltaStream.ReadUInt32(Endian.Big);
            int blockType = (int)(tmpVal & 0xF0000000) >> 28;
            deltaSize += sizeof(uint);
            
            if (blockType == 0x00) // Read base blocks
            {
                int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                while (blockCount-- > 0)
                {
                    ReadBlock(baseStream!, ref originalSize, ref baseSize);
                }
            }
            else if (blockType == 0x01) // Merge base w/delta
            {
                int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                long baseOriginalSize = 0;
                ReadBlock(baseStream!, ref baseOriginalSize, ref baseSize);
                baseOriginalSize *= -1;

                long pos = 0;
                
                while (blockCount-- > 0)
                {
                    int offset = deltaStream.ReadUInt16(Endian.Big);
                    int skipCount = deltaStream.ReadUInt16(Endian.Big);
                    deltaSize += 2 * sizeof(ushort);

                    int numBytes = (int)(offset - pos);
                    originalSize -= numBytes;
                    pos += numBytes;

                    long discard = 0;
                    ReadBlock(deltaStream, ref discard, ref deltaSize);

                    pos += skipCount;
                }

                originalSize -= baseOriginalSize - pos;
            }
            else if (blockType == 0x02) // Merge base w/delta
            {
                long discard = 0;
                ReadBlock(baseStream!, ref discard, ref baseSize);
                int deltaBlockSize = (int)(tmpVal & 0x0FFFFFFF);
                int newBlockSize = deltaStream.ReadUInt16(Endian.Big) + 1;

                originalSize -= newBlockSize;
                deltaSize += sizeof(ushort);
                
                long startPos = deltaStream.Position;

                while (deltaStream.Position - startPos < deltaBlockSize)
                {
                    ushort offset = deltaStream.ReadUInt16(Endian.Big);
                    int skipCount = deltaStream.ReadByte();
                    int addCount = deltaStream.ReadByte();
                    deltaSize += sizeof(ushort) + 2 * sizeof(byte);
                }
            }
            else if (blockType == 0x03) // Read delta blocks
            {
                int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                while (blockCount-- > 0)
                {
                    ReadBlock(deltaStream, ref originalSize, ref deltaSize);
                }
            }
            else if (blockType == 0x04) // Skip blocks
            {
                int blockCount = (int)(tmpVal & 0x0FFFFFFF);
                while (blockCount-- > 0)
                {
                    long discard = 0;
                    ReadBlock(baseStream!, ref discard, ref baseSize);
                }
            }
        }

        return (deltaSize, baseSize);
    }

}