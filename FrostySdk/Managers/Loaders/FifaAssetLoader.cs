using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Managers.Infos;
using Frosty.Sdk.Managers.Loaders.Helpers;
using FileInfo = Frosty.Sdk.Managers.Infos.FileInfo;

namespace Frosty.Sdk.Managers.Loaders;

public class FifaAssetLoader : IAssetLoader
{
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
                
                    using (DataStream stream = new(new FileStream(tocPath, FileMode.Open, FileAccess.Read), true))
                    {
                        LoadToc(sbInfo.Name, superBundleId, stream, isPatched);
                    }
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
                
                    using (DataStream stream = new(new FileStream(tocPath, FileMode.Open, FileAccess.Read), true))
                    {
                        LoadToc(sbName, superBundleId, stream, isPatched);
                    }
                }
            }
        }
    }

    private void LoadToc(string sbName, int superBundleId, DataStream stream, bool isPatched)
    {
        uint magic = stream.ReadUInt32();
        uint bundlesOffset = stream.ReadUInt32();
        uint chunksOffset = stream.ReadUInt32();

        if (magic == 0xC3E5D5C3)
        {
            // get second encryption key
            byte[] key = KeyManager.GetKey("Key2");

            byte[] buffer = stream.ReadBytes((int)(stream.Length - 0x0C));
            byte[] outBuffer = new byte[buffer.Length + 0x0C];
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = key;
                aes.Padding = PaddingMode.None;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (CryptoStream cryptoStream = new(new MemoryStream(buffer), decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.Read(outBuffer, 0x0C, buffer.Length);
                }
            }

            stream.Dispose();
            stream = new DataStream(new MemoryStream(outBuffer));
        }

        if (bundlesOffset != 0)
        {
            stream.Position = bundlesOffset;

            int bundleCount = stream.ReadInt32();

            // hashmap
            for (int i = 0; i < bundleCount; i++)
            {
                stream.ReadInt32();
            }

            for (int i = 0; i < bundleCount; i++)
            {
                uint bundleOffset = stream.ReadUInt32();

                long curPos = stream.Position;

                stream.Position = bundleOffset;

                string name = ReadString(stream, stream.ReadInt32() - 1);

                List<CasMergedResourceInfo> files = new();
                while (true)
                {
                    int file = stream.ReadInt32();
                    uint fileOffset = stream.ReadUInt32();
                    uint fileSize = stream.ReadUInt32();

                    files.Add(new CasMergedResourceInfo(file & 0x7FFFFFFF, fileOffset, fileSize));
                    if ((file & 0x80000000) == 0)
                    {
                        break;
                    }
                }

                stream.Position = curPos;

                int index = 0;
                CasMergedResourceInfo resourceInfo = files[index];

                DataStream dataStream =
                    new(new FileStream(
                        FileSystemManager.ResolvePath(
                            FileSystemManager.GetFilePath(resourceInfo.FileIndex)), FileMode.Open,
                        FileAccess.Read));

                dataStream.Position = resourceInfo.Offset;
                DbObject bundle = BinaryBundle.Deserialize(dataStream);

                foreach (DbObject ebx in bundle["ebx"].As<DbObject>())
                {
                    if (dataStream.Position == resourceInfo.Offset + resourceInfo.Size)
                    {
                        dataStream.Dispose();
                        resourceInfo = files[++index];
                        dataStream =
                            new(new FileStream(
                                FileSystemManager.ResolvePath(
                                    FileSystemManager.GetFilePath(resourceInfo.FileIndex)), FileMode.Open,
                                FileAccess.Read));
                    }

                    ebx.AddValue("path",
                        FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)));
                    ebx.AddValue("offset", dataStream.Position);
                    ebx.AddValue("size", StandardAssetLoader.GetSize(dataStream, ebx["originalSize"].As<long>()));
                }
                foreach (DbObject res in bundle["res"].As<DbObject>())
                {
                    if (dataStream.Position == resourceInfo.Offset + resourceInfo.Size)
                    {
                        dataStream.Dispose();
                        resourceInfo = files[++index];
                        dataStream =
                            new(new FileStream(
                                FileSystemManager.ResolvePath(
                                    FileSystemManager.GetFilePath(resourceInfo.FileIndex)), FileMode.Open,
                                FileAccess.Read));
                    }

                    res.AddValue("path",
                        FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)));
                    res.AddValue("offset", dataStream.Position);
                    res.AddValue("size", StandardAssetLoader.GetSize(dataStream, res["originalSize"].As<long>()));
                }
                foreach (DbObject chunk in bundle["chunks"].As<DbObject>())
                {
                    if (dataStream.Position == resourceInfo.Offset + resourceInfo.Size)
                    {
                        dataStream.Dispose();
                        resourceInfo = files[++index];
                        dataStream =
                            new(new FileStream(
                                FileSystemManager.ResolvePath(
                                    FileSystemManager.GetFilePath(resourceInfo.FileIndex)), FileMode.Open,
                                FileAccess.Read));
                    }

                    chunk.AddValue("path",
                        FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(resourceInfo.FileIndex)));
                    chunk.AddValue("offset", dataStream.Position);
                    chunk.AddValue("size", StandardAssetLoader.GetSize(dataStream, chunk["originalSize"].As<long>()));
                }

                dataStream.Dispose();

                AssetManager.ProcessBundle(bundle, superBundleId, name);
            }
        }

        if (chunksOffset > 0)
        {
            stream.Position = chunksOffset;
            int chunksCount = stream.ReadInt32();

            // hashmap
            for (int i = 0; i < chunksCount; i++)
            {
                stream.ReadInt32();
            }

            for (int i = 0; i < chunksCount; i++)
            {
                int offset = stream.ReadInt32();

                long pos = stream.Position;
                stream.Position = offset;

                Guid guid = stream.ReadGuid();
                int fileIndex = stream.ReadInt32();
                int dataOffset = stream.ReadInt32();
                int dataSize = stream.ReadInt32();

                ChunkAssetEntry chunk = new(guid, Sha1.Zero, dataSize, 0, 0, superBundleId)
                {
                    Location = AssetDataLocation.CasNonIndexed,
                    FileInfo = new FileInfo(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(fileIndex)), dataOffset, dataSize)
                };
                
                
                
                stream.Position = pos;
            }
        }
    }
    
    private string ReadString(DataStream reader, int offset)
    {
        long curPos = reader.Position;
        StringBuilder sb = new StringBuilder();

        do
        {
            reader.Position = offset;
            string tmp = reader.ReadNullTerminatedString();
            offset = reader.ReadInt32() - 1;

            sb.Append(tmp);
        } while (offset != -1);

        reader.Position = curPos;
        return sb.ToString().Reverse().ToString()!;
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