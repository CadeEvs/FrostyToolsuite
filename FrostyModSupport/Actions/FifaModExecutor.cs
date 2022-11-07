using Frosty.Hash;
using FrostySdk;
using FrostySdk.BaseProfile;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using FrostySdk.Managers.Entries;

namespace Frosty.ModSupport
{
    public partial class FrostyModExecutor
    {
        private class FifaBundleAction
        {
            private class BundleFileEntry
            {
                public int CasIndex;
                public int Offset;
                public int Size;

                public BundleFileEntry(int inCasIndex, int inOffset, int inSize)
                {
                    CasIndex = inCasIndex;
                    Offset = inOffset;
                    Size = inSize;
                }
            }

            private static readonly object Locker = new object();
            public static int CasFileCount = 0;

            public CatalogInfo CatalogInfo { get; }
            public Dictionary<int, string> CasFiles { get; } = new Dictionary<int, string>();

            public bool HasErrored => Exception != null;
            public Exception Exception { get; private set; }

            private ManualResetEvent doneEvent;
            private FrostyModExecutor parent;
            private CancellationToken cancelToken;

            public FifaBundleAction(CatalogInfo inCatalogInfo, ManualResetEvent inDoneEvent, FrostyModExecutor inParent, CancellationToken inCancelToken)
            {
                CatalogInfo = inCatalogInfo;
                parent = inParent;
                doneEvent = inDoneEvent;
                cancelToken = inCancelToken;
            }

