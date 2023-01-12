using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FrostySdk.Managers.Entries;

namespace Frosty.ModSupport
{
    public partial class FrostyModExecutor
    {
        private class HeatBundleAction
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

            private class BundleInfo
            {
                public string Name => parent.GetString(NameOffset);
                public int NameHash => Fnv1.HashString(Name.ToLower());

                public uint NameOffset;
                public long Offset;
                public uint Size;

                private TocFile parent;
                public BundleInfo(TocFile inParent, uint nameOffset, uint size, long offset)
                {
                    parent = inParent;
                    NameOffset = nameOffset;
                    Offset = offset;
                    Size = size;
                }
            }
            private class ChunkGuidInfo
            {
                public Guid Guid;
                public int Index;
            }
            private class ChunkInfo
            {
                public Guid Guid;
                public byte Unknown;
                public bool IsPatch;
                public byte CatalogIndex;
                public byte CasIndex;
                public uint Offset;
                public uint Size;
            }
            private class TocFile
            {
                public IEnumerable<BundleInfo> Bundles
                {
                    get
                    {
                        for (int i = 0; i < bundles.Count; i++)
                            yield return bundles[i];
                    }
                }
                public IEnumerable<ChunkInfo> Chunks
                {
                    get
                    {
                        for (int i = 0; i < chunks.Count; i++)
                            yield return chunks[i];
                    }
                }

                private int bundleHashMapOffset;
                private int bundleDataOffset;
                private int bundlesCount;
                private int chunkHashMapOffset;
                private int chunkGuidOffset;
                private int chunksCount;
                private int chunkDataOffset;
                private int stringsOffset;
                private int unknownOffset1;
                private int unknownOffset2;
                private int unknownCount;
                private int unknownOffset3;

                private List<int> bundleHashMap = new List<int>();
                private List<int> chunkHashMap = new List<int>();
                private List<BundleInfo> bundles = new List<BundleInfo>();
                private List<ChunkGuidInfo> chunkGuids = new List<ChunkGuidInfo>();
                private Dictionary<Guid, int> chunkGuidMapping = new Dictionary<Guid, int>();
                private List<ChunkInfo> chunks = new List<ChunkInfo>();
                private Dictionary<uint, string> strings = new Dictionary<uint, string>();

                public TocFile(string tocPath, FrostyModExecutor parent)
                {
                    using (NativeReader reader = new NativeReader(new FileStream(tocPath, FileMode.Open, FileAccess.Read), parent.m_fs.CreateDeobfuscator()))
                    {
                        int startPos = (int)reader.Position;
                        bundleHashMapOffset = reader.ReadInt(Endian.Big) + startPos;
                        bundleDataOffset = reader.ReadInt(Endian.Big) + startPos;
                        bundlesCount = reader.ReadInt(Endian.Big);
                        chunkHashMapOffset = reader.ReadInt(Endian.Big) + startPos;
                        chunkGuidOffset = reader.ReadInt(Endian.Big) + startPos;
                        chunksCount = reader.ReadInt(Endian.Big);
                        chunkDataOffset = reader.ReadInt(Endian.Big) + startPos;
                        unknownOffset1 = reader.ReadInt(Endian.Big) + startPos;
                        stringsOffset = reader.ReadInt(Endian.Big) + startPos;
                        unknownOffset2 = reader.ReadInt(Endian.Big) + startPos;
                        unknownCount = reader.ReadInt(Endian.Big);
                        unknownOffset3 = reader.ReadInt(Endian.Big) + startPos;

                        if (bundlesCount != 0)
                        {
                            reader.Position = bundleHashMapOffset;
                            for (int i = 0; i < bundlesCount; i++)
                                bundleHashMap.Add(reader.ReadInt(Endian.Big));

                            reader.Position = bundleDataOffset;
                            for (int i = 0; i < bundlesCount; i++)
                            {
                                BundleInfo bi = new BundleInfo(this, reader.ReadUInt(Endian.Big), reader.ReadUInt(Endian.Big), reader.ReadLong(Endian.Big));
                                bundles.Add(bi);
                            }
                        }
                        if (chunksCount != 0)
                        {
                            reader.Position = chunkHashMapOffset;
                            for (int i = 0; i < chunksCount; i++)
                                chunkHashMap.Add(reader.ReadInt(Endian.Big));

                            reader.Position = chunkGuidOffset;
                            List<Guid> guids = new List<Guid>(chunksCount);

                            for (int i = 0; i < chunksCount; i++)
                            {
                                ChunkGuidInfo cgi = new ChunkGuidInfo();

                                byte[] b = reader.ReadBytes(16);
                                Guid guid = new Guid(new byte[]
                                {
                                    b[15], b[14], b[13], b[12],
                                    b[11], b[10], b[9], b[8],
                                    b[7], b[6], b[5], b[4],
                                    b[3], b[2], b[1], b[0]
                                });

                                cgi.Guid = guid;
                                cgi.Index = reader.ReadInt(Endian.Big);

                                chunkGuids.Add(cgi);

                                int index = (int)((cgi.Index & 0xFFFFFF) / 3);
                                while (guids.Count <= index)
                                    guids.Add(Guid.Empty);

                                guids[index] = cgi.Guid;
                            }

                            reader.Position = chunkDataOffset;
                            for (int i = 0; i < chunksCount; i++)
                            {
                                ChunkInfo ci = new ChunkInfo()
                                {
                                    Guid = guids[i],
                                    Unknown = reader.ReadByte(),
                                    IsPatch = reader.ReadBoolean(),
                                    CatalogIndex = reader.ReadByte(),
                                    CasIndex = reader.ReadByte(),
                                    Offset = reader.ReadUInt(Endian.Big),
                                    Size = reader.ReadUInt(Endian.Big)
                                };
                                chunks.Add(ci);
                            }
                        }

                        reader.Position = stringsOffset;
                        while (reader.Position < reader.Length)
                            strings.Add((uint)(reader.Position - stringsOffset), reader.ReadNullTerminatedString());
                    }
                }

