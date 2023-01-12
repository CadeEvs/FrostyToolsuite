using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using FrostySdk.Managers.Entries;

namespace Frosty.ModSupport
{
    public partial class FrostyModExecutor
    {
        private class ManifestBundleAction
        {
            private static readonly object resourceLock = new object();

            public List<Sha1> DataRefs { get; } = new List<Sha1>();
            public List<Sha1> BundleRefs { get; } = new List<Sha1>();
            public List<CasFileEntry> FileInfos { get; } = new List<CasFileEntry>();
            public List<byte[]> BundleBuffers { get; } = new List<byte[]>();

            public Exception Exception { get; private set; }

            private List<ModBundleInfo> bundles;
            private ManualResetEvent doneEvent;
            private FrostyModExecutor parent;
            private CancellationToken cancelToken;

            public ManifestBundleAction(List<ModBundleInfo> inBundles, ManualResetEvent inDoneEvent, FrostyModExecutor inParent, CancellationToken inCancelToken)
            {
                bundles = inBundles;
                doneEvent = inDoneEvent;
                parent = inParent;
                cancelToken = inCancelToken;
            }

            private void Run()
            {
                try
                {
                    FileSystemManager fs = parent.m_fs;

                    foreach (ModBundleInfo bundle in bundles)
                    {
                        cancelToken.ThrowIfCancellationRequested();

#if FROSTY_DEVELOPER
                        List<Sha1> sha1s = new List<Sha1>();
#endif

                        ManifestBundleInfo manifestBundle = fs.GetManifestBundle(bundle.Name);
                        ManifestFileInfo bundleFile = null;
                        DbObject bundleObj = null;

                        if (manifestBundle.files.Count == 0)
                        {
                            bundleFile = new ManifestFileInfo() { file = new ManifestFileRef(1, false, 0) };
                            manifestBundle.files.Add(bundleFile);

                            bundleObj = new DbObject();
                            bundleObj.SetValue("ebx", DbObject.CreateList());
                            bundleObj.SetValue("res", DbObject.CreateList());
                            bundleObj.SetValue("chunks", DbObject.CreateList());
                            bundleObj.SetValue("chunkMeta", DbObject.CreateList());
                        }
                        else
                        {
                            bundleFile = manifestBundle.files[0];
                            string catalogName = fs.GetCatalog(bundleFile.file);
                            List<ManifestFileInfo> mfi = new List<ManifestFileInfo>();

                            for (int i = 1; i < manifestBundle.files.Count; i++)
                            {
                                ManifestFileInfo fi = manifestBundle.files[i];

                                string catFile = fs.ResolvePath(((fi.file.IsInPatch) ? "native_patch/" : "native_data/") + fs.GetCatalog(fi.file) + "/cas.cat");
                                int catFileHash = Fnv1.HashString(catFile.ToLower());

                                Dictionary<uint, CatResourceEntry> casList = parent.m_resources[catFileHash][fi.file.CasIndex];
                                List<uint> offsets = casList.Keys.ToList();

                                uint totalSize = 0;
                                uint offset = fi.offset;

                                mfi.Add(fi);
#if FROSTY_DEVELOPER
                                sha1s.Add(Sha1.Zero);
#endif

                                if (!casList.ContainsKey(offset))
                                {
                                    offset += (uint)fi.size;

                                    int nextIdx = (!casList.ContainsKey(offset)) ? casList.Count : offsets.BinarySearch(offset);
                                    while (offset > fi.offset)
                                    {
                                        nextIdx--;
                                        offset = offsets[nextIdx];
                                    }

                                    fi.size += fi.offset - offset;
                                }

                                CatResourceEntry entry = casList[offset];
                                totalSize += entry.Size;
                                offset += entry.Size;

                                long size = fi.size;
                                fi.size = offset - fi.offset;

#if FROSTY_DEVELOPER
                                sha1s[sha1s.Count - 1] = entry.Sha1;
                                Debug.Assert(entry.Offset <= fi.offset);
#endif

                                int subIdx = i;
                                while (totalSize != size)
                                {
#if FROSTY_DEVELOPER
                                    Debug.Assert(totalSize <= size);
#endif
                                    CatResourceEntry nextEntry = casList[offset];
                                    {
                                        ManifestFileInfo newFi = new ManifestFileInfo
                                        {
                                            file = new ManifestFileRef(fi.file.CatalogIndex, fi.file.IsInPatch, fi.file.CasIndex),
                                            offset = nextEntry.Offset,
                                            size = nextEntry.Size
                                        };
#if FROSTY_DEVELOPER
                                        sha1s.Add(nextEntry.Sha1);
#endif
                                        mfi.Add(newFi);

                                        totalSize += nextEntry.Size;
                                        offset += nextEntry.Size;
                                    }
                                }
                            }

                            manifestBundle.files.Clear();
                            manifestBundle.files.Add(bundleFile);
                            manifestBundle.files.AddRange(mfi);

                            using (NativeReader reader = new NativeReader(new FileStream(fs.ResolvePath(bundleFile.file), FileMode.Open, FileAccess.Read)))
                            {
                                using (BinarySbReader sbReader = new BinarySbReader(reader.CreateViewStream(bundleFile.offset, bundleFile.size), null))
                                    bundleObj = sbReader.ReadDbObject();
                            }
                        }

                        int idx = 1;
                        foreach (DbObject ebx in bundleObj.GetValue<DbObject>("ebx"))
                        {
                            string name = ebx.GetValue<string>("name");
                            if (bundle.Modify.Ebx.Contains(name))
                            {
                                ManifestFileInfo fi = manifestBundle.files[idx];
                                EbxAssetEntry entry = parent.m_modifiedEbx[name];

                                ebx.SetValue("sha1", entry.Sha1);
                                ebx.SetValue("originalSize", entry.OriginalSize);

                                DataRefs.Add(entry.Sha1);
                                FileInfos.Add(new CasFileEntry() { Entry = null, FileInfo = fi });
                            }
                            idx++;
                        }
                        foreach (string name in bundle.Add.Ebx)
                        {
                            EbxAssetEntry entry = parent.m_modifiedEbx[name];

                            DbObject ebx = new DbObject();
                            ebx.SetValue("name", entry.Name);
                            ebx.SetValue("sha1", entry.Sha1);
                            ebx.SetValue("originalSize", entry.OriginalSize);
                            bundleObj.GetValue<DbObject>("ebx").Add(ebx);

                            ManifestFileInfo fi = new ManifestFileInfo {file = new ManifestFileRef(bundleFile.file.CatalogIndex, false, 0)};
                            manifestBundle.files.Insert(idx++, fi);

                            DataRefs.Add(entry.Sha1);
                            FileInfos.Add(new CasFileEntry() { Entry = null, FileInfo = fi });
                        }
                        foreach (DbObject res in bundleObj.GetValue<DbObject>("res"))
                        {
                            string name = res.GetValue<string>("name");
                            if (bundle.Modify.Res.Contains(name))
                            {
                                ManifestFileInfo fi = manifestBundle.files[idx];
                                ResAssetEntry entry = parent.m_modifiedRes[name];

                                //if (entry.ExtraData != null)
                                //{
                                //    lock (resourceLock)
                                //    {
                                //        // invoke custom handler to modify the base data with the custom data
                                //        HandlerExtraData extraData = (HandlerExtraData)entry.ExtraData;
                                //        if (extraData != null)
                                //        {
                                //            byte[] data = null;
                                //            Stream baseData = parent.rm.GetResourceData(parent.fs.GetFilePath(fi.file.CatalogIndex, fi.file.CasIndex, fi.file.IsInPatch), fi.offset, fi.size);
                                //            ResAssetEntry newEntry = (ResAssetEntry)extraData.Handler.Modify(entry, baseData, extraData.Data, out data);

                                //            if (!parent.archiveData.ContainsKey(newEntry.Sha1))
                                //                parent.archiveData.Add(newEntry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });

                                //            entry.Sha1 = newEntry.Sha1;
                                //            entry.OriginalSize = newEntry.OriginalSize;
                                //            entry.ResMeta = newEntry.ResMeta;
                                //            entry.ExtraData = null;
                                //        }
                                //    }
                                //}

                                res.SetValue("sha1", entry.Sha1);
                                res.SetValue("originalSize", entry.OriginalSize);
                                if (entry.ResMeta != null)
                                    res.SetValue("resMeta", entry.ResMeta);

                                DataRefs.Add(entry.Sha1);
                                FileInfos.Add(new CasFileEntry() { Entry = null, FileInfo = fi });
                            }
                            idx++;
                        }
                        foreach (string name in bundle.Add.Res)
                        {
                            ResAssetEntry entry = parent.m_modifiedRes[name];

                            DbObject res = new DbObject();
                            res.SetValue("name", entry.Name);
                            res.SetValue("sha1", entry.Sha1);
                            res.SetValue("originalSize", entry.OriginalSize);
                            res.SetValue("resRid", (long)entry.ResRid);
                            res.SetValue("resType", entry.ResType);
                            res.SetValue("resMeta", entry.ResMeta);
                            bundleObj.GetValue<DbObject>("res").Add(res);
                            ManifestFileInfo fi = new ManifestFileInfo {file = new ManifestFileRef(bundleFile.file.CatalogIndex, false, 0)};
                            manifestBundle.files.Insert(idx++, fi);

                            DataRefs.Add(entry.Sha1);
                            FileInfos.Add(new CasFileEntry() { Entry = null, FileInfo = fi });
                        }

                        DbObject chunkMeta = bundleObj.GetValue<DbObject>("chunkMeta");

                        // modify chunks
                        int chunkIndex = 0;
                        List<int> chunksToRemove = new List<int>();
                        foreach (DbObject chunk in bundleObj.GetValue<DbObject>("chunks"))
                        {
                            Guid name = chunk.GetValue<Guid>("id");
                            if (bundle.Remove.Chunks.Contains(name))
                            {
                                chunksToRemove.Add(chunkIndex);
                            }
                            else if (bundle.Modify.Chunks.Contains(name))
                            {
                                ChunkAssetEntry entry = parent.m_modifiedChunks[name];
                                DbObject meta = chunkMeta.Find<DbObject>((object a) => { return (a as DbObject).GetValue<int>("h32") == entry.H32; });

                                chunk.SetValue("sha1", entry.Sha1);
                                chunk.SetValue("logicalOffset", entry.LogicalOffset);
                                chunk.SetValue("logicalSize", entry.LogicalSize);
                                if (entry.FirstMip != -1)
                                {
                                    chunk.SetValue("rangeStart", entry.RangeStart);
                                    chunk.SetValue("rangeEnd", entry.RangeEnd);

                                    meta?.GetValue<DbObject>("meta").SetValue("firstMip", entry.FirstMip);
                                }

                                if (idx < manifestBundle.files.Count)
                                {
                                    DataRefs.Add(entry.Sha1);
                                    ManifestFileInfo fi = manifestBundle.files[idx];
                                    FileInfos.Add(new CasFileEntry() { Entry = entry, FileInfo = fi });
                                }
                            }

                            idx++;
                            chunkIndex++;
                        }
                        chunksToRemove.Reverse();
                        foreach (int index in chunksToRemove)
                        {
                            bundleObj.GetValue<DbObject>("chunks").RemoveAt(index);
                            bundleObj.GetValue<DbObject>("chunkMeta").RemoveAt(index);
                            manifestBundle.files.RemoveAt(index + idx);
                        }
                        foreach (Guid name in bundle.Add.Chunks)
                        {
                            ChunkAssetEntry entry = parent.m_modifiedChunks[name];

                            DbObject chunk = new DbObject();
                            chunk.SetValue("id", name);
                            chunk.SetValue("sha1", entry.Sha1);
                            chunk.SetValue("logicalOffset", entry.LogicalOffset);
                            chunk.SetValue("logicalSize", entry.LogicalSize);

                            DbObject meta = new DbObject();
                            meta.SetValue("h32", entry.H32);
                            meta.SetValue("meta", new DbObject());
                            chunkMeta.Add(meta);

                            if (entry.FirstMip != -1)
                            {
                                 chunk.SetValue("rangeStart", entry.RangeStart);
                                chunk.SetValue("rangeEnd", entry.RangeEnd);
                                meta.GetValue<DbObject>("meta").SetValue("firstMip", entry.FirstMip);
                            }

                            bundleObj.GetValue<DbObject>("chunks").Add(chunk);

                            ManifestFileInfo fi = new ManifestFileInfo {file = new ManifestFileRef(bundleFile.file.CatalogIndex, false, 0)};
                            manifestBundle.files.Insert(idx++, fi);

                            DataRefs.Add(entry.Sha1);
                            FileInfos.Add(new CasFileEntry() { Entry = entry, FileInfo = fi });
                        }

                        // finally write out new binary superbundle
                        MemoryStream ms = new MemoryStream();
                        using (BinarySbWriter writer = new BinarySbWriter(ms, true))
                        {
                            writer.Write(bundleObj);
                        }

                        byte[] bundleBuffer = ms.ToArray();
                        Sha1 newSha1 = Utils.GenerateSha1(bundleBuffer);
                        ms.Dispose();

                        BundleRefs.Add(newSha1);
                        DataRefs.Add(newSha1);
                        FileInfos.Add(new CasFileEntry{ Entry = null, FileInfo = bundleFile });
                        BundleBuffers.Add(bundleBuffer);
                    }
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
        }
    }
}