            private void Run()
            {
                try
                {
                    NativeWriter casWriter = null;
                    int casFileIndex = 0;

                    byte[] key = KeyManager.Instance.GetKey("Key2");
                    foreach (string sbName in CatalogInfo.SuperBundles.Keys)
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        string sbPath = sbName;
                        if (CatalogInfo.SuperBundles[sbName].Item2)
                            sbPath = sbName.Replace("win32", CatalogInfo.Name);

                        string tocPath = parent.fs.ResolvePath(string.Format("{0}.toc", sbPath)).ToLower();
                        if (tocPath != "")
                        {
                            uint bundlesOffset = 0;
                            uint chunksOffset = 0;
                            byte[] buffer = null;

                            using (NativeReader reader = new NativeReader(new FileStream(tocPath, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                            {
                                uint magic = reader.ReadUInt();
                                bundlesOffset = reader.ReadUInt();
                                chunksOffset = reader.ReadUInt();
                                buffer = reader.ReadToEnd();

                                if (magic == 0xC3E5D5C3)
                                {
                                    using (Aes aes = Aes.Create())
                                    {
                                        aes.Key = key;
                                        aes.IV = key;
                                        aes.Padding = PaddingMode.None;

                                        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                                        using (MemoryStream decryptStream = new MemoryStream(buffer))
                                        {
                                            using (CryptoStream cryptoStream = new CryptoStream(decryptStream, decryptor, CryptoStreamMode.Read))
                                                cryptoStream.Read(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }

                            string newTocPath = tocPath.Replace("patch\\win32", parent.modDirName.ToLower() + "\\patch\\win32");
                            FileInfo tocFi = new FileInfo(newTocPath);
                            if (!Directory.Exists(tocFi.DirectoryName))
                                Directory.CreateDirectory(tocFi.DirectoryName);

                            using (NativeWriter tocWriter = new NativeWriter(new FileStream(newTocPath, FileMode.Create)))
                            {
                                tocWriter.Write(0x01CED100);
                                tocWriter.Write(new byte[0x228]);
                                long dataPos = tocWriter.Position;
                                tocWriter.Write(0xC3889333);

                                long bundlePos = 0xFFFFFFFF;
                                long chunkPos = 0xFFFFFFFF;

                                if (buffer.Length != 0)
                                {
                                    tocWriter.Write(0xDEADBEEF);
                                    tocWriter.Write(0xDEADBEEF);

                                    using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
                                    {
                                        if (bundlesOffset != 0xFFFFFFFF)
                                        {
                                            reader.Position = bundlesOffset - 0x0C;
                                            int bundleCount = reader.ReadInt();

                                            List<int> unkList = new List<int>();
                                            for (int i = 0; i < bundleCount; i++)
                                                unkList.Add(reader.ReadInt());

                                            List<int> bundleOffsets = new List<int>();
                                            for (int i = 0; i < bundleCount; i++)
                                            {
                                                cancelToken.ThrowIfCancellationRequested();
                                                int offset = reader.ReadInt() - 0x0C;

                                                long pos = reader.Position;
                                                reader.Position = offset;

                                                int bundleNameOffset = reader.ReadInt() - 1;
                                                List<BundleFileEntry> files = new List<BundleFileEntry>();

                                                while (true)
                                                {
                                                    int fileIndex = reader.ReadInt();
                                                    int fileOffset = reader.ReadInt();
                                                    int fileSize = reader.ReadInt();

                                                    files.Add(new BundleFileEntry(fileIndex & 0x7FFFFFFF, fileOffset, fileSize));
                                                    if ((fileIndex & 0x80000000) == 0)
                                                        break;
                                                }

                                                reader.Position = bundleNameOffset - 0x0C;

                                                int off = 0;
                                                string bundleName = "";

                                                do
                                                {
                                                    string tmp = reader.ReadNullTerminatedString();
                                                    off = reader.ReadInt() - 1;

                                                    bundleName = Utils.ReverseString(tmp) + bundleName;
                                                    if (off != -1)
                                                        reader.Position = off - 0x0C;

                                                } while (off != -1);

                                                reader.Position = pos;

                                                int bundleNameHash = Fnv1.HashString(bundleName.ToLower());
                                                if (parent.modifiedBundles.ContainsKey(bundleNameHash))
                                                {
                                                    ModBundleInfo modBundle = parent.modifiedBundles[bundleNameHash];
                                                    MemoryStream ms = new MemoryStream();

                                                    foreach (BundleFileEntry entry in files)
                                                    {
                                                        using (NativeReader casReader = new NativeReader(new FileStream(parent.fs.ResolvePath(parent.fs.GetFilePath(entry.CasIndex)), FileMode.Open, FileAccess.Read)))
                                                        {
                                                            casReader.Position = entry.Offset;
                                                            ms.Write(casReader.ReadBytes(entry.Size), 0, entry.Size);
                                                        }
                                                    }

                                                    DbObject bundle = null;
                                                    using (BinarySbReader sbReader = new BinarySbReader(ms, parent.fs.CreateDeobfuscator()))
                                                    {
                                                        bundle = sbReader.ReadDbObject();
                                                        foreach (DbObject ebx in bundle.GetValue<DbObject>("ebx"))
                                                        {
                                                            int size = ebx.GetValue<int>("size");
                                                            long dataOffset = ebx.GetValue<long>("offset");

                                                            long totalSize = 0;
                                                            foreach (BundleFileEntry file in files)
                                                            {
                                                                if (dataOffset < (totalSize + file.Size))
                                                                {
                                                                    dataOffset -= totalSize;
                                                                    dataOffset += file.Offset;
                                                                    ebx.SetValue("offset", dataOffset);
                                                                    ebx.SetValue("cas", file.CasIndex);
                                                                    break;
                                                                }
                                                                totalSize += file.Size;
                                                            }
                                                        }
                                                        foreach (DbObject res in bundle.GetValue<DbObject>("res"))
                                                        {
                                                            int size = res.GetValue<int>("size");
                                                            long dataOffset = res.GetValue<long>("offset");

                                                            long totalSize = 0;
                                                            foreach (BundleFileEntry file in files)
                                                            {
                                                                if (dataOffset < (totalSize + file.Size))
                                                                {
                                                                    dataOffset -= totalSize;
                                                                    dataOffset += file.Offset;
                                                                    res.SetValue("offset", dataOffset);
                                                                    res.SetValue("cas", file.CasIndex);
                                                                    break;
                                                                }
                                                                totalSize += file.Size;
                                                            }
                                                        }
                                                        foreach (DbObject chunk in bundle.GetValue<DbObject>("chunks"))
                                                        {
                                                            int size = chunk.GetValue<int>("size");
                                                            long dataOffset = chunk.GetValue<long>("offset");

                                                            long totalSize = 0;
                                                            foreach (BundleFileEntry file in files)
                                                            {
                                                                if (dataOffset < (totalSize + file.Size))
                                                                {
                                                                    dataOffset -= totalSize;
                                                                    dataOffset += file.Offset;
                                                                    chunk.SetValue("offset", dataOffset);
                                                                    chunk.SetValue("cas", file.CasIndex);
                                                                    break;
                                                                }
                                                                totalSize += file.Size;
                                                            }
                                                        }
                                                    }

                                                    foreach (DbObject ebx in bundle.GetValue<DbObject>("ebx"))
                                                    {
                                                        int idx = modBundle.Modify.Ebx.FindIndex((string a) => a.Equals(ebx.GetValue<string>("name")));
                                                        if (idx != -1)
                                                        {
                                                            EbxAssetEntry entry = parent.modifiedEbx[modBundle.Modify.Ebx[idx]];

                                                            // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                            if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                            {
                                                                casWriter?.Close();
                                                                casWriter = GetNextCas(out casFileIndex);
                                                            }

                                                            ebx.SetValue("originalSize", entry.OriginalSize);
                                                            ebx.SetValue("size", entry.Size);
                                                            ebx.SetValue("cas", casFileIndex);
                                                            ebx.SetValue("offset", (int)casWriter.Position);

                                                            casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                        }
                                                    }
                                                    foreach (string name in modBundle.Add.Ebx)
                                                    {
                                                        EbxAssetEntry entry = parent.modifiedEbx[name];

                                                        // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                        if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                        {
                                                            casWriter?.Close();
                                                            casWriter = GetNextCas(out casFileIndex);
                                                        }

                                                        DbObject ebx = DbObject.CreateObject();
                                                        ebx.SetValue("name", entry.Name);
                                                        ebx.SetValue("originalSize", entry.OriginalSize);
                                                        ebx.SetValue("size", entry.Size);
                                                        ebx.SetValue("cas", casFileIndex);
                                                        ebx.SetValue("offset", (int)casWriter.Position);
                                                        bundle.GetValue<DbObject>("ebx").Add(ebx);

                                                        casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                    }
                                                    foreach (DbObject res in bundle.GetValue<DbObject>("res"))
                                                    {
                                                        int idx = modBundle.Modify.Res.FindIndex((string a) => a.Equals(res.GetValue<string>("name")));
                                                        if (idx != -1)
                                                        {
                                                            ResAssetEntry entry = parent.modifiedRes[modBundle.Modify.Res[idx]];

                                                            // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                            if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                            {
                                                                casWriter?.Close();
                                                                casWriter = GetNextCas(out casFileIndex);
                                                            }

                                                            res.SetValue("originalSize", entry.OriginalSize);
                                                            res.SetValue("size", entry.Size);
                                                            res.SetValue("cas", casFileIndex);
                                                            res.SetValue("offset", (int)casWriter.Position);
                                                            res.SetValue("resRid", (long)entry.ResRid);
                                                            res.SetValue("resMeta", entry.ResMeta);
                                                            res.SetValue("resType", entry.ResType);

                                                            casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                        }
                                                    }
                                                    foreach (string name in modBundle.Add.Res)
                                                    {
                                                        ResAssetEntry entry = parent.modifiedRes[name];

                                                        // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                        if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                        {
                                                            casWriter?.Close();
                                                            casWriter = GetNextCas(out casFileIndex);
                                                        }

                                                        DbObject res = DbObject.CreateObject();
                                                        res.SetValue("name", entry.Name);
                                                        res.SetValue("originalSize", entry.OriginalSize);
                                                        res.SetValue("size", entry.Size);
                                                        res.SetValue("cas", casFileIndex);
                                                        res.SetValue("offset", (int)casWriter.Position);
                                                        res.SetValue("resRid", (long)entry.ResRid);
                                                        res.SetValue("resMeta", entry.ResMeta);
                                                        res.SetValue("resType", entry.ResType);
                                                        bundle.GetValue<DbObject>("res").Add(res);

                                                        casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                    }
                                                    foreach (DbObject chunk in bundle.GetValue<DbObject>("chunks"))
                                                    {
                                                        int idx = modBundle.Modify.Chunks.FindIndex((Guid a) => a == chunk.GetValue<Guid>("id"));
                                                        if (idx != -1)
                                                        {
                                                            ChunkAssetEntry entry = parent.modifiedChunks[modBundle.Modify.Chunks[idx]];

                                                            // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                            if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                            {
                                                                casWriter?.Close();
                                                                casWriter = GetNextCas(out casFileIndex);
                                                            }

                                                            chunk.SetValue("originalSize", entry.OriginalSize);
                                                            chunk.SetValue("size", entry.Size);
                                                            chunk.SetValue("cas", casFileIndex);
                                                            chunk.SetValue("offset", (int)casWriter.Position);
                                                            chunk.SetValue("logicalOffset", entry.LogicalOffset);
                                                            chunk.SetValue("logicalSize", entry.LogicalSize);

                                                            casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                        }
                                                    }
                                                    foreach (Guid guid in modBundle.Add.Chunks)
                                                    {
                                                        ChunkAssetEntry entry = parent.modifiedChunks[guid];

                                                        // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                        if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                        {
                                                            casWriter?.Close();
                                                            casWriter = GetNextCas(out casFileIndex);
                                                        }

                                                        DbObject chunk = DbObject.CreateObject();
                                                        chunk.SetValue("id", entry.Id);
                                                        chunk.SetValue("originalSize", entry.OriginalSize);
                                                        chunk.SetValue("size", entry.Size);
                                                        chunk.SetValue("cas", casFileIndex);
                                                        chunk.SetValue("offset", (int)casWriter.Position);
                                                        chunk.SetValue("logicalOffset", entry.LogicalOffset);
                                                        chunk.SetValue("logicalSize", entry.LogicalSize);
                                                        bundle.GetValue<DbObject>("chunks").Add(chunk);

                                                        DbObject chunkMeta = DbObject.CreateObject();
                                                        chunkMeta.SetValue("h32", entry.H32);
                                                        DbObject meta = DbObject.CreateObject();
                                                        if (entry.FirstMip != -1)
                                                            meta.SetValue("firstMip", entry.FirstMip);
                                                        bundle.GetValue<DbObject>("chunkMeta").Add(chunkMeta);

                                                        casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                    }

                                                    BundleFileEntry bundleFile = files[0];
                                                    files.Clear();

                                                    files.Add(bundleFile);
                                                    foreach (DbObject ebx in bundle.GetValue<DbObject>("ebx"))
                                                        files.Add(new BundleFileEntry(ebx.GetValue<int>("cas"), ebx.GetValue<int>("offset"), ebx.GetValue<int>("size")));
                                                    foreach (DbObject res in bundle.GetValue<DbObject>("res"))
                                                        files.Add(new BundleFileEntry(res.GetValue<int>("cas"), res.GetValue<int>("offset"), res.GetValue<int>("size")));
                                                    foreach (DbObject chunk in bundle.GetValue<DbObject>("chunks"))
                                                        files.Add(new BundleFileEntry(chunk.GetValue<int>("cas"), chunk.GetValue<int>("offset"), chunk.GetValue<int>("size")));

                                                    // now write out new patched binary bundle
                                                    using (BinarySbWriter writer = new BinarySbWriter(new MemoryStream()))
                                                    {
                                                        writer.Write(bundle);

                                                        // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                        if (casWriter == null || casWriter.Length + writer.Length > 1073741824)
                                                        {
                                                            casWriter?.Close();
                                                            casWriter = GetNextCas(out casFileIndex);
                                                        }

                                                        bundleFile.CasIndex = casFileIndex;
                                                        bundleFile.Offset = (int)casWriter.Position;
                                                        bundleFile.Size = (int)(writer.Length);

                                                        casWriter.Write(((MemoryStream)(writer.BaseStream)).ToArray());
                                                    }
                                                }

                                                // write it
                                                bundleOffsets.Add((int)(tocWriter.Position - dataPos));
                                                tocWriter.Write((int)((tocWriter.Position - dataPos) + (files.Count * 3 * 4) + 5));

                                                for (int j = 0; j < files.Count; j++)
                                                {
                                                    uint fileIndex = (uint)files[j].CasIndex;
                                                    if (j != files.Count - 1)
                                                        fileIndex |= 0x80000000;

                                                    tocWriter.Write(fileIndex);
                                                    tocWriter.Write(files[j].Offset);
                                                    tocWriter.Write(files[j].Size);
                                                }

                                                tocWriter.WriteNullTerminatedString(new string(bundleName.Reverse().ToArray()));
                                                tocWriter.Write(0x00);

                                                int t = bundleName.Length + 5;
                                                for (int z = 0; z < 0x10 - (t % 0x10); z++)
                                                    tocWriter.Write((byte)0x00);
                                            }

                                            bundlePos = tocWriter.Position - dataPos;
                                            tocWriter.Write(bundleCount);
                                            foreach (int unk in unkList)
                                                tocWriter.Write(unk);
                                            foreach (int offset in bundleOffsets)
                                                tocWriter.Write(offset);
                                        }

                                        List<int> unkChunksList = new List<int>();
                                        List<uint> chunkOffsets = new List<uint>();
                                        //Dictionary<uint, uint> chunkOffsetMapping = new Dictionary<uint, uint>();
                                        List<Guid> chunkGuids = new List<Guid>();
                                        List<List<Tuple<Guid, int>>> buckets = new List<List<Tuple<Guid, int>>>();
                                        int numChunks = 0;
                                        //int oldNumChunks = 0;

                                        if (chunksOffset != 0xFFFFFFFF)
                                        {
                                            long startChunksPosNew = tocWriter.Position;
                                            long startChunksPosOld = reader.Position + 0x238;

                                            reader.Position = chunksOffset - 0x0c;

                                            numChunks = reader.ReadInt();
                                            //oldNumChunks = numChunks;

                                            for (int i = 0; i < numChunks; i++)
                                            {
                                                unkChunksList.Add(reader.ReadInt());
                                                buckets.Add(new List<Tuple<Guid, int>>());
                                            }

                                            //for (int i = 0; i < numChunks; i++)
                                            //    chunkOffsets.Add(reader.ReadUInt());

                                            //List<uint> tmp = chunkOffsets;
                                            //tmp.Sort();

                                            //reader.Position = 0;
                                            //for (int i = 0; i < numChunks; i++)
                                            //{
                                            //    reader.Position = chunkOffsets[i] - 0x0c;
                                            //    chunkOffsetMapping.Add(chunkOffsets[i], (uint)(tocWriter.Position - dataPos));

                                            //    Guid guid = reader.ReadGuid();
                                            //    int fileIndex = reader.ReadInt();
                                            //    int dataOffset = reader.ReadInt();
                                            //    int dataSize = reader.ReadInt();

                                            //    if (parent.modifiedBundles.ContainsKey(chunksBundleHash))
                                            //    {
                                            //        ModBundleInfo bundleInfo = parent.modifiedBundles[chunksBundleHash];
                                            //        int idx = bundleInfo.Modify.Chunks.FindIndex((Guid g) => g == guid);
                                            //        if (idx != -1)
                                            //        {
                                            //            ChunkAssetEntry entry = parent.modifiedChunks[bundleInfo.Modify.Chunks[idx]];
                                            //            byte[] data = null;

                                            //            if (entry.ExtraData != null)
                                            //            {
                                            //                // invoke custom handler to modify the base data with the custom data
                                            //                HandlerExtraData extraData = (HandlerExtraData)entry.ExtraData;
                                            //                Stream baseData = parent.rm.GetResourceData(parent.fs.GetFilePath(fileIndex), dataOffset, dataSize);
                                            //                entry = (ChunkAssetEntry)extraData.Handler.Modify(baseData, extraData.Data, out data);
                                            //            }
                                            //            else
                                            //                data = parent.archiveData[entry.Sha1].Data;

                                            //            // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                            //            if (casWriter == null || casWriter.Length + data.Length > 1073741824)
                                            //            {
                                            //                if (casWriter != null)
                                            //                    casWriter.Close();
                                            //                casWriter = GetNextCas(out casFileIndex);
                                            //            }

                                            //            fileIndex = casFileIndex;
                                            //            dataOffset = (int)casWriter.Position;
                                            //            dataSize = (int)entry.Size;

                                            //            casWriter.Write(data);
                                            //        }
                                            //    }

                                            //    tocWriter.Write(guid);
                                            //    tocWriter.Write(fileIndex);
                                            //    tocWriter.Write(dataOffset);
                                            //    tocWriter.Write(dataSize);

                                            //    chunkGuids.Add(guid);
                                            //}

                                            for (int i = 0; i < numChunks; i++)
                                            {
                                                cancelToken.ThrowIfCancellationRequested();
                                                uint chunkOffset = reader.ReadUInt();

                                                long oldPos = reader.Position;
                                                reader.Position = chunkOffset - 0x0c;

                                                Guid guid = reader.ReadGuid();
                                                int fileIndex = reader.ReadInt();
                                                int dataOffset = reader.ReadInt();
                                                int dataSize = reader.ReadInt();

                                                reader.Position = oldPos;
                                                chunkOffsets.Add((uint)(tocWriter.Position - dataPos));

                                                if (parent.modifiedBundles.ContainsKey(chunksBundleHash))
                                                {
                                                    ModBundleInfo bundleInfo = parent.modifiedBundles[chunksBundleHash];
                                                    int idx = bundleInfo.Modify.Chunks.FindIndex((Guid g) => g == guid);
                                                    if (idx != -1)
                                                    {
                                                        ChunkAssetEntry entry = parent.modifiedChunks[bundleInfo.Modify.Chunks[idx]];
                                                        byte[] data = null;

                                                        //if (entry.ExtraData != null)
                                                        //{
                                                        //    // invoke custom handler to modify the base data with the custom data
                                                        //    HandlerExtraData extraData = (HandlerExtraData)entry.ExtraData;
                                                        //    Stream baseData = parent.rm.GetResourceData(parent.fs.GetFilePath(fileIndex), dataOffset, dataSize);
                                                        //    entry = (ChunkAssetEntry)extraData.Handler.Modify(entry, baseData, null, null, extraData.Data, out data);
                                                        //}
                                                        //else
                                                        data = parent.archiveData[entry.Sha1].Data;

                                                        // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                        if (casWriter == null || casWriter.Length + data.Length > 1073741824)
                                                        {
                                                            casWriter?.Close();
                                                            casWriter = GetNextCas(out casFileIndex);
                                                        }

                                                        fileIndex = casFileIndex;
                                                        dataOffset = (int)casWriter.Position;
                                                        dataSize = (int)entry.Size;

                                                        casWriter.Write(data);
                                                    }
                                                }

                                                tocWriter.Write(guid);
                                                tocWriter.Write(fileIndex);
                                                tocWriter.Write(dataOffset);
                                                tocWriter.Write(dataSize);

                                                chunkGuids.Add(guid);
                                            }
                                        }
                                        if (parent.modifiedBundles.ContainsKey(chunksBundleHash) && tocPath.ToLower().Contains("globals.toc"))
                                        {
                                            foreach (Guid guid in parent.modifiedBundles[chunksBundleHash].Add.Chunks)
                                            {
                                                ChunkAssetEntry entry = parent.modifiedChunks[guid];

                                                // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                if (casWriter == null || casWriter.Length + parent.archiveData[entry.Sha1].Data.Length > 1073741824)
                                                {
                                                    casWriter?.Close();
                                                    casWriter = GetNextCas(out casFileIndex);
                                                }

                                                int fileIndex = casFileIndex;
                                                int dataOffset = (int)casWriter.Position;
                                                int dataSize = (int)entry.Size;

                                                chunkOffsets.Add((uint)(tocWriter.Position - dataPos));

                                                buckets.Add(new List<Tuple<Guid, int>>());
                                                unkChunksList.Add(-1);
                                                numChunks++;

                                                chunkGuids.Add(guid);

                                                casWriter.Write(parent.archiveData[entry.Sha1].Data);
                                                tocWriter.Write(guid);
                                                tocWriter.Write(fileIndex);
                                                tocWriter.Write(dataOffset);
                                                tocWriter.Write(dataSize);
                                            }
                                        }
                                        if (numChunks > 0)
                                        {
                                            //if (oldNumChunks != numChunks)
                                            {
                                                chunkPos = tocWriter.Position - dataPos;

                                                int l = 0;
                                                List<int> slots = new List<int>();
                                                for (int i = 0; i < numChunks; i++)
                                                {
                                                    slots.Add(-1);
                                                    unkChunksList[i] = -1;

                                                    uint chunkNameHash = HashData(chunkGuids[i].ToByteArray());
                                                    int cidx = (int)((uint)((int)chunkNameHash % 0x1000193) % numChunks);
                                                    buckets[cidx].Add(new Tuple<Guid, int>(chunkGuids[i], (int)chunkOffsets[i]));
                                                }
                                                for (int z = 0; z < buckets.Count; z++)
                                                {
                                                    List<Tuple<Guid, int>> entries = buckets[z];
                                                    if (entries.Count > 1)
                                                    {
                                                        uint j = 1;
                                                        List<int> ids = new List<int>();

                                                        while (true)
                                                        {
                                                            bool done = true;
                                                            for (int k = 0; k < entries.Count; k++)
                                                            {
                                                                uint hash = HashData(entries[k].Item1.ToByteArray(), j);
                                                                int idx = (int)((uint)((int)hash % 0x1000193) % numChunks);
                                                                if (slots[idx] != -1 || ids.Contains(idx))
                                                                {
                                                                    done = false;
                                                                    break;
                                                                }
                                                                ids.Add(idx);
                                                            }
                                                            if (done)
                                                                break;
                                                            j++;
                                                            ids.Clear();
                                                        }
                                                        for (int k = 0; k < entries.Count; k++)
                                                        {
                                                            slots[ids[k]] = entries[k].Item2;
                                                        }
                                                        unkChunksList[z] = (int)j;
                                                    }
                                                }
                                                for (int z = 0; z < buckets.Count; z++)
                                                {
                                                    if (buckets[z].Count == 1)
                                                    {
                                                        while (slots[l] != -1)
                                                            l++;
                                                        unkChunksList[z] = -1 - l;
                                                        slots[l] = buckets[z][0].Item2;
                                                    }
                                                }

                                                tocWriter.Write(numChunks);
                                                for (int i = 0; i < numChunks; i++)
                                                    tocWriter.Write(unkChunksList[i]);
                                                for (int i = 0; i < numChunks; i++)
                                                    tocWriter.Write(slots[i]);
                                            }
                                            //else
                                            //{
                                            //    chunkPos = tocWriter.Position - dataPos;

                                            //    tocWriter.Write(numChunks);
                                            //    for (int i = 0; i < numChunks; i++)
                                            //        tocWriter.Write(unkChunksList[i]);
                                            //    for (int i = 0; i < numChunks; i++)
                                            //        tocWriter.Write(chunkOffsetMapping[chunkOffsets[i]]);
                                            //}
                                        }

                                        tocWriter.Position = dataPos + 4;
                                        tocWriter.Write((int)bundlePos);
                                        tocWriter.Write((int)chunkPos);
                                    }
                                }
                                else
                                {
                                    tocWriter.Write(0xFFFFFFFF);
                                    tocWriter.Write(0xFFFFFFFF);
                                }
                            }
                        }
                    }

                    casWriter?.Close();
                }
                catch (Exception e)
                {
                    Exception = e;
                }
            }