                public void Write(string newPath)
                {
                    FileInfo fi = new FileInfo(newPath);
                    if (!Directory.Exists(fi.DirectoryName))
                        Directory.CreateDirectory(fi.DirectoryName);

                    using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
                    {
                        writer.Write(0x01CED100);
                        writer.Position += 0x228;

                        writer.Write(bundleHashMapOffset - 0x22c, Endian.Big);
                        writer.Write(bundleDataOffset - 0x22c, Endian.Big);
                        writer.Write(bundlesCount, Endian.Big);
                        writer.Write(chunkHashMapOffset - 0x22c, Endian.Big);
                        writer.Write(chunkGuidOffset - 0x22c, Endian.Big);
                        writer.Write(chunksCount, Endian.Big);
                        writer.Write(chunkDataOffset - 0x22c, Endian.Big);
                        writer.Write(unknownOffset1 - 0x22c, Endian.Big);
                        writer.Write(stringsOffset - 0x22c, Endian.Big);
                        writer.Write(unknownOffset2 - 0x22c, Endian.Big);
                        writer.Write(unknownCount, Endian.Big);
                        writer.Write(unknownOffset3 - 0x22c, Endian.Big);

                        for (int i = 0; i < bundleHashMap.Count; i++)
                            writer.Write(bundleHashMap[i], Endian.Big);

                        while ((writer.Position - 0x22c) % 0x08 != 0)
                            writer.Write((byte)0x00);

                        for (int i = 0; i < bundles.Count; i++)
                        {
                            writer.Write(bundles[i].NameOffset, Endian.Big);
                            writer.Write(bundles[i].Size, Endian.Big);
                            writer.Write(bundles[i].Offset, Endian.Big);
                        }

                        for (int i = 0; i < chunkHashMap.Count; i++)
                            writer.Write(chunkHashMap[i], Endian.Big);

                        for (int i = 0; i < chunkGuids.Count; i++)
                        {
                            byte[] barray = chunkGuids[i].Guid.ToByteArray();
                            foreach (byte b in barray.Reverse())
                                writer.Write(b);
                            writer.Write(chunkGuids[i].Index, Endian.Big);
                        }

                        for (int i = 0; i < chunks.Count; i++)
                        {
                            writer.Write(chunks[i].Unknown);
                            writer.Write(chunks[i].IsPatch ? (byte)0x01 : (byte)0x00);
                            writer.Write(chunks[i].CatalogIndex);
                            writer.Write(chunks[i].CasIndex);
                            writer.Write(chunks[i].Offset, Endian.Big);
                            writer.Write(chunks[i].Size, Endian.Big);
                        }

                        foreach (uint offset in strings.Keys)
                        {
                            writer.WriteNullTerminatedString(strings[offset]);
                        }
                    }
                }

