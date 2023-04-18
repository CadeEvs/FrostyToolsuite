using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Loaders.Helpers;
using FileInfo = Frosty.Sdk.Managers.Infos.FileInfo;

namespace Frosty.Sdk.Managers.Loaders;

public class StandardAssetLoader : IAssetLoader
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
            DbObject? toc = DbObject.Deserialize(tocPath);
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

    private bool LoadToc(string sbName, int superBundleId, DbObject toc, ref List<BundleInfo> bundles, ref Dictionary<int, BundleInfo> baseBundleDic, bool isPatched)
    {
        // flag for if the assets are stored in cas files or in the superbundle directly
        bool isCas = toc.ContainsKey("cas") && toc["cas"].As<bool>();

        // process toc chunks
        if (toc.ContainsKey("chunks"))
        {
            string path = FileSystemManager.ResolvePath($"{(isPatched ? "native_patch" : "native_data")}/{sbName}.sb");

            foreach (DbObject chunk in toc["chunks"].As<DbObject>())
            {
                long size = chunk.ContainsKey("size") ? chunk["size"].As<long>() : -1;
                
                
                ChunkAssetEntry entry = new(chunk["id"].As<Guid>(),
                    chunk.ContainsKey("sha1") ? chunk["sha1"].As<Sha1>() : Sha1.Zero, size, 0, 0, superBundleId);

                if (size != -1)
                {
                    entry.Location = AssetDataLocation.CasNonIndexed;
                    entry.FileInfo = new FileInfo()
                    {
                        Path = path,
                        Offset = chunk["offset"].As<long>(),
                        Size = entry.Size
                    };
                }
                    
                AssetManager.AddSuperBundleChunk(entry);
            }
        }

        bool processBaseBundles = false;
        
        // process bundles
        if (toc.ContainsKey("bundles"))
        {
            foreach (DbObject bundleInfo in toc["bundles"].As<DbObject>())
            {
                string name = bundleInfo["id"].As<string>();
                
                bool isDelta = bundleInfo.ContainsKey("delta") && bundleInfo["delta"].As<bool>();
                bool isBase = bundleInfo.ContainsKey("base") && bundleInfo["base"].As<bool>();
                    
                long offset = bundleInfo["offset"].As<long>();
                long size = bundleInfo["size"].As<long>();
                
                bundles.Add(new BundleInfo()
                {
                    Name = name,
                    SbName = sbName,
                    Offset = offset,
                    Size = size,
                    IsDelta = isDelta,
                    IsPatch = isPatched && !isBase,
                    IsCas = isCas
                });

                if (isDelta)
                {
                    processBaseBundles = true;
                }
            }
        }

        if (processBaseBundles)
        {
            string tocPath = FileSystemManager.ResolvePath($"native_data/{sbName}.toc");
            DbObject? baseToc = DbObject.Deserialize(tocPath);
            if (baseToc == null)
            {
                return false;
            }
                
            isCas = baseToc.ContainsKey("cas") && toc["cas"].As<bool>();
                
            if (!baseToc.ContainsKey("bundles"))
            {
                return false;
            }
                
            foreach (DbObject bundleInfo in baseToc["bundles"].As<DbObject>())
            {
                string name = bundleInfo["id"].As<string>();
                    
                long offset = bundleInfo["offset"].As<long>();
                long size = bundleInfo["size"].As<long>();
                
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
        DataStream? patchStream = null;
        DataStream? baseStream = null;

        foreach (BundleInfo bundleInfo in bundles)
        {
            // get correct stream for this bundle
            DataStream stream;
            if (bundleInfo.IsPatch)
            {
                if (patchStream == null || patchSbPath != bundleInfo.SbName)
                {
                    patchSbPath = bundleInfo.SbName;
                    patchStream?.Dispose();
                    patchStream = new DataStream(new FileStream(
                        FileSystemManager.ResolvePath($"native_patch/{patchSbPath}.sb"), FileMode.Open,
                        FileAccess.Read), true);
                }
                stream = patchStream;
            }
            else
            {
                if (baseStream == null || baseSbPath != bundleInfo.SbName)
                {
                    baseSbPath = bundleInfo.SbName;
                    baseStream?.Dispose();
                    baseStream = new DataStream(new FileStream(
                        FileSystemManager.ResolvePath($"native_data/{baseSbPath}.sb"), FileMode.Open,
                        FileAccess.Read), true);
                }
                stream = baseStream;
            }
            
            DbObject bundle;
            
            // load bundle from sb file
            if (bundleInfo.IsCas)
            {
                stream.Position = bundleInfo.Offset;
                bundle = DbObject.Deserialize(stream)!;
                Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);
            }
            else
            {
                if (bundleInfo.IsDelta)
                {
                    int hash = Utils.Utils.HashString(bundleInfo.Name, true);
                    bool hasBase;
                    if (hasBase = baseBundleDic.TryGetValue(hash, out BundleInfo baseBundleInfo))
                    {
                        baseStream!.Position = baseBundleInfo.Offset;
                    }

                    stream.Position = bundleInfo.Offset;
                    
                    bundle = DeserializeDeltaBundle(baseStream, stream);
                    
                    // read data
                    foreach (DbObject ebx in bundle["ebx"].As<DbObject>())
                    {
                        long baseOffset = hasBase ? baseStream!.Position : -1;
                        long deltaOffset = stream.Position;
                        (long, long) size = GetSize(baseStream, stream, ebx["originalSize"].As<long>());
                        
                        ebx.AddValue("size", -1);
                    }
                    
                    foreach (DbObject res in bundle["res"].As<DbObject>())
                    {
                        long baseOffset = hasBase ? baseStream!.Position : -1;
                        long deltaOffset = stream.Position;
                        (long, long) size = GetSize(baseStream, stream, res["originalSize"].As<long>());
                        
                        res.AddValue("size", -1);
                    }
                    
                    foreach (DbObject chunk in bundle["chunks"].As<DbObject>())
                    {
                        long baseOffset = hasBase ? baseStream!.Position : -1;
                        long deltaOffset = stream.Position;
                        (long, long) size = GetSize(baseStream, stream, chunk["originalSize"].As<long>());
                        
                        chunk.AddValue("size", -1);
                    }
                    
                    Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);

                    if (baseBundleDic.TryGetValue(hash, out baseBundleInfo))
                    {
                        Debug.Assert(baseStream!.Position == baseBundleInfo.Offset + baseBundleInfo.Size);
                    }
                }
                else
                {
                    stream.Position = bundleInfo.Offset;
                    bundle = BinaryBundle.Deserialize(stream);
                    
                    string path = FileSystemManager.ResolvePath($"{(bundleInfo.IsPatch ? "native_patch" : "native_data")}/{bundleInfo.Name}.sb");

                    // read data
                    foreach (DbObject ebx in bundle["ebx"].As<DbObject>())
                    {
                        ebx.AddValue("path", path);
                        ebx.AddValue("offset", stream.Position);
                        ebx.AddValue("size", GetSize(stream, ebx["originalSize"].As<long>()));
                    }
                    
                    foreach (DbObject res in bundle["res"].As<DbObject>())
                    {
                        res.AddValue("path", path);
                        res.AddValue("offset", stream.Position);
                        res.AddValue("size", GetSize(stream, res["originalSize"].As<long>()));                    }
                    
                    foreach (DbObject chunk in bundle["chunks"].As<DbObject>())
                    {
                        chunk.AddValue("path", path);
                        chunk.AddValue("offset", stream.Position);
                        chunk.AddValue("size", GetSize(stream, chunk["originalSize"].As<long>()));
                    }
                    
                    Debug.Assert(stream.Position == bundleInfo.Offset + bundleInfo.Size);
                }
            }
            AssetManager.ProcessBundle(bundle, superBundleId, bundleInfo.Name);
        }
        
        patchStream?.Dispose();
        baseStream?.Dispose();
    }

    private DbObject DeserializeDeltaBundle(DataStream? baseStream, DataStream deltaStream)
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
            bufferSize = ((flags & 0x0F) << 0x10) + bufferSize;
        if ((decompressedSize & 0xFF000000) != 0)
            decompressedSize &= 0x00FFFFFF;

        originalSize -= decompressedSize;

        compressionType = (ushort)(compressionType & 0x7F);
        if (compressionType == 0x00)
            bufferSize = decompressedSize;

        size += bufferSize + 8;
        stream.Position += bufferSize;
    }
    
    private (long, long) GetSize(DataStream? baseStream, DataStream deltaStream, long originalSize)
    {
        // TODO: there can be multiple assets in one block, so probably easier to create a cache like before
        // or just store the base offset if its not changed, and delta if its changed
        // this might be hacky, but it only has to work for the pvz games
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