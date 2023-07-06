using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;

namespace Frosty.Sdk.Managers.Loaders;

public class ManifestAssetLoader : IAssetLoader
{
    public void Load()
    {
        // This format has all SuperBundles stripped
        // all of the bundles and chunks of all SuperBundles are put into the manifest
        // afaik u cant reconstruct the SuperBundles, so this might make things a bit ugly
        // They also seem to have catalog files which entries are not used, but they still make a sanity check for the offsets and indices in the file
        
        // AssetManager.AddSuperBundle("Manifest");
        //
        // DbObject manifest = FileSystemManager.Manifest!;
        //
        // CasFileIdentifier file = CasFileIdentifier.FromFileIdentifierV2(manifest["file"].As<uint>());
        //
        // string path = FileSystemManager.GetFilePath(file);
        //
        // using (DataStream stream = new(new FileStream(FileSystemManager.ResolvePath(path), FileMode.Open, FileAccess.Read)))
        // {
        //     stream.Position = manifest["offset"].As<long>();
        //
        //     uint resourceInfoCount = stream.ReadUInt32();
        //     uint bundleCount = stream.ReadUInt32();
        //     uint chunkCount = stream.ReadUInt32();
        //
        //     CasMergedResourceInfo[] files = new CasMergedResourceInfo[resourceInfoCount];
        //
        //     // resource infos
        //     for (int i = 0; i < resourceInfoCount; i++)
        //     {
        //         files[i] = new CasMergedResourceInfo(stream.ReadUInt32(), stream.ReadUInt32(),
        //             stream.ReadInt64());
        //     }
        //     
        //     // bundles
        //     for (int i = 0; i < bundleCount; i++)
        //     {
        //         int nameHash = stream.ReadInt32();
        //         int startIndex = stream.ReadInt32();
        //         int resourceCount = stream.ReadInt32();
        //
        //         // unknown, always 0
        //         stream.ReadInt32();
        //         stream.ReadInt32();
        //
        //         int index = startIndex;
        //         CasMergedResourceInfo resourceInfo = files[startIndex];
        //         
        //         DataStream dataStream =
        //             new (new FileStream(
        //                 FileSystemManager.ResolvePath(
        //                     FileSystemManager.GetFilePath(resourceInfo.CasMergedFileInfo)), FileMode.Open,
        //                 FileAccess.Read));
        //         
        //         dataStream.Position = resourceInfo.Offset;
        //         DbObject bundle = BinaryBundle.Deserialize(dataStream);
        //
        //         foreach (DbObject ebx in bundle["ebx"].As<DbObject>())
        //         {
        //             if (dataStream.Position == resourceInfo.Offset + resourceInfo.Size)
        //             {
        //                 dataStream.Dispose();
        //                 resourceInfo = files[++index];
        //                 dataStream =
        //                     new DataStream(new FileStream(
        //                         FileSystemManager.ResolvePath(
        //                             FileSystemManager.GetFilePath(resourceInfo.CasMergedFileInfo)), FileMode.Open,
        //                         FileAccess.Read));
        //                 dataStream.Position = resourceInfo.Offset;
        //             }
        //             
        //             ebx.AddValue("fileInfo", resourceInfo.CasMergedFileInfo);
        //             ebx.AddValue("offset", dataStream.Position);
        //             ebx.AddValue("size", StandardAssetLoader.GetSize(dataStream, ebx["originalSize"].As<long>()));
        //         }
        //         
        //         foreach (DbObject res in bundle["res"].As<DbObject>())
        //         {
        //             if (dataStream.Position == resourceInfo.Offset + resourceInfo.Size)
        //             {
        //                 dataStream.Dispose();
        //                 resourceInfo = files[++index];
        //                 dataStream =
        //                     new DataStream(new FileStream(
        //                         FileSystemManager.ResolvePath(
        //                             FileSystemManager.GetFilePath(resourceInfo.CasMergedFileInfo)), FileMode.Open,
        //                         FileAccess.Read));
        //                 dataStream.Position = resourceInfo.Offset;
        //             }
        //
        //             res.AddValue("fileInfo", resourceInfo.CasMergedFileInfo);
        //             res.AddValue("offset", dataStream.Position);
        //             res.AddValue("size", StandardAssetLoader.GetSize(dataStream, res["originalSize"].As<long>()));
        //         }
        //         
        //         foreach (DbObject chunk in bundle["chunks"].As<DbObject>())
        //         {
        //             if (dataStream.Position == resourceInfo.Offset + resourceInfo.Size)
        //             {
        //                 dataStream.Dispose();
        //                 resourceInfo = files[++index];
        //                 dataStream =
        //                     new DataStream(new FileStream(
        //                         FileSystemManager.ResolvePath(
        //                             FileSystemManager.GetFilePath(resourceInfo.CasMergedFileInfo)), FileMode.Open,
        //                         FileAccess.Read));
        //                 dataStream.Position = resourceInfo.Offset;
        //             }
        //
        //             chunk.AddValue("fileInfo", resourceInfo.CasMergedFileInfo);
        //             chunk.AddValue("offset", dataStream.Position);
        //             chunk.AddValue("size", StandardAssetLoader.GetSize(dataStream, chunk["originalSize"].As<long>()));
        //         }
        //         
        //         dataStream.Dispose();
        //         
        //         Debug.Assert(index - startIndex + 1 == resourceCount, "Didn't read the correct resources.");
        //         
        //         if (!ProfilesLibrary.SharedBundles.TryGetValue(nameHash, out string? name))
        //         {
        //             // we get the name while processing the ebx, since blueprint/sublevel bundles always have an ebx asset with the same name
        //             name = nameHash.ToString("x8");
        //         }
        //         AssetManager.ProcessBundle(bundle, 0, name);
        //     }
        //     
        //     // chunks
        //     for (int i = 0; i < chunkCount; i++)
        //     {
        //         Guid chunkId = stream.ReadGuid();
        //         CasMergedResourceInfo resourceInfo = files[stream.ReadInt32()];
        //
        //         ChunkAssetEntry entry = new(chunkId, Sha1.Zero, resourceInfo.Size, 0, 0, 0)
        //         {
        //             Location = AssetDataLocation.CasNonIndexed,
        //         };
        //
        //         entry.FileInfos.Add(new IFileInfo(resourceInfo.CasMergedFileInfo, resourceInfo.Offset, resourceInfo.Size));
        //
        //         AssetManager.AddSuperBundleChunk(entry);
        //     }
        // }
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