                public bool ContainsModifiedBundles(IEnumerable<int> bundleHashes)
                {
                    foreach (BundleInfo bi in bundles)
                    {
                        if (bundleHashes.Contains(bi.NameHash))
                            return true;
                    }
                    return false;
                }

                public string GetString(uint offset)
                {
                    return strings[offset];
                }
            }

            public static Dictionary<string, int> CasFiles = new Dictionary<string, int>();
            private static readonly object Locker = new object();

            public string SuperBundle { get; }
            public bool TocModified { get; private set; }
            public bool SbModified { get; private set; }

            public bool HasErrored => Exception != null;
            public Exception Exception { get; private set; }

            private ManualResetEvent doneEvent;
            private FrostyModExecutor parent;
            private string catalog;

            public HeatBundleAction(string inSuperBundle, ManualResetEvent inDoneEvent, FrostyModExecutor inParent)
            {
                SuperBundle = inSuperBundle;
                parent = inParent;
                doneEvent = inDoneEvent;
                catalog = parent.m_fs.GetCatalogFromSuperBundle(SuperBundle);
            }

            private void Run()
            {
                try
                {
                    string tocPath = parent.m_fs.ResolvePath(SuperBundle + ".toc");
                    string newTocPath = tocPath.Replace("patch\\win32", parent.m_modDirName.ToLower() + "\\patch\\win32");

                    TocFile toc = new TocFile(parent.m_fs.ResolvePath(SuperBundle + ".toc"), parent);
                    NativeWriter casWriter = null;
                    int casFileIndex = 0;

                    string sbPath = parent.m_fs.ResolvePath(SuperBundle + ".sb");
                    string newSbPath = sbPath.Replace("patch\\win32", parent.m_modDirName.ToLower() + "\\patch\\win32");

                    FileInfo sbFi = new FileInfo(newSbPath);
                    if (!Directory.Exists(sbFi.DirectoryName))
                        Directory.CreateDirectory(sbFi.DirectoryName);

                    using (NativeReader reader = new NativeReader(new FileStream(sbPath, FileMode.Open, FileAccess.Read), parent.m_fs.CreateDeobfuscator()))
                    {
                        if (toc.ContainsModifiedBundles(parent.m_modifiedBundles.Keys))
                        {
                            TocModified = true;
                            SbModified = true;

                            using (NativeWriter writer = new NativeWriter(new FileStream(sbFi.FullName, FileMode.Create)))
                            {
                                foreach (var bundle in toc.Bundles)
                                {
                                    if (parent.m_modifiedBundles.ContainsKey(bundle.NameHash))
                                    {
                                        ModBundleInfo modBundle = parent.m_modifiedBundles[bundle.NameHash];

                                        reader.Position = bundle.Offset;
                                        DbObject bundleObj = ReadBundle(reader);

                                        foreach (DbObject ebx in bundleObj.GetValue<DbObject>("ebx"))
                                        {
                                            int idx = modBundle.Modify.Ebx.FindIndex((string a) => a.Equals(ebx.GetValue<string>("name")));
                                            if (idx != -1)
                                            {
                                                EbxAssetEntry entry = parent.m_modifiedEbx[modBundle.Modify.Ebx[idx]];

                                                // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                if (casWriter == null || casWriter.Length + parent.m_archiveData[entry.Sha1].Data.Length > 1073741824)
                                                {
                                                    casWriter?.Close();
                                                    casWriter = GetNextCas(out casFileIndex);
                                                }

                                                ebx.SetValue("sha1", entry.Sha1);
                                                ebx.SetValue("originalSize", entry.OriginalSize);
                                                ebx.SetValue("size", entry.Size);
                                                ebx.SetValue("cas", casFileIndex);
                                                ebx.SetValue("offset", (int)casWriter.Position);
                                                ebx.SetValue("patch", true);

                                                casWriter.Write(parent.m_archiveData[entry.Sha1].Data);
                                            }
                                        }
                                        foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                                        {
                                            int idx = modBundle.Modify.Res.FindIndex((string a) => a.Equals(res.GetValue<string>("name")));
                                            if (idx != -1)
                                            {
                                                ResAssetEntry entry = parent.m_modifiedRes[modBundle.Modify.Res[idx]];

                                                // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                if (casWriter == null || casWriter.Length + parent.m_archiveData[entry.Sha1].Data.Length > 1073741824)
                                                {
                                                    casWriter?.Close();
                                                    casWriter = GetNextCas(out casFileIndex);
                                                }

                                                res.SetValue("sha1", entry.Sha1);
                                                res.SetValue("originalSize", entry.OriginalSize);
                                                res.SetValue("size", entry.Size);
                                                res.SetValue("cas", casFileIndex);
                                                res.SetValue("offset", (int)casWriter.Position);
                                                res.SetValue("resRid", (long)entry.ResRid);
                                                res.SetValue("resMeta", entry.ResMeta);
                                                res.SetValue("resType", entry.ResType);
                                                res.SetValue("patch", true);

                                                casWriter.Write(parent.m_archiveData[entry.Sha1].Data);
                                            }
                                        }
                                        foreach (DbObject chunk in bundleObj.GetValue<DbObject>("chunks"))
                                        {
                                            int idx = modBundle.Modify.Chunks.FindIndex((Guid a) => a == chunk.GetValue<Guid>("id"));
                                            if (idx != -1)
                                            {
                                                ChunkAssetEntry entry = parent.m_modifiedChunks[modBundle.Modify.Chunks[idx]];

                                                // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                                                if (casWriter == null || casWriter.Length + parent.m_archiveData[entry.Sha1].Data.Length > 1073741824)
                                                {
                                                    casWriter?.Close();
                                                    casWriter = GetNextCas(out casFileIndex);
                                                }

                                                chunk.SetValue("sha1", entry.Sha1);
                                                chunk.SetValue("originalSize", entry.OriginalSize);
                                                chunk.SetValue("size", entry.Size);
                                                chunk.SetValue("cas", casFileIndex);
                                                chunk.SetValue("offset", (int)casWriter.Position);
                                                chunk.SetValue("logicalOffset", entry.LogicalOffset);
                                                chunk.SetValue("logicalSize", entry.LogicalSize);
                                                chunk.SetValue("patch", true);

                                                casWriter.Write(parent.m_archiveData[entry.Sha1].Data);
                                            }
                                        }

                                        byte[] bundleData = WriteBundle(bundleObj);

                                        bundle.Offset = writer.Position;
                                        bundle.Size = (uint)bundleData.Length;

                                        writer.Write(bundleData);
                                        writer.WritePadding(0x04);
                                    }
                                    else
                                    {
                                        reader.Position = bundle.Offset;
                                        byte[] bundleData = reader.ReadBytes((int)bundle.Size);

                                        bundle.Offset = writer.Position;
                                        writer.Write(bundleData);
                                        writer.WritePadding(0x04);
                                    }
                                }
                            }
                        }

                        //if (parent.m_modifiedBundles.ContainsKey(s_chunksBundleHash))
                        //{
                        //    foreach (var chunk in toc.Chunks)
                        //    {
                        //        ModBundleInfo modBundle = parent.m_modifiedBundles[s_chunksBundleHash];
                        //        int idx = modBundle.Modify.Chunks.FindIndex((Guid a) => a == chunk.Guid);

                        //        if (idx != -1)
                        //        {
                        //            TocModified = true;
                        //            ChunkAssetEntry entry = parent.m_modifiedChunks[modBundle.Modify.Chunks[idx]];

                        //            // get next cas (if one hasnt been obtained or the current one will exceed 1gb)
                        //            if (casWriter == null || casWriter.Length + parent.m_archiveData[entry.Sha1].Data.Length > 1073741824)
                        //            {
                        //                casWriter?.Close();
                        //                casWriter = GetNextCas(out casFileIndex);
                        //            }

                        //            chunk.Offset = (uint)casWriter.Position;
                        //            chunk.Size = (uint)entry.Size;
                        //            chunk.CasIndex = (byte)casFileIndex;

                        //            casWriter.Write(parent.m_archiveData[entry.Sha1].Data);
                        //        }
                        //    }
                        //}
                    }

                    casWriter?.Close();

                    if (TocModified)
                        toc.Write(newTocPath);
                }
                catch (Exception e)
                {
                    Exception = e;
                }
            }

