using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using FileInfo = Frosty.Sdk.Managers.Infos.FileInfo;

namespace Frosty.Sdk.Managers.Loaders;

public class ManifestAssetLoader : IAssetLoader
{
    public void Load()
    {
        // This format has all SuperBundles stripped
        // all of the bundles and chunks of all SuperBundles are put into the manifest
        // afaik u cant reconstruct the SuperBundles, so this might make things a bit ugly
        
        AssetManager.AddSuperBundle("Manifest");
        
        DbObject manifest = FileSystemManager.Manifest!;

        CasMergedFileInfo file = manifest["file"].As<uint>();

        string path = FileSystemManager.GetFilePath(file);

        using (DataStream stream = new(new FileStream(FileSystemManager.ResolvePath(path), FileMode.Open, FileAccess.Read)))
        {
            stream.Position = manifest["offset"].As<long>();

            uint resourceInfoCount = stream.ReadUInt32();
            uint bundleCount = stream.ReadUInt32();
            uint chunkCount = stream.ReadUInt32();

            CasMergedResourceInfo[] resourceInfos = new CasMergedResourceInfo[resourceInfoCount];

            // resource infos
            for (int i = 0; i < resourceInfoCount; i++)
            {
                resourceInfos[i] = new CasMergedResourceInfo(stream.ReadUInt32(), stream.ReadUInt32(),
                    stream.ReadInt64());
            }
            
            // bundles
            for (int i = 0; i < bundleCount; i++)
            {
                int nameHash = stream.ReadInt32();
                int startIndex = stream.ReadInt32();
                int resourceCount = stream.ReadInt32();

                // unknown, always 0
                stream.ReadInt32();
                stream.ReadInt32();

                // first info is always the binary bundle
                CasMergedResourceInfo bundleInfo = resourceInfos[startIndex];

                DbObject bundle;
                
                string bundlePath = FileSystemManager.GetFilePath(bundleInfo.CasMergedFileInfo);
                using (DataStream casStream = new(new FileStream(FileSystemManager.ResolvePath(bundlePath), FileMode.Open, FileAccess.Read)))
                {
                    casStream.Position = bundleInfo.Offset;
                    bundle = BinaryBundle.Deserialize(casStream);
                    
                    // TODO: could theoretically also contain some more data if i understand these resource infos correctly
                    Debug.Assert(casStream.Position == bundleInfo.Offset + bundleInfo.Size);
                }

                // TODO: maybe parse merged resource infos, since the game uses those and not the catalog lookups
                foreach (DbObject ebx in bundle["ebx"].As<DbObject>())
                {
                    ebx.AddValue("size", -1);
                }
                
                foreach (DbObject res in bundle["res"].As<DbObject>())
                {
                    res.AddValue("size", -1);
                }
                
                foreach (DbObject chunk in bundle["chunks"].As<DbObject>())
                {
                    chunk.AddValue("size", -1);
                }
                
                if (!ProfilesLibrary.SharedBundles.TryGetValue(nameHash, out string? name))
                {
                    // we get the name while processing the ebx, since blueprint/sublevel bundles always have an ebx asset with the same name
                    name = nameHash.ToString("x8");
                }
                AssetManager.ProcessBundle(bundle, 0, name);
            }
            
            // chunks
            for (int i = 0; i < chunkCount; i++)
            {
                Guid chunkId = stream.ReadGuid();
                CasMergedResourceInfo resourceInfo = resourceInfos[stream.ReadInt32()];

                ChunkAssetEntry entry = new(chunkId, Sha1.Zero, resourceInfo.Size, 0, 0, 0)
                {
                    Location = AssetDataLocation.CasNonIndexed,
                    FileInfo = new FileInfo(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.CasMergedFileInfo)), resourceInfo.Offset, resourceInfo.Size)
                };

                AssetManager.AddSuperBundleChunk(entry);
            }
        }
    }
    
    private long ReadCas(DataStream stream, int originalSize)
    {
        long size = 0;
        while (originalSize > 0)
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
        return size;
    }
}