            private NativeWriter GetNextCas(out int casFileIndex)
            {
                int casIndex = 1;
                string casPath = parent.fs.BasePath + parent.modDirName + "\\patch\\" + CatalogInfo.Name + "\\cas_" + casIndex.ToString("D2") + ".cas";
                while (File.Exists(casPath))
                {
                    casIndex++;
                    casPath = parent.fs.BasePath + parent.modDirName + "\\patch\\" + CatalogInfo.Name + "\\cas_" + casIndex.ToString("D2") + ".cas";
                }

                lock (Locker)
                {
                    CasFiles.Add(++CasFileCount, "/native_data/Patch/" + CatalogInfo.Name + "/cas_" + casIndex.ToString("D2") + ".cas");
                    casFileIndex = CasFileCount;
                }

                FileInfo casFi = new FileInfo(casPath);
                if (!Directory.Exists(casFi.DirectoryName))
                    Directory.CreateDirectory(casFi.DirectoryName);

                return new NativeWriter(new FileStream(casPath, FileMode.Create));
            }

            private uint HashString(string strToHash, uint initial = 0x00)
            {
                uint hash = 0x811C9DC5;
                if (initial != 0x00)
                    hash = initial;

                for (int i = 0; i < strToHash.Length; i++)
                {
                    uint B = (uint)strToHash[i];
                    hash = B ^ 0x1000193 * hash;
                }

                return hash;
            }

            static uint HashData(byte[] b, uint initial = 0x00)
            {
                uint hash = (uint)((sbyte)b[0] ^ 0x50C5D1F);
                int start = 1;
                if (initial != 0x00)
                {
                    hash = initial;
                    start = 0;
                }

                for (int i = start; i < b.Length; i++)
                {
                    uint B = (uint)((sbyte)b[i]);
                    hash = B ^ 0x1000193 * hash;
                }

                return hash;
            }

            public void ThreadPoolCallback(object threadContext)
            {
                Run();

                // are all threads done?
                if (Interlocked.Decrement(ref parent.numTasks) == 0)
                    doneEvent.Set();
            }
        }
    }
}