            public void ThreadPoolCallback(object threadContext)
            {
                Run();

                // are all threads done?
                if (Interlocked.Decrement(ref parent.m_numTasks) == 0)
                    doneEvent.Set();
            }

            private NativeWriter GetNextCas(out int casFileIndex)
            {
                lock (Locker)
                {
                    casFileIndex = CasFiles[catalog];
                    CasFiles[catalog]++;
                }

                FileInfo fi = new FileInfo(parent.m_fs.BasePath + parent.m_modDirName + "\\Patch\\" + catalog + "\\cas_" + casFileIndex.ToString("D2") + ".cas");
                if (!Directory.Exists(fi.DirectoryName))
                    Directory.CreateDirectory(fi.DirectoryName);

                return new NativeWriter(new FileStream(fi.FullName, FileMode.Create));
            }

            private DbObject ReadBundle(NativeReader bundleReader)
            {
                uint startPos = (uint)bundleReader.Position;

                uint headerSize = bundleReader.ReadUInt(Endian.Big);
                uint dataOffset = bundleReader.ReadUInt(Endian.Big);
                uint locationOffset = bundleReader.ReadUInt(Endian.Big) + startPos;
                byte[] unkBuf = bundleReader.ReadBytes(0x14);

                byte[] bundleBuf = bundleReader.ReadBytes((int)dataOffset);

                DbObject bundle = null;
                uint totalCount = 0;

                using (BinarySbReader reader = new BinarySbReader(new MemoryStream(bundleBuf), parent.m_fs.CreateDeobfuscator()))
                {
                    bundle = reader.ReadDbObject();
                    totalCount = reader.TotalCount;
                }

                bundleReader.Position = locationOffset;

                bool[] list = new bool[totalCount];
                for (uint i = 0; i < totalCount; i++)
                    list[i] = bundleReader.ReadBoolean();

                bundleReader.Position = dataOffset + startPos + 0x20;

                byte unk = 0;
                bool isPatch = false;
                int catalogIndex = 0;
                int casIndex = 0;
                int z = 0;

                for (int i = 0; i < bundle.GetValue<DbObject>("ebx").Count; i++)
                {
                    if (list[z++])
                    {
                        unk = bundleReader.ReadByte();
                        isPatch = bundleReader.ReadBoolean();
                        catalogIndex = bundleReader.ReadByte();
                        casIndex = bundleReader.ReadByte();
                    }

                    DbObject ebx = bundle.GetValue<DbObject>("ebx")[i] as DbObject;
                    int offset = bundleReader.ReadInt(Endian.Big);
                    int size = bundleReader.ReadInt(Endian.Big);

                    ebx.SetValue("catalog", catalogIndex);
                    ebx.SetValue("cas", casIndex);
                    ebx.SetValue("offset", offset);
                    ebx.SetValue("size", size);
                    ebx.SetValue("unk", unk);
                    if (isPatch)
                        ebx.SetValue("patch", true);
                }
                for (int i = 0; i < bundle.GetValue<DbObject>("res").Count; i++)
                {
                    if (list[z++])
                    {
                        unk = bundleReader.ReadByte();
                        isPatch = bundleReader.ReadBoolean();
                        catalogIndex = bundleReader.ReadByte();
                        casIndex = bundleReader.ReadByte();
                    }

                    DbObject res = bundle.GetValue<DbObject>("res")[i] as DbObject;
                    int offset = bundleReader.ReadInt(Endian.Big);
                    int size = bundleReader.ReadInt(Endian.Big);

                    res.SetValue("catalog", catalogIndex);
                    res.SetValue("cas", casIndex);
                    res.SetValue("offset", offset);
                    res.SetValue("size", size);
                    res.SetValue("unk", unk);
                    if (isPatch)
                        res.SetValue("patch", true);
                }
                for (int i = 0; i < bundle.GetValue<DbObject>("chunks").Count; i++)
                {
                    if (list[z++])
                    {
                        unk = bundleReader.ReadByte();
                        isPatch = bundleReader.ReadBoolean();
                        catalogIndex = bundleReader.ReadByte();
                        casIndex = bundleReader.ReadByte();
                    }

                    DbObject chunk = bundle.GetValue<DbObject>("chunks")[i] as DbObject;
                    int offset = bundleReader.ReadInt(Endian.Big);
                    int size = bundleReader.ReadInt(Endian.Big);

                    chunk.SetValue("catalog", catalogIndex);
                    chunk.SetValue("cas", casIndex);
                    chunk.SetValue("offset", offset);
                    chunk.SetValue("size", size);
                    chunk.SetValue("unk", unk);
                    if (isPatch)
                        chunk.SetValue("patch", true);
                }

                return bundle;
            }

            private byte[] WriteBundle(DbObject bundle)
            {
                using (BinarySbWriter bundleWriter = new BinarySbWriter(new MemoryStream()))
                {
                    int totalCount = 0;
                    long newDataOffset;
                    long newLocationOffset;
                    bundleWriter.Write(0x20, Endian.Big);
                    bundleWriter.Write(bundle.GetValue<int>("dataOffset") + 4, Endian.Big);
                    bundleWriter.Write(bundle.GetValue<int>("locationOffset"), Endian.Big);
                    bundleWriter.Write(0xDEADBEEF, Endian.Big);
                    bundleWriter.Write(bundle.GetValue<int>("dataOffset") + 0x24, Endian.Big);
                    bundleWriter.Write(bundle.GetValue<int>("dataOffset") + 0x24, Endian.Big);
                    bundleWriter.Write(bundle.GetValue<int>("dataOffset") + 0x24, Endian.Big);
                    bundleWriter.Write((int)0x00);

                    bundleWriter.Write(bundle);
                    newDataOffset = bundleWriter.Position - 0x20;

                    foreach (DbObject ebx in bundle.GetValue<DbObject>("ebx"))
                    {
                        bundleWriter.Write(ebx.GetValue<byte>("unk"));
                        bundleWriter.Write(ebx.HasValue("patch") ? (byte)0x01 : (byte)0x00);
                        bundleWriter.Write(ebx.GetValue<byte>("catalog"));
                        bundleWriter.Write(ebx.GetValue<byte>("cas"));
                        bundleWriter.Write(ebx.GetValue<int>("offset"), Endian.Big);
                        bundleWriter.Write(ebx.GetValue<int>("size"), Endian.Big);
                        totalCount++;
                    }
                    foreach (DbObject res in bundle.GetValue<DbObject>("res"))
                    {
                        bundleWriter.Write(res.GetValue<byte>("unk"));
                        bundleWriter.Write(res.HasValue("patch") ? (byte)0x01 : (byte)0x00);
                        bundleWriter.Write(res.GetValue<byte>("catalog"));
                        bundleWriter.Write(res.GetValue<byte>("cas"));
                        bundleWriter.Write(res.GetValue<int>("offset"), Endian.Big);
                        bundleWriter.Write(res.GetValue<int>("size"), Endian.Big);
                        totalCount++;
                    }
                    foreach (DbObject chunk in bundle.GetValue<DbObject>("chunks"))
                    {
                        bundleWriter.Write(chunk.GetValue<byte>("unk"));
                        bundleWriter.Write(chunk.HasValue("patch") ? (byte)0x01 : (byte)0x00);
                        bundleWriter.Write(chunk.GetValue<byte>("catalog"));
                        bundleWriter.Write(chunk.GetValue<byte>("cas"));
                        bundleWriter.Write(chunk.GetValue<int>("offset"), Endian.Big);
                        bundleWriter.Write(chunk.GetValue<int>("size"), Endian.Big);
                        totalCount++;
                    }

                    newLocationOffset = bundleWriter.Position;
                    for (int i = 0; i < totalCount; i++)
                        bundleWriter.Write((byte)0x01);

                    bundleWriter.Position = 4;
                    bundleWriter.Write((int)newDataOffset, Endian.Big);
                    bundleWriter.Write((int)newLocationOffset, Endian.Big);
                    bundleWriter.Write(totalCount, Endian.Big);
                    bundleWriter.Write((int)newDataOffset + 0x20, Endian.Big);
                    bundleWriter.Write((int)newDataOffset + 0x20, Endian.Big);
                    bundleWriter.Write((int)newDataOffset + 0x20, Endian.Big);

                    return bundleWriter.ToByteArray();

                }
            }
        }
    }
}
