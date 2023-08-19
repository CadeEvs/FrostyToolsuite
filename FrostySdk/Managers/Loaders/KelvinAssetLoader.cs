using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Infos.FileInfos;

namespace Frosty.Sdk.Managers.Loaders;

public class KelvinAssetLoader : IAssetLoader
{
    public void Load()
    {
        foreach (SuperBundleInfo sbInfo in FileSystemManager.EnumerateSuperBundles())
        {
            foreach (KeyValuePair<int, InstallChunkType> installChunk in sbInfo.InstallChunks)
            {
                if (installChunk.Value.HasFlag(InstallChunkType.Default))
                {
                    string tocPath = FileSystemManager.ResolvePath(true, $"{sbInfo.Name}.toc");
                    if (string.IsNullOrEmpty(tocPath))
                    {
                        tocPath = FileSystemManager.ResolvePath(false, $"{sbInfo.Name}.toc");
                        if (string.IsNullOrEmpty(tocPath))
                        {
                            continue;
                        }
                    }
                    
                    int superBundleId = AssetManager.AddSuperBundle(sbInfo.Name);

                    LoadToc(superBundleId, tocPath);
                }
                
                if (installChunk.Value.HasFlag(InstallChunkType.Split))
                {
                    InstallChunkInfo installChunkInfo = FileSystemManager.GetInstallChunkInfo(installChunk.Key);

                    string sbName = $"{installChunkInfo.InstallBundle}{sbInfo.Name[sbInfo.Name.IndexOf("/", StringComparison.Ordinal)..]}";

                    string tocPath = FileSystemManager.ResolvePath(true, $"{sbName}.toc");
                    if (string.IsNullOrEmpty(tocPath))
                    {
                        tocPath = FileSystemManager.ResolvePath(false, $"{sbName}.toc");
                        if (string.IsNullOrEmpty(tocPath))
                        {
                            continue;
                        }
                    }
                    
                    int superBundleId = AssetManager.AddSuperBundle(sbInfo.Name);
                
                    LoadToc(superBundleId, tocPath);
                }
            }
        }
    }

    private readonly struct FileIdentifier
    {
        public readonly int FileIndex;
        public readonly uint Offset;
        public readonly uint Size;

        public FileIdentifier(int inFileIndex, uint inOffset, uint inSize)
        {
            FileIndex = inFileIndex;
            Offset = inOffset;
            Size = inSize;
        }
    }
    
