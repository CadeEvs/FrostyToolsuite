using System;
using System.Collections.Generic;
using Frosty.Sdk.DbObjectElements;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Infos.FileInfos;

namespace Frosty.Sdk.Managers.Loaders;

public class ManifestAssetLoader : IAssetLoader
{
    public void Load()
    {
        // This format has all SuperBundles stripped
        // all of the bundles and chunks of all SuperBundles are put into the manifest
        // afaik u cant reconstruct the SuperBundles, so this might make things a bit ugly
        // They also seem to have catalog files which entries are not used, but they still make a sanity check for the offsets and indices in the file
        
        AssetManager.AddSuperBundle("Manifest");
        
        DbObjectDict manifest = FileSystemManager.Manifest!;
        
        CasFileIdentifier file = CasFileIdentifier.FromFileIdentifierV2(manifest.AsUInt("file"));
        
        string path = FileSystemManager.GetFilePath(file);
        
        using (BlockStream stream = BlockStream.FromFile(FileSystemManager.ResolvePath(path), manifest.AsUInt("offset"), manifest.AsInt("size")))
        {
            uint resourceInfoCount = stream.ReadUInt32();
            uint bundleCount = stream.ReadUInt32();
            uint chunkCount = stream.ReadUInt32();

            (CasFileIdentifier, uint, long)[] files = new (CasFileIdentifier, uint, long)[resourceInfoCount];
        
            // resource infos
            for (int i = 0; i < resourceInfoCount; i++)
            {
                files[i] = (CasFileIdentifier.FromFileIdentifierV2(stream.ReadUInt32()), stream.ReadUInt32(),
                    (uint)stream.ReadInt64());
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
                
                (CasFileIdentifier, uint, long) resourceInfo = files[startIndex];
                BinaryBundle bundle;
                using (BlockStream bundleStream = BlockStream.FromFile(FileSystemManager.ResolvePath(
                           FileSystemManager.GetFilePath(resourceInfo.Item1)), resourceInfo.Item2, (int)resourceInfo.Item3))
                {
                     bundle = BinaryBundle.Deserialize(bundleStream);
                }
                
                if (!ProfilesLibrary.SharedBundles.TryGetValue(nameHash, out string? name))
                {
                    // we get the name while processing the ebx, since blueprint/sublevel bundles always have an ebx asset with the same name
                    name = nameHash.ToString("x8");
                }
            
                int bundleId = AssetManager.AddBundle(name, 0);

                foreach (EbxAssetEntry ebx in bundle.EbxList)
                {
                    IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(ebx.Sha1);
                    if (fileInfos is not null)
                    {
                        ebx.FileInfos.UnionWith(fileInfos);
                    }
                    
                    AssetManager.AddEbx(ebx, bundleId);
                }

                foreach (ResAssetEntry res in bundle.ResList)
                {
                    IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(res.Sha1);
                    if (fileInfos is not null)
                    {
                        res.FileInfos.UnionWith(fileInfos);
                    }

                    AssetManager.AddRes(res, bundleId);
                }

                foreach (ChunkAssetEntry chunk in bundle.ChunkList)
                {
                    IEnumerable<IFileInfo>? fileInfos = ResourceManager.GetFileInfos(chunk.Sha1);
                    if (fileInfos is not null)
                    {
                        chunk.FileInfos.UnionWith(fileInfos);
                    }

                    AssetManager.AddChunk(chunk, bundleId);
                }
            }
            
            // chunks
            for (int i = 0; i < chunkCount; i++)
            {
                Guid chunkId = stream.ReadGuid();
                (CasFileIdentifier, uint, long) resourceInfo = files[stream.ReadInt32()];

                ChunkAssetEntry entry = new(chunkId, Sha1.Zero, resourceInfo.Item3, 0, 0, 0);

                entry.FileInfos.Add(
                    new CasFileInfo(resourceInfo.Item1, resourceInfo.Item2, (uint)resourceInfo.Item3, 0));

                AssetManager.AddSuperBundleChunk(entry);
            }
        }
    }
}