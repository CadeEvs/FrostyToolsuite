using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Loaders.Helpers;
using Frosty.Sdk.Utils;
using FileInfo = Frosty.Sdk.Managers.Infos.FileInfo;

namespace Frosty.Sdk.Managers.Loaders;

public class CasAssetLoader : IAssetLoader
{
    [Flags]
    private enum Flags
    {
        HasBaseBundles = 1 << 0, // base toc has bundles that the patch doesnt have
        HasBaseChunks = 1 << 1, // base toc has chunks that the patch doesnt have
        HasCompressedNames = 1 << 2 // bundle names are huffman encoded
    }
    
    public void Load()
    {
        foreach (SuperBundleInfo sbInfo in FileSystemManager.EnumerateSuperBundles())
        {
            // TODO: maybe just add super bundles to assetmanager if they have data
            int superBundleId = AssetManager.AddSuperBundle(sbInfo.Name);

            foreach (KeyValuePair<int, InstallChunkType> installChunk in sbInfo.InstallChunks)
            {
                if (installChunk.Value.HasFlag(InstallChunkType.Default))
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
                
                    List<BundleInfo> bundles = new();
                
                    using (DataStream stream = new(new FileStream(tocPath, FileMode.Open, FileAccess.Read), true))
                    {
                        if (!LoadToc(sbInfo.Name, superBundleId, stream, ref bundles, isPatched))
                        {
                            continue;
                        }
                    }

                    LoadSb(bundles, superBundleId);
                }
                
                if (installChunk.Value.HasFlag(InstallChunkType.Split))
                {
                    InstallChunkInfo installChunkInfo = FileSystemManager.GetInstallChunkInfo(installChunk.Key);

                    string sbName = $"{installChunkInfo.InstallBundle}{sbInfo.Name[sbInfo.Name.IndexOf("/", StringComparison.Ordinal)..]}";
                    
                    bool isPatched = true;
                    string tocPath = FileSystemManager.ResolvePath($"native_patch/{sbName}.toc");
                    if (string.IsNullOrEmpty(tocPath))
                    {
                        isPatched = false;
                        tocPath = FileSystemManager.ResolvePath($"native_data/{sbName}.toc");
                        if (string.IsNullOrEmpty(tocPath))
                        {
                            continue;
                        }
                    }
                
                    List<BundleInfo> bundles = new();
                
                    using (DataStream stream = new(new FileStream(tocPath, FileMode.Open, FileAccess.Read), true))
                    {
                        if (!LoadToc(sbName, superBundleId, stream, ref bundles, isPatched))
                        {
                            continue;
                        }
                    }

                    LoadSb(bundles, superBundleId);
                }
            }
        }
    }

    private bool LoadToc(string sbName, int superBundleId, DataStream stream, ref List<BundleInfo> bundles,
        bool isPatched)
    {
        uint bundleHashMapOffset = stream.ReadUInt32(Endian.Big);
        uint bundleDataOffset = stream.ReadUInt32(Endian.Big);
        int bundlesCount = stream.ReadInt32(Endian.Big);

        uint chunkHashMapOffset = stream.ReadUInt32(Endian.Big);
        uint chunkGuidOffset = stream.ReadUInt32(Endian.Big);
        int chunksCount = stream.ReadInt32(Endian.Big);

        uint unknownOffset1 = stream.ReadUInt32(Endian.Big); // not used
        uint unknownOffset2 = stream.ReadUInt32(Endian.Big); // not used

        uint namesOffset = stream.ReadUInt32(Endian.Big);

        uint chunkDataOffset = stream.ReadUInt32(Endian.Big);
        int dataCount = stream.ReadInt32(Endian.Big);

        Flags flags = (Flags)stream.ReadInt32(Endian.Big);

        if (flags.HasFlag(Flags.HasBaseBundles) || flags.HasFlag(Flags.HasBaseChunks))
        {
            string tocPath = FileSystemManager.ResolvePath($"native_data/{sbName}.toc");
            using (DataStream baseReader = new(new FileStream(tocPath, FileMode.Open, FileAccess.Read), true))
            {
                LoadToc(sbName, superBundleId, baseReader, ref bundles, false);
            }
        }

        uint namesCount = 0;
        uint tableCount = 0;
        uint tableOffset = uint.MaxValue;
        HuffmanDecoder? huffmanDecoder = null;

        if (flags.HasFlag(Flags.HasCompressedNames))
        {
            huffmanDecoder = new HuffmanDecoder();
            namesCount = stream.ReadUInt32(Endian.Big);
            tableCount = stream.ReadUInt32(Endian.Big);
            tableOffset = stream.ReadUInt32(Endian.Big);
        }

#if FROSTY_DEVELOPER
        Debug.Assert(unknownOffset1 == chunkDataOffset && unknownOffset2 == chunkDataOffset);
#endif

        if (bundlesCount != 0)
        {
            if (flags.HasFlag(Flags.HasCompressedNames))
            {
                stream.Position = namesOffset;
                huffmanDecoder!.ReadEncodedData(stream, namesCount, Endian.Big);

                stream.Position = tableOffset;
                huffmanDecoder.ReadHuffmanTable(stream, tableCount, Endian.Big);
            }

            int[] bundleHashMap = new int[bundlesCount];
            stream.Position = bundleHashMapOffset;
            for (int i = 0; i < bundlesCount; i++)
            {
                bundleHashMap[i] = stream.ReadInt32(Endian.Big);
            }

            stream.Position = bundleDataOffset;

            for (int i = 0; i < bundlesCount; i++)
            {
                int nameOffset = stream.ReadInt32(Endian.Big);
                uint bundleSize = stream.ReadUInt32(Endian.Big); // flag in first 2 bits: 0x40000000 inline sb
                long bundleOffset = stream.ReadInt64(Endian.Big);

                string name;

                if (flags.HasFlag(Flags.HasCompressedNames))
                {
                    name = huffmanDecoder!.ReadHuffmanEncodedString(nameOffset);
                }
                else
                {
                    long curPos = stream.Position;
                    stream.Position = namesOffset + nameOffset;
                    name = stream.ReadNullTerminatedString();
                    stream.Position = curPos;
                }

                int idx = bundles.FindIndex((bbi) => bbi.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (idx != -1)
                {
                    bundles.RemoveAt(idx);
                }

                BundleInfo bi = new()
                {
                    Name = name,
                    SbName = sbName,
                    Offset = bundleOffset,
                    Size = bundleSize,
                    IsPatch = isPatched
                };

                if (bundleSize != uint.MaxValue && bundleOffset != -1)
                {
                    bundles.Add(bi);
                }
            }
            huffmanDecoder?.Dispose();
        }
        if (chunksCount != 0)
        {
            stream.Position = chunkHashMapOffset;
            int[] chunkHashMap = new int[chunksCount];
            for (int i = 0; i < chunksCount; i++)
            {
                chunkHashMap[i] = stream.ReadInt32(Endian.Big);
            }

            stream.Position = chunkGuidOffset;
            Guid[] chunkGuids = new Guid[dataCount / 3];
            for (int i = 0; i < chunksCount; i++)
            {
                byte[] b = stream.ReadBytes(16);

                Array.Reverse(b);

                Guid guid = new Guid(b);

                // 0xFFFFFFFF remove chunk
                int index = stream.ReadInt32(Endian.Big);

                if (index != -1)
                {
                    // im guessing the unknown offsets are connected to this
                    byte flag = (byte)((index & 0xFF000000) >> 24);
#if FROSTY_DEVELOPER
                    Debug.Assert(flag == 1);
#endif
                    index = (index & 0xFFFFFF) / 3;


                    chunkGuids[index] = guid;
                }
            }

            stream.Position = chunkDataOffset;
            for (int i = 0; i < (dataCount / 3); i++)
            {
                CasFileInfo casFileInfo = stream.ReadUInt32(Endian.Big);
                uint offset = stream.ReadUInt32(Endian.Big);
                uint size = stream.ReadUInt32(Endian.Big);

                ChunkAssetEntry chunk = new(chunkGuids[i], Sha1.Zero, size, 0, 0, superBundleId)
                {
                    Location = AssetDataLocation.CasNonIndexed,
                    FileInfo = new FileInfo(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(casFileInfo)), offset, size)
                };

                AssetManager.AddSuperBundleChunk(chunk);
            }
        }

        return true;
    }

    private void LoadSb(List<BundleInfo> bundles, int superBundleId)
    {
        string patchSbPath = string.Empty;
        string baseSbPath = string.Empty;
        DataStream? patchStream = null;
        DataStream? baseStream = null;

        byte flag = 0;
        foreach (BundleInfo bundleInfo in bundles)
        {
            // get where the bundle is stored, either in toc or sb file
            if ((bundleInfo.Size & 0xC0000000) == 0x40000000)
            {
                flag = 1;
            }
            else if ((bundleInfo.Size & 0xC0000000) == 0x80000000)
            {
                throw new NotImplementedException("unkown flag");
            }
            else
            {
                flag = 0;
            }
            // get correct stream for this bundle
            DataStream stream;
            if (bundleInfo.IsPatch)
            {
                if (patchStream == null || patchSbPath != bundleInfo.SbName)
                {
                    patchSbPath = bundleInfo.SbName;
                    patchStream?.Dispose();
                    
                    if (flag == 1)
                    {
                        patchStream = new DataStream(new FileStream(
                            FileSystemManager.ResolvePath($"native_patch/{patchSbPath}.toc"), FileMode.Open,
                            FileAccess.Read), true);
                    }
                    else
                    {
                        patchStream = new DataStream(new FileStream(
                            FileSystemManager.ResolvePath($"native_patch/{patchSbPath}.sb"), FileMode.Open,
                            FileAccess.Read));
                    }
                }
                stream = patchStream;
            }
            else
            {
                if (baseStream == null || baseSbPath != bundleInfo.SbName)
                {
                    baseSbPath = bundleInfo.SbName;
                    baseStream?.Dispose();
                    
                    if (flag == 1)
                    {
                        baseStream = new DataStream(new FileStream(
                            FileSystemManager.ResolvePath($"native_data/{baseSbPath}.toc"), FileMode.Open,
                            FileAccess.Read), true);
                    }
                    else
                    {
                        baseStream = new DataStream(new FileStream(
                            FileSystemManager.ResolvePath($"native_data/{baseSbPath}.sb"), FileMode.Open,
                            FileAccess.Read));
                    }
                }
                stream = baseStream;
            }
            
            DbObject bundle;

            stream.Position = bundleInfo.Offset;
            
            int bundleOffset = stream.ReadInt32(Endian.Big);
            int bundleSize = stream.ReadInt32(Endian.Big);
            uint locationOffset = stream.ReadUInt32(Endian.Big);
            uint totalCount = stream.ReadUInt32(Endian.Big);
            uint dataOffset = stream.ReadUInt32(Endian.Big);

            uint unkOffset1 = stream.ReadUInt32(Endian.Big); // not used
            uint unkOffset2 = stream.ReadUInt32(Endian.Big); // not used
            int unkCount = stream.ReadInt32(Endian.Big); // count for unkOffset1 and 2 maybe

            // bundles can be stored in this file or in a seperate cas file, then the first file info is for the bundle. Seems to be related to the flag for in which file the sb is stored
            bool inlineBundle = !(bundleOffset == 0 && bundleSize == 0);
#if FROSTY_DEVELOPER
            Debug.Assert(unkOffset2 == unkOffset1 && unkCount == 0);
#endif
            stream.Position = bundleInfo.Offset + locationOffset;

            bool[] flags = new bool[totalCount];
            for (uint i = 0; i < totalCount; i++)
                flags[i] = stream.ReadBoolean();

            CasFileInfo casFileInfo = 0;
            uint offset = 0;
            int size = 0;
            int z = 0;

            if (inlineBundle)
            {
                stream.Position = bundleInfo.Offset + bundleOffset;
                bundle = BinaryBundle.Deserialize(stream);
                stream.Position = bundleInfo.Offset + dataOffset;
            }
            else
            {
                stream.Position = bundleInfo.Offset + dataOffset;

                if (flags[z++])
                {
                    casFileInfo = stream.ReadUInt32(Endian.Big);
                }
                offset = stream.ReadUInt32(Endian.Big);
                size = stream.ReadInt32(Endian.Big);

                string path = FileSystemManager.GetFilePath(casFileInfo);

                using (DataStream casStream = new(new FileStream(FileSystemManager.ResolvePath(path), FileMode.Open, FileAccess.Read)))
                {
                    casStream.Position = offset;
                    bundle = BinaryBundle.Deserialize(casStream);
                    Debug.Assert(casStream.Position == offset + size);
                }
            }

            foreach (DbObject ebx in bundle["ebx"].As<DbObject>())
            {
                if (flags[z++])
                {
                    casFileInfo = stream.ReadUInt32(Endian.Big);
                }

                offset = stream.ReadUInt32(Endian.Big);
                size = stream.ReadInt32(Endian.Big);

                ebx.AddValue("casFileInfo", casFileInfo);
                ebx.AddValue("offset", offset);
                ebx.AddValue("size", size);
            }

            foreach (DbObject res in bundle["res"].As<DbObject>())
            {
                if (flags[z++])
                {
                    casFileInfo = stream.ReadUInt32(Endian.Big);
                }

                offset = stream.ReadUInt32(Endian.Big);
                size = stream.ReadInt32(Endian.Big);

                res.AddValue("casFileInfo", casFileInfo);
                res.AddValue("offset", offset);
                res.AddValue("size", size);
            }

            foreach (DbObject chunk in bundle["chunks"].As<DbObject>())
            {
                if (flags[z++])
                {
                    casFileInfo = stream.ReadUInt32(Endian.Big);
                }

                offset = stream.ReadUInt32(Endian.Big);
                size = stream.ReadInt32(Endian.Big);

                chunk.AddValue("casFileInfo", casFileInfo);
                chunk.AddValue("offset", offset);
                chunk.AddValue("size", size);
            }

            // process assets
            AssetManager.ProcessBundle(bundle, superBundleId, bundleInfo.Name);

        }

        patchStream?.Dispose();
        baseStream?.Dispose();
    }
}