    private void LoadToc(int superBundleId, string tocPath)
    {
        using (BlockStream stream = BlockStream.FromFile(tocPath, true))
        {
            uint magic = stream.ReadUInt32();
            uint bundlesOffset = stream.ReadUInt32();
            uint chunksOffset = stream.ReadUInt32();
    
            if (magic == 0xC3E5D5C3)
            {
                stream.Decrypt(KeyManager.GetKey("BundleEncryptionKey"), PaddingMode.None);
            }
    
            if (bundlesOffset != 0xFFFFFFFF)
            {
                stream.Position = bundlesOffset;
    
                int bundleCount = stream.ReadInt32();
    
                // bundle hashmap
                stream.Position += sizeof(int) * bundleCount;
    
                for (int i = 0; i < bundleCount; i++)
                {
                    uint bundleOffset = stream.ReadUInt32();
    
                    long curPos = stream.Position;
    
                    stream.Position = bundleOffset;
    
                    string name = ReadString(stream, stream.ReadInt32());
    
                    List<FileIdentifier> files = new();
                    while (true)
                    {
                        int file = stream.ReadInt32();
                        uint fileOffset = stream.ReadUInt32();
                        uint fileSize = stream.ReadUInt32();
    
                        files.Add(new FileIdentifier(file & 0x7FFFFFFF, fileOffset, fileSize));
                        if ((file & 0x80000000) == 0)
                        {
                            break;
                        }
                    }
    
                    stream.Position = curPos;
    
                    int index = 0;
                    FileIdentifier resourceInfo = files[index];

                    BlockStream dataStream = BlockStream.FromFile(
                        FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)),
                        resourceInfo.Offset, (int)resourceInfo.Size);
    
                    BinaryBundle bundle = BinaryBundle.Deserialize(dataStream);
            
                    int bundleId = AssetManager.AddBundle(name, superBundleId);
    
                    foreach (EbxAssetEntry ebx in bundle.EbxList)
                    {
                         if (dataStream.Position == resourceInfo.Size)
                         {
                             dataStream.Dispose();
                             resourceInfo = files[++index];
                             dataStream = BlockStream.FromFile(
                                 FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)),
                                 resourceInfo.Offset, (int)resourceInfo.Size);
                         }
                
                         uint offset = (uint)dataStream.Position;
                         ebx.Size = DbObjectAssetLoader.GetSize(dataStream, ebx.OriginalSize);

                         ebx.FileInfos.Add(new PathFileInfo(FileSystemManager.GetFilePath(resourceInfo.FileIndex),
                             resourceInfo.Offset + offset, (uint)ebx.Size, 0));
                
                         AssetManager.AddEbx(ebx, bundleId);
                    }
                    foreach (ResAssetEntry res in bundle.ResList)
                    {
                         if (dataStream.Position == resourceInfo.Size)
                         {
                             dataStream.Dispose();
                             resourceInfo = files[++index];
                             dataStream = BlockStream.FromFile(
                                 FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)),
                                 resourceInfo.Offset, (int)resourceInfo.Size);
                         }
                         
                         uint offset = (uint)dataStream.Position;
                         res.Size = DbObjectAssetLoader.GetSize(dataStream, res.OriginalSize);

                         res.FileInfos.Add(new PathFileInfo(FileSystemManager.GetFilePath(resourceInfo.FileIndex),
                             resourceInfo.Offset + offset, (uint)res.Size, 0));
                        
                         AssetManager.AddRes(res, bundleId);
                    }
                    foreach (ChunkAssetEntry chunk in bundle.ChunkList)
                    {
                         if (dataStream.Position == resourceInfo.Size)
                         {
                             dataStream.Dispose();
                             resourceInfo = files[++index];
                             dataStream = BlockStream.FromFile(
                                 FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)),
                                 resourceInfo.Offset, (int)resourceInfo.Size);
                         }

                         uint offset = (uint)dataStream.Position;
                         chunk.Size = DbObjectAssetLoader.GetSize(dataStream,
                             (chunk.LogicalOffset & 0xFFFF) | chunk.LogicalSize);

                         chunk.FileInfos.Add(new PathFileInfo(FileSystemManager.GetFilePath(resourceInfo.FileIndex),
                             resourceInfo.Offset + offset, (uint)chunk.Size, chunk.LogicalOffset));
                        
                         AssetManager.AddChunk(chunk, bundleId);
                    }
    
                    dataStream.Dispose();
                }
            }
    
            if (chunksOffset != 0xFFFFFFFF)
            {
                stream.Position = chunksOffset;
                int chunksCount = stream.ReadInt32();
    
                // hashmap
                stream.Position += sizeof(int) * chunksCount;
    
                for (int i = 0; i < chunksCount; i++)
                {
                    int offset = stream.ReadInt32();
    
                    long pos = stream.Position;
                    stream.Position = offset;
    
                    Guid guid = stream.ReadGuid();
                    int fileIndex = stream.ReadInt32();
                    uint dataOffset = stream.ReadUInt32();
                    uint dataSize = stream.ReadUInt32();
    
                    ChunkAssetEntry chunk = new(guid, Sha1.Zero, dataSize, 0, 0, superBundleId);
    
                    chunk.FileInfos.Add(new PathFileInfo(FileSystemManager.GetFilePath(fileIndex), dataOffset, dataSize, 0));
                    
                    AssetManager.AddSuperBundleChunk(chunk);
                    
                    stream.Position = pos;
                }
            }
        }
    }
    
    private string ReadString(DataStream reader, int offset)
    {
        long curPos = reader.Position;
        StringBuilder sb = new();

        do
        {
            reader.Position = offset - 1;
            string tmp = reader.ReadNullTerminatedString();
            offset = reader.ReadInt32();

            sb.Append(tmp);
        } while (offset != 0);

        reader.Position = curPos;
        return new string(sb.ToString().Reverse().ToArray());
    }
}