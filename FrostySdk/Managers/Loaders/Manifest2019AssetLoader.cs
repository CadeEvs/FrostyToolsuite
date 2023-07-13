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
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Loaders;

public class Manifest2019AssetLoader : IAssetLoader
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
                    int superBundleId = AssetManager.AddSuperBundle(sbInfo.Name);
                
                    if (!LoadToc(sbInfo.Name, superBundleId, tocPath, ref bundles, isPatched))
                    {
                        continue;
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
                    int superBundleId = AssetManager.AddSuperBundle(sbInfo.Name);
                
                    if (!LoadToc(sbName, superBundleId, tocPath, ref bundles, isPatched))
                    {
                        continue;
                    }

                    LoadSb(bundles, superBundleId);
                }
            }
        }
    }

    private bool LoadToc(string sbName, int superBundleId, string path, ref List<BundleInfo> bundles,
        bool isPatched)
    {
        using (BlockStream stream = BlockStream.FromFile(path, true))
        {
            uint bundleHashMapOffset = stream.ReadUInt32(Endian.Big);
            uint bundleDataOffset = stream.ReadUInt32(Endian.Big);
            int bundlesCount = stream.ReadInt32(Endian.Big);

            uint chunkHashMapOffset = stream.ReadUInt32(Endian.Big);
            uint chunkGuidOffset = stream.ReadUInt32(Endian.Big);
            int chunksCount = stream.ReadInt32(Endian.Big);

            // not used by any game rn, maybe crypto stuff
            stream.Position += sizeof(uint);
            stream.Position += sizeof(uint);

            uint namesOffset = stream.ReadUInt32(Endian.Big);

            uint chunkDataOffset = stream.ReadUInt32(Endian.Big);
            int dataCount = stream.ReadInt32(Endian.Big);

            Flags flags = (Flags)stream.ReadInt32(Endian.Big);

            if (flags.HasFlag(Flags.HasBaseBundles) || flags.HasFlag(Flags.HasBaseChunks))
            {
                string tocPath = FileSystemManager.ResolvePath($"native_data/{sbName}.toc");
                LoadToc(sbName, superBundleId, tocPath, ref bundles, false);
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

            if (bundlesCount != 0)
            {
                if (flags.HasFlag(Flags.HasCompressedNames))
                {
                    stream.Position = namesOffset;
                    huffmanDecoder!.ReadEncodedData(stream, namesCount, Endian.Big);

                    stream.Position = tableOffset;
                    huffmanDecoder.ReadHuffmanTable(stream, tableCount, Endian.Big);
                }

                stream.Position = bundleHashMapOffset;
                stream.Position += sizeof(int) * bundlesCount;

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
                stream.Position += sizeof(int) * chunksCount;

                stream.Position = chunkGuidOffset;
                Guid[] chunkGuids = new Guid[dataCount / 3];
                Span<byte> b = stackalloc byte[16];
                for (int i = 0; i < chunksCount; i++)
                {
                    stream.ReadExactly(b);
                    b.Reverse();

                    Guid guid = new(b);

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
                for (int i = 0; i < dataCount / 3; i++)
                {
                    CasFileIdentifier casFileIdentifier = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32(Endian.Big));
                    uint offset = stream.ReadUInt32(Endian.Big);
                    uint size = stream.ReadUInt32(Endian.Big);

                    ChunkAssetEntry chunk = new(chunkGuids[i], Sha1.Zero, size, 0, 0, superBundleId);

                    chunk.FileInfos.Add(new CasFileInfo(casFileIdentifier, offset, size, 0));

                    AssetManager.AddSuperBundleChunk(chunk);
                }
            }

            return true;
        }
        
    }

    private void LoadSb(List<BundleInfo> bundles, int superBundleId)
    {
        string patchSbPath = string.Empty;
        string baseSbPath = string.Empty;
        BlockStream? patchStream = null;
        BlockStream? baseStream = null;

        foreach (BundleInfo bundleInfo in bundles)
        {
            // get where the bundle is stored, either in toc or sb file
            byte flag;
            if ((bundleInfo.Size & 0xC0000000) == 0x40000000)
            {
                flag = 1;
            }
            else if ((bundleInfo.Size & 0xC0000000) == 0x80000000)
            {
                throw new NotImplementedException("unknown flag");
            }
            else
            {
                flag = 0;
            }
            
            // get correct stream for this bundle
            BlockStream stream;
            if (bundleInfo.IsPatch)
            {
                if (patchStream == null || patchSbPath != bundleInfo.SbName)
                {
                    patchSbPath = bundleInfo.SbName;
                    patchStream?.Dispose();
                    
                    if (flag == 1)
                    {
                        patchStream = BlockStream.FromFile(
                            FileSystemManager.ResolvePath($"native_patch/{patchSbPath}.toc"), true);
                    }
                    else
                    {
                        patchStream = BlockStream.FromFile(
                            FileSystemManager.ResolvePath($"native_patch/{patchSbPath}.sb"), false);
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
                        baseStream = BlockStream.FromFile(
                            FileSystemManager.ResolvePath($"native_data/{baseSbPath}.toc"), true);
                    }
                    else
                    {
                        baseStream = BlockStream.FromFile(
                            FileSystemManager.ResolvePath($"native_data/{baseSbPath}.sb"), false);
                    }
                }
                stream = baseStream;
            }
            
            BinaryBundle bundle;

            stream.Position = bundleInfo.Offset;
            
            int bundleOffset = stream.ReadInt32(Endian.Big);
            int bundleSize = stream.ReadInt32(Endian.Big);
            uint locationOffset = stream.ReadUInt32(Endian.Big);
            uint totalCount = stream.ReadUInt32(Endian.Big);
            uint dataOffset = stream.ReadUInt32(Endian.Big);

            // not used by any game rn, again maybe crypto stuff
            stream.Position += sizeof(uint);
            stream.Position += sizeof(uint);
            // maybe count for the offsets above
            stream.Position += sizeof(int);

            // bundles can be stored in this file or in a separate cas file, then the first file info is for the bundle.
            // Seems to be related to the flag for in which file the sb is stored
            bool inlineBundle = !(bundleOffset == 0 && bundleSize == 0);
            
            stream.Position = bundleInfo.Offset + locationOffset;

            bool[] flags = new bool[totalCount];
            for (uint i = 0; i < totalCount; i++)
            {
                flags[i] = stream.ReadBoolean();
            }

            CasFileIdentifier casFileIdentifier = default;
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
                    casFileIdentifier = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32(Endian.Big));
                }
                uint offset = stream.ReadUInt32(Endian.Big);
                int size = stream.ReadInt32(Endian.Big);

                string path = FileSystemManager.GetFilePath(casFileIdentifier);

                using (BlockStream casStream = BlockStream.FromFile(FileSystemManager.ResolvePath(path), offset, size))
                {
                    bundle = BinaryBundle.Deserialize(casStream);
                }
            }
            
            int bundleId = AssetManager.AddBundle(bundleInfo.Name, superBundleId);

            foreach (EbxAssetEntry ebx in bundle.EbxList)
            {
                if (flags[z++])
                {
                    casFileIdentifier = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32(Endian.Big));
                }
                
                uint offset = stream.ReadUInt32(Endian.Big);
                ebx.Size = stream.ReadUInt32(Endian.Big);
                
                ebx.FileInfos.Add(new CasFileInfo(casFileIdentifier, offset, (uint)ebx.Size, 0));
                
                AssetManager.AddEbx(ebx, bundleId);
            }

            foreach (ResAssetEntry res in bundle.ResList)
            {
                if (flags[z++])
                {
                    casFileIdentifier = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32(Endian.Big));
                }

                uint offset = stream.ReadUInt32(Endian.Big);
                res.Size = stream.ReadUInt32(Endian.Big);
                
                res.FileInfos.Add(new CasFileInfo(casFileIdentifier, offset, (uint)res.Size, 0));
                
                AssetManager.AddRes(res, bundleId);
            }

            foreach (ChunkAssetEntry chunk in bundle.ChunkList)
            {
                if (flags[z++])
                {
                    casFileIdentifier = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32(Endian.Big));
                }

                uint offset = stream.ReadUInt32(Endian.Big);
                chunk.Size = stream.ReadUInt32(Endian.Big);
                
                chunk.FileInfos.Add(new CasFileInfo(casFileIdentifier, offset, (uint)chunk.Size, chunk.LogicalOffset));
                
                AssetManager.AddChunk(chunk, bundleId);
            }
        }

        patchStream?.Dispose();
        baseStream?.Dispose();
    }
}