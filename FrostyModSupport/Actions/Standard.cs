using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Frosty.ModSupport
{
    public partial class FrostyModExecutor
    {
        private class SuperBundleAction
        {
            internal class BaseBundleInfo
            {
                public string Name;
                public long Offset;
                public long Size;
            }

            internal class AssetInfo
            {
                public string Name;
                public int NameHash;
                public Guid Id;
                public bool Modified;
                public bool Inserted;
                public bool Removed;
                public DbObject Asset;
                public DbObject BaseAsset;
                public DbObject Meta;
            }

            public string SuperBundle => superBundle;
            public List<Sha1> CasRefs => casRefs;
            public List<ChunkAssetEntry> ChunkEntries => chunkEntries;
            public bool HasErrored => errorException != null;
            public Exception Exception => errorException;
            public bool TocModified = false;
            public bool SbModified = false;

            private string superBundle;
            private ManualResetEvent doneEvent;
            private FrostyModExecutor parent;

            private List<Sha1> casRefs = new List<Sha1>();
            private List<ChunkAssetEntry> chunkEntries = new List<ChunkAssetEntry>();
            private Exception errorException;

            private string modPath;
            private CancellationToken cancelToken;

            public SuperBundleAction(string inSuperBundle, ManualResetEvent inDoneEvent, FrostyModExecutor inParent, string inModPath, CancellationToken inCancelToken)
            {
                superBundle = inSuperBundle;
                doneEvent = inDoneEvent;
                parent = inParent;
                modPath = inModPath;
                cancelToken = inCancelToken;
            }

            public void Run()
            {
                string path = parent.fs.ResolvePath(superBundle + ".toc");

                try
                {
                    List<Sha1> localCasRefs = new List<Sha1>();
                    DbObject toc = null;

                    bool containsBundlesToModify = false;
                    bool isBinary = false;
                    bool isBase = false;

                    string basePath = parent.fs.ResolvePath("native_data/" + superBundle + ".toc");
                    if (basePath.Equals(path))
                        isBase = true;

                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare2 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                    {
                        if (basePath == "")
                            return;

                        // read base toc to determine binary status
                        using (DbReader reader = new DbReader(new FileStream(basePath, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                            toc = reader.ReadDbObject();

                        // binary superbundle
                        if (toc.GetValue<bool>("alwaysEmitSuperBundle") || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare2)
                            isBinary = true;
                    }

                    if (path != "")
                    {
                        if (!File.Exists(path.Replace(".toc", ".sb")))
                        {
                            // no point doing anything with this, as it has no
                            // superbundle
                            return;
                        }

                        // read base or patch toc
                        using (DbReader reader = new DbReader(new FileStream(path, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                            toc = reader.ReadDbObject();
                    }
                    else
                    {
                        // create new toc
                        toc = new DbObject();
                        toc.SetValue("bundles", new DbObject(false));
                        toc.SetValue("chunks", new DbObject(false));
                        toc.SetValue("cas", true);
                        toc.SetValue("name", superBundle);
                        toc.SetValue("alwaysEmitSuperbundle", false);
                        containsBundlesToModify = true;
                    }

                    bool tocChanged = false;
                    bool sbChanged = false;

                    // special handling for chunk bundles
                    //if (superBundle.Contains("chunks"))
                    if (toc.HasValue("chunks") && toc.GetValue<DbObject>("chunks").Count > 0)
                    {
#if FROSTY_DEVELOPER
                        Debug.Assert(toc.HasValue("bundles") ? toc.GetValue<DbObject>("bundles").Count == 0 : true);
#endif

                        if (parent.modifiedBundles.ContainsKey(chunksBundleHash))
                        {
                            FileInfo sbFi = new FileInfo(parent.fs.BasePath + modPath + "/" + superBundle + ".sb");
                            ModBundleInfo chunkBundle = parent.modifiedBundles[chunksBundleHash];

                            if (isBinary)
                            {
                                byte[] chunkBuffer = null;
                                if (isBase)
                                {
                                    // create new toc
                                    toc = new DbObject();
                                    toc.AddValue("bundles", new DbObject(false));
                                    toc.AddValue("chunks", new DbObject(false));
                                    chunkBuffer = new byte[0];
                                }
                                else
                                {
                                    // read in existing sb data to be appended to
                                    using (NativeReader reader = new NativeReader(new FileStream(path.Replace(".toc", ".sb"), FileMode.Open, FileAccess.Read)))
                                        chunkBuffer = reader.ReadToEnd();
                                }

                                bool isModified = false;

                                DbObject baseToc = null;
                                using (DbReader reader = new DbReader(new FileStream(basePath, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                                    baseToc = reader.ReadDbObject();

                                // update chunk list with base chunks
                                DbObject chunkList = new DbObject(false);
                                foreach (DbObject chunk in baseToc.GetValue<DbObject>("chunks"))
                                    chunkList.Add(chunk);

                                // update chunk list with patch chunks
                                foreach (DbObject chunk in toc.GetValue<DbObject>("chunks"))
                                {
                                    Guid chunkId = chunk.GetValue<Guid>("id");
                                    int index = chunkList.FindIndex((object o) => { return ((DbObject)o).GetValue<Guid>("id") == chunkId; });
                                    if (index != -1)
                                    {
                                        chunk.SetValue("modified", true);
                                        chunkList.SetAt(index, chunk);
                                    }
                                    else
                                    {
                                        chunk.SetValue("modified", true);
                                        chunkList.Add(chunk);
                                    }
                                }

                                // check for modified or added chunks
                                foreach (DbObject chunk in chunkList)
                                {
                                    Guid id = chunk.GetValue<Guid>("id");
                                    if (chunkBundle.Modify.Chunks.Contains(id))
                                    {
                                        isModified = true;
                                        break;
                                    }
                                    if (chunkBundle.Add.Chunks.Count > 0)
                                    {
                                        isModified = true;
                                        break;
                                    }
                                }

                                if (isModified)
                                {
                                    // modify superbundle/toc
                                    if (!Directory.Exists(sbFi.DirectoryName))
                                        Directory.CreateDirectory(sbFi.DirectoryName);

                                    using (NativeWriter writer = new NativeWriter(new FileStream(sbFi.FullName, FileMode.Create, FileAccess.Write)))
                                    {
                                        long offset = chunkBuffer.Length;
                                        writer.Write(chunkBuffer);

                                        toc.SetValue("chunks", new DbObject(false));
                                        foreach (DbObject chunk in chunkList)
                                        {
                                            Guid id = chunk.GetValue<Guid>("id");
                                            if (chunkBundle.Modify.Chunks.Contains(id))
                                            {
                                                isModified = true;
                                                ChunkAssetEntry entry = parent.modifiedChunks[id];

                                                chunk.RemoveValue("modified");
                                                chunk.SetValue("sha1", entry.Sha1);
                                                chunk.SetValue("size", entry.Size);
                                                chunk.SetValue("offset", offset);
                                                toc.GetValue<DbObject>("chunks").Add(chunk);

                                                offset += entry.Size;
                                                writer.Write(parent.archiveData[entry.Sha1].Data);
                                            }
                                            else if (chunk.GetValue<bool>("modified"))
                                            {
                                                chunk.RemoveValue("modified");
                                                toc.GetValue<DbObject>("chunks").Add(chunk);
                                            }
                                        }

                                        // @hack: to ensure new chunks are only added to the chunks bundles
                                        if (superBundle.Contains("chunks"))
                                        {
                                            foreach (Guid id in chunkBundle.Add.Chunks)
                                            {
                                                isModified = true;
                                                ChunkAssetEntry entry = parent.modifiedChunks[id];

                                                DbObject chunk = new DbObject();
                                                chunk.SetValue("id", entry.Id);
                                                chunk.SetValue("sha1", entry.Sha1);
                                                chunk.SetValue("size", entry.Size);
                                                chunk.SetValue("offset", offset);
                                                toc.GetValue<DbObject>("chunks").Add(chunk);

                                                offset += entry.Size;
                                                writer.Write(parent.archiveData[entry.Sha1].Data);
                                            }
                                        }

                                        using (DbWriter tocWriter = new DbWriter(new FileStream(sbFi.FullName.Replace(".sb", ".toc"), FileMode.Create), true))
                                            tocWriter.Write(toc);

                                        TocModified = true;
                                        SbModified = true;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                // DAI stores only delta chunks in patch chunks bundles, so need to
                                // read the base one
                                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                                {
                                    if (isBase)
                                    {
                                        // create new toc
                                        toc = new DbObject();
                                        toc.AddValue("bundles", new DbObject(false));
                                        toc.AddValue("chunks", new DbObject(false));
                                        toc.AddValue("cas", true);
                                    }

                                    DbObject baseToc = null;
                                    using (DbReader reader = new DbReader(new FileStream(basePath, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                                        baseToc = reader.ReadDbObject();

                                    // iterate thru base toc looking for chunks
                                    DbObject chunkList = toc.GetValue<DbObject>("chunks");
                                    foreach (DbObject chunk in baseToc.GetValue<DbObject>("chunks"))
                                    {
                                        Guid id = chunk.GetValue<Guid>("id");
                                        if (chunkBundle.Modify.Chunks.Contains(id))
                                        {
                                            DbObject chunkToEdit = chunk;
                                            bool bFound = false;

                                            foreach (DbObject patchChunk in toc.GetValue<DbObject>("chunks"))
                                            {
                                                Guid patchId = patchChunk.GetValue<Guid>("id");
                                                if (patchId == id)
                                                {
                                                    chunkToEdit = patchChunk;
                                                    bFound = true;
                                                    break;
                                                }
                                            }

                                            if (!bFound)
                                                chunkList.Insert(0, chunkToEdit);

                                            ChunkAssetEntry entry = parent.modifiedChunks[id];
                                            chunkToEdit.SetValue("sha1", entry.Sha1);
                                            chunkToEdit.SetValue("delta", true);

                                            if (entry.IsTocChunk)
                                            {
                                                if (!casRefs.Contains(entry.Sha1))
                                                {
                                                    casRefs.Add(entry.Sha1);
                                                    chunkEntries.Add(entry);
                                                }
                                            }
                                        }

                                        tocChanged = true;
                                    }

                                    // @hack: to ensure new chunks are only added to the chunks bundles
                                    if (superBundle.Contains("chunks"))
                                    {
                                        // add any required chunks
                                        foreach (Guid id in chunkBundle.Add.Chunks)
                                        {
                                            ChunkAssetEntry entry = parent.modifiedChunks[id];

                                            DbObject chunk = new DbObject();
                                            chunk.SetValue("id", entry.Id);
                                            chunk.SetValue("sha1", entry.Sha1);
                                            chunkList.Add(chunk);

                                            if (entry.IsTocChunk)
                                            {
                                                if (!casRefs.Contains(entry.Sha1))
                                                {
                                                    casRefs.Add(entry.Sha1);
                                                    chunkEntries.Add(entry);
                                                }
                                            }

                                            tocChanged = true;
                                        }
                                    }
                                }
                                else
                                {
                                    // update toc with chunks
                                    foreach (DbObject chunk in toc.GetValue<DbObject>("chunks"))
                                    {
                                        Guid id = chunk.GetValue<Guid>("id");
                                        if (chunkBundle.Modify.Chunks.Contains(id))
                                        {
                                            ChunkAssetEntry entry = parent.modifiedChunks[id];
                                            //if (entry.ExtraData != null)
                                            //{
                                            //    // invoke custom handler to modify the base data with the custom data
                                            //    HandlerExtraData extraData = (HandlerExtraData)entry.ExtraData;
                                            //    Stream baseData = parent.rm.GetResourceData(chunk.GetValue<Sha1>("sha1"));

                                            //    byte[] data = null;
                                            //    entry = (ChunkAssetEntry)extraData.Handler.Modify(entry, baseData, null, null, extraData.Data, out data);

                                            //    // needs to be added to the archive data list (bad for threading)
                                            //    parent.archiveData.Add(entry.Sha1, new ArchiveInfo() { Data = data, RefCount = 1 });
                                            //}

                                            chunk.SetValue("sha1", entry.Sha1);

                                            if (entry.IsTocChunk)
                                            {
                                                if (!casRefs.Contains(entry.Sha1))
                                                {
                                                    casRefs.Add(entry.Sha1);
                                                    chunkEntries.Add(entry);
                                                }
                                            }

                                            tocChanged = true;
                                        }
                                    }

                                    // @hack: to ensure new chunks are only added to the chunks bundles
                                    if (superBundle.Contains("chunks"))
                                    {
                                        // add any required chunks
                                        foreach (Guid id in chunkBundle.Add.Chunks)
                                        {
                                            ChunkAssetEntry entry = parent.modifiedChunks[id];

                                            DbObject chunk = new DbObject();
                                            chunk.SetValue("id", entry.Id);
                                            chunk.SetValue("sha1", entry.Sha1);
                                            toc.GetValue<DbObject>("chunks").Add(chunk);

                                            if (entry.IsTocChunk)
                                            {
                                                if (!casRefs.Contains(entry.Sha1))
                                                {
                                                    casRefs.Add(entry.Sha1);
                                                    chunkEntries.Add(entry);
                                                }
                                            }

                                            tocChanged = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // make sure superbundle has bundles requiring modification
                    foreach (DbObject bundle in toc.GetValue<DbObject>("bundles"))
                    {
                        int bundleName = Fnv1.HashString(bundle.GetValue<string>("id").ToLower());
                        if (parent.modifiedBundles.ContainsKey(bundleName))
                        {
                            containsBundlesToModify = true;
                            break;
                        }
                    }

                    // added bundles count as bundles to modify
                    int sbHash = Fnv1a.HashString(superBundle.ToLower());
                    containsBundlesToModify |= parent.addedBundles.ContainsKey(sbHash);

                    if (!containsBundlesToModify && isBase)
                    {
                        // to prevent symlinking
                        TocModified = true;
                        SbModified = true;
                        return;
                    }

                    if (containsBundlesToModify)
                    {
                        if (isBinary)
                        {
                            Dictionary<int, BaseBundleInfo> baseBundles = new Dictionary<int, BaseBundleInfo>();
                            NativeReader baseSb = new NativeReader(new FileStream(parent.fs.ResolvePath("native_data/" + superBundle + ".sb"), FileMode.Open, FileAccess.Read));
                            NativeReader patchSb = null;

                            DbObject baseToc = null;
                            using (DbReader reader = new DbReader(new FileStream(basePath, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                                baseToc = reader.ReadDbObject();

                            foreach (DbObject bundle in baseToc.GetValue<DbObject>("bundles"))
                            {
                                BaseBundleInfo info = new BaseBundleInfo
                                {
                                    Name = bundle.GetValue<string>("id"),
                                    Offset = bundle.GetValue<long>("offset"),
                                    Size = bundle.GetValue<long>("size")
                                };
                                baseBundles.Add(Fnv1.HashString(info.Name.ToLower()), info);
                            }

                            if (!isBase)
                            {
                                // only load patch superbundle if we actually have one
                                patchSb = new NativeReader(new FileStream(parent.fs.ResolvePath("native_patch/" + superBundle + ".sb"), FileMode.Open, FileAccess.Read));
                            }

                            // make sure directory exists
                            FileInfo sbFi = new FileInfo(parent.fs.BasePath + modPath + "/" + superBundle + ".sb");
                            Directory.CreateDirectory(sbFi.DirectoryName);

                            long bundleOffset = 0;
                            foreach (DbObject bundle in toc.GetValue<DbObject>("bundles"))
                            {
                                cancelToken.ThrowIfCancellationRequested();
                                int bundleName = Fnv1.HashString(bundle.GetValue<string>("id").ToLower());

                                List<AssetInfo> totalAssetInfo = new List<AssetInfo>();
                                List<AssetInfo> ebxAssetInfo = new List<AssetInfo>();
                                List<AssetInfo> resAssetInfo = new List<AssetInfo>();
                                List<AssetInfo> chunkAssetInfo = new List<AssetInfo>();

                                bool isDelta = bundle.GetValue<bool>("delta");
                                long baseBundleDataOffset = 0;
                                bool isModified = false;

                                if (isDelta)
                                {
                                    if (parent.modifiedBundles.ContainsKey(bundleName))
                                    {
                                        // all delta bundles must be modified
                                        isModified = true;
                                        BaseBundleInfo bi = null;
                                        Stream baseSbStream = null;
                                        DbObject baseSbBundle = null;

                                        // if bundle has a base bundle version
                                        if (baseBundles.ContainsKey(bundleName))
                                        {
                                            bi = baseBundles[bundleName];
                                            baseSbStream = baseSb.CreateViewStream(bi.Offset, bi.Size);

                                            // read in base bundle unpatched
                                            using (BinarySbReader reader = new BinarySbReader(baseSbStream, 0, parent.fs.CreateDeobfuscator()))
                                                baseSbBundle = reader.ReadDbObject();
                                            baseBundleDataOffset = baseSbBundle.GetValue<long>("dataOffset");
                                            baseSbStream = baseSb.CreateViewStream(bi.Offset, bi.Size);
                                        }

                                        Stream patchSbStream = patchSb.CreateViewStream(bundle.GetValue<long>("offset"), bundle.GetValue<long>("size"));
                                        DbObject sbBundle = null;

                                        // read in bundle and patch it
                                        using (BinarySbReader reader = new BinarySbReader(baseSbStream, patchSbStream, parent.fs.CreateDeobfuscator()))
                                            sbBundle = reader.ReadDbObject();

                                        baseSbStream?.Dispose();
                                        patchSbStream.Dispose();

                                        // ebx
                                        if (baseSbBundle != null)
                                        {
                                            // iterate base bundle first
                                            foreach (DbObject ebx in baseSbBundle.GetValue<DbObject>("ebx"))
                                            {
                                                bool bFound = false;
                                                bool bModified = false;
                                                DbObject foundObject = null;

                                                int hash = Fnv1.HashString(ebx.GetValue<string>("name"));
                                                foreach (DbObject patchEbx in sbBundle.GetValue<DbObject>("ebx"))
                                                {
                                                    if (hash == patchEbx.GetValue<int>("nameHash"))
                                                    {
                                                        bFound = true;
                                                        bModified = (ebx.GetValue<Sha1>("sha1") != patchEbx.GetValue<Sha1>("sha1"));
                                                        foundObject = patchEbx;
                                                        break;
                                                    }
                                                }

                                                AssetInfo info = new AssetInfo {Name = ebx.GetValue<string>("name")};
                                                info.NameHash = Fnv1.HashString(info.Name);
                                                info.Removed = !bFound;
                                                info.Modified = bModified;
                                                info.Asset = (bFound) ? foundObject : ebx;
                                                info.BaseAsset = ebx;
                                                ebxAssetInfo.Add(info);
                                            }
                                        }
                                        // add in all delta bundle items
                                        foreach (DbObject ebx in sbBundle.GetValue<DbObject>("ebx"))
                                        {
                                            int hash = Fnv1.HashString(ebx.GetValue<string>("name"));
                                            if (ebxAssetInfo.FindIndex((AssetInfo a) => a.NameHash == hash) == -1)
                                            {
                                                AssetInfo info = new AssetInfo
                                                {
                                                    Name = ebx.GetValue<string>("name"),
                                                    Inserted = true,
                                                    Asset = ebx
                                                };

                                                info.NameHash = Fnv1.HashString(info.Name);
                                                ebxAssetInfo.Add(info);
                                            }
                                        }

                                        // res
                                        if (baseSbBundle != null)
                                        {
                                            // iterate base bundle first
                                            foreach (DbObject res in baseSbBundle.GetValue<DbObject>("res"))
                                            {
                                                bool bFound = false;
                                                bool bModified = false;
                                                DbObject foundObject = null;

                                                int hash = Fnv1.HashString(res.GetValue<string>("name"));
                                                foreach (DbObject patchRes in sbBundle.GetValue<DbObject>("res"))
                                                {
                                                    if (hash == patchRes.GetValue<int>("nameHash"))
                                                    {
                                                        bFound = true;
                                                        bModified = (res.GetValue<Sha1>("sha1") != patchRes.GetValue<Sha1>("sha1"));
                                                        foundObject = patchRes;
                                                        break;
                                                    }
                                                }

                                                AssetInfo info = new AssetInfo
                                                {
                                                    Name = res.GetValue<string>("name"),
                                                    Removed = !bFound,
                                                    Modified = bModified,
                                                    Asset = (bFound) ? foundObject : res,
                                                    BaseAsset = res
                                                };

                                                info.NameHash = Fnv1.HashString(info.Name);

                                                resAssetInfo.Add(info);
                                            }
                                        }
                                        // add in all delta bundle items
                                        foreach (DbObject res in sbBundle.GetValue<DbObject>("res"))
                                        {
                                            int hash = Fnv1.HashString(res.GetValue<string>("name"));
                                            if (resAssetInfo.FindIndex((AssetInfo a) => a.NameHash == hash) == -1)
                                            {
                                                AssetInfo info = new AssetInfo
                                                {
                                                    Name = res.GetValue<string>("name"),
                                                    Inserted = true,
                                                    Asset = res
                                                };
                                                info.NameHash = Fnv1.HashString(info.Name);
                                                resAssetInfo.Add(info);
                                            }
                                        }

                                        // chunk
                                        int k = 0;
                                        if (baseSbBundle != null)
                                        {
                                            // iterate base bundle first
                                            foreach (DbObject chunk in baseSbBundle.GetValue<DbObject>("chunks"))
                                            {
                                                bool bFound = false;
                                                bool bModified = false;
                                                DbObject foundObject = null;
                                                DbObject foundMeta = (DbObject)baseSbBundle.GetValue<DbObject>("chunkMeta")[k];

                                                int j = 0;
                                                foreach (DbObject patchChunk in sbBundle.GetValue<DbObject>("chunks"))
                                                {
                                                    if (chunk.GetValue<Guid>("id") == patchChunk.GetValue<Guid>("id"))
                                                    {
                                                        bFound = true;
                                                        bModified = (chunk.GetValue<Sha1>("sha1") != patchChunk.GetValue<Sha1>("sha1"));
                                                        foundObject = patchChunk;
                                                        foundMeta = (DbObject)sbBundle.GetValue<DbObject>("chunkMeta")[j];
                                                        break;
                                                    }
                                                    j++;
                                                }

                                                AssetInfo info = new AssetInfo
                                                {
                                                    Id = chunk.GetValue<Guid>("id"),
                                                    Removed = !bFound,
                                                    Modified = bModified,
                                                    Asset = (bFound) ? foundObject : chunk,
                                                    BaseAsset = chunk,
                                                    Meta = foundMeta
                                                };
                                                chunkAssetInfo.Add(info);
                                                k++;
                                            }
                                        }
                                        k = 0;
                                        // add in all delta bundle items
                                        foreach (DbObject chunk in sbBundle.GetValue<DbObject>("chunks"))
                                        {
                                            if (chunkAssetInfo.FindIndex((AssetInfo a) => a.Id == chunk.GetValue<Guid>("id")) == -1)
                                            {
                                                AssetInfo info = new AssetInfo
                                                {
                                                    Id = chunk.GetValue<Guid>("id"),
                                                    Inserted = true,
                                                    Asset = chunk,
                                                    Meta = (DbObject)sbBundle.GetValue<DbObject>("chunkMeta")[k]
                                                };
                                                chunkAssetInfo.Add(info);
                                            }
                                            k++;
                                        }
                                    }
                                    else
                                    {
                                        patchSb.Position = bundle.GetValue<long>("offset");
                                        using (NativeWriter writer = new NativeWriter(new FileStream(parent.fs.BasePath + modPath + "/" + superBundle + ".sb", FileMode.Append, FileAccess.Write)))
                                        {
                                            byte[] buf = new byte[1024 * 1024];
                                            long size = bundle.GetValue<long>("size");

                                            while (size > 0)
                                            {
                                                int bufSize = (size > 1024 * 1024) ? 1024 * 1024 : (int)size;

                                                patchSb.Read(buf, 0, bufSize);
                                                writer.Write(buf, 0, bufSize);

                                                size -= bufSize;
                                            }
                                        }

                                        bundle.SetValue("offset", bundleOffset);
                                        bundleOffset += bundle.GetValue<int>("size");
                                    }
                                }
                                else
                                {
                                    // only base bundles that have affected assets are modified
                                    if (parent.modifiedBundles.ContainsKey(bundleName))
                                    {
                                        isModified = true;
                                        BaseBundleInfo bi = baseBundles[bundleName];

                                        Stream baseSbStream = baseSb.CreateViewStream(bi.Offset, bi.Size);
                                        DbObject baseSbBundle = null;

                                        // read in base bundle unpatched
                                        using (BinarySbReader reader = new BinarySbReader(baseSbStream, 0, parent.fs.CreateDeobfuscator()))
                                            baseSbBundle = reader.ReadDbObject();
                                        baseBundleDataOffset = baseSbBundle.GetValue<long>("dataOffset");

                                        foreach (DbObject ebx in baseSbBundle.GetValue<DbObject>("ebx"))
                                        {
                                            AssetInfo info = new AssetInfo
                                            {
                                                Name = ebx.GetValue<string>("name"),
                                                Asset = ebx,
                                                BaseAsset = ebx
                                            };

                                            info.NameHash = Fnv1.HashString(info.Name);
                                            ebxAssetInfo.Add(info);

                                        }
                                        foreach (DbObject res in baseSbBundle.GetValue<DbObject>("res"))
                                        {
                                            AssetInfo info = new AssetInfo
                                            {
                                                Name = res.GetValue<string>("name"),
                                                Asset = res,
                                                BaseAsset = res
                                            };

                                            info.NameHash = Fnv1.HashString(info.Name);
                                            resAssetInfo.Add(info);
                                        }
                                        int k = 0;
                                        foreach (DbObject chunk in baseSbBundle.GetValue<DbObject>("chunks"))
                                        {
                                            AssetInfo info = new AssetInfo
                                            {
                                                Id = chunk.GetValue<Guid>("id"),
                                                Asset = chunk,
                                                BaseAsset = chunk,
                                                Meta = (DbObject)baseSbBundle.GetValue<DbObject>("chunkMeta")[k++]
                                            };
                                            chunkAssetInfo.Add(info);
                                        }
                                        baseSbStream.Dispose();
                                    }
                                }

                                if (!isModified)
                                    continue;

                                sbChanged = true;
                                if (parent.modifiedBundles.ContainsKey(bundleName))
                                {
                                    ModBundleInfo modifiedBundle = parent.modifiedBundles[bundleName];

                                    // now add in custom modifications
                                    foreach (AssetInfo info in ebxAssetInfo)
                                    {
                                        if (modifiedBundle.Modify.Ebx.Contains(info.Name.ToLower()) && !info.Removed)
                                        {
                                            EbxAssetEntry entry = parent.modifiedEbx[info.Name.ToLower()];
                                            DbObject newObj = new DbObject();

                                            newObj.SetValue("name", info.Name);
                                            newObj.SetValue("sha1", info.Asset.GetValue<Sha1>("sha1"));
                                            newObj.SetValue("originalSize", entry.OriginalSize);
                                            newObj.SetValue("data", parent.archiveData[entry.Sha1].Data);
                                            newObj.SetValue("dataCompressed", true);

                                            info.BaseAsset = (info.Modified) ? info.BaseAsset : info.Asset;
                                            info.Modified = true;
                                            info.Asset = newObj;
                                            isModified = true;
                                        }
                                    }
                                    foreach (string name in modifiedBundle.Add.Ebx)
                                    {
                                        EbxAssetEntry entry = parent.modifiedEbx[name];
                                        DbObject newObj = DbObject.CreateObject();

                                        newObj.SetValue("name", entry.Name);
                                        newObj.SetValue("sha1", entry.Sha1);
                                        newObj.SetValue("originalSize", entry.OriginalSize);
                                        newObj.SetValue("data", parent.archiveData[entry.Sha1].Data);
                                        newObj.SetValue("dataCompressed", true);

                                        AssetInfo info = new AssetInfo
                                        {
                                            Name = entry.Name,
                                            BaseAsset = newObj,
                                            Modified = true,
                                            Inserted = true,
                                            Asset = newObj
                                        };
                                        info.NameHash = Fnv1.HashString(info.Name);
                                        isModified = true;

                                        ebxAssetInfo.Add(info);
                                    }
                                    foreach (AssetInfo info in resAssetInfo)
                                    {
                                        if (modifiedBundle.Modify.Res.Contains(info.Name.ToLower()) && !info.Removed)
                                        {
                                            ResAssetEntry entry = parent.modifiedRes[info.Name.ToLower()];
                                            DbObject newObj = new DbObject();

                                            newObj.SetValue("name", info.Name);
                                            newObj.SetValue("sha1", info.Asset.GetValue<Sha1>("sha1"));
                                            newObj.SetValue("originalSize", entry.OriginalSize);
                                            newObj.SetValue("data", parent.archiveData[entry.Sha1].Data);
                                            newObj.SetValue("dataCompressed", true);
                                            newObj.SetValue("resRid", entry.ResRid);
                                            newObj.SetValue("resMeta", entry.ResMeta);
                                            newObj.SetValue("resType", entry.ResType);

                                            info.BaseAsset = (info.Modified) ? info.BaseAsset : info.Asset;
                                            info.Modified = true;
                                            info.Asset = newObj;
                                            isModified = true;
                                        }
                                    }
                                    foreach (AssetInfo info in chunkAssetInfo)
                                    {
                                        if (modifiedBundle.Modify.Chunks.Contains(info.Id) && !info.Removed)
                                        {
                                            ChunkAssetEntry entry = parent.modifiedChunks[info.Id];
                                            DbObject newObj = new DbObject();

                                            byte[] data = parent.archiveData[entry.Sha1].Data;
                                            if (entry.LogicalOffset != 0)
                                            {
                                                data = new byte[entry.RangeEnd - entry.RangeStart];
                                                Array.Copy(parent.archiveData[entry.Sha1].Data, entry.RangeStart, data, 0, data.Length);
                                            }

                                            newObj.SetValue("id", info.Id);
                                            newObj.SetValue("sha1", info.Asset.GetValue<Sha1>("sha1"));
                                            newObj.SetValue("logicalOffset", entry.LogicalOffset);
                                            newObj.SetValue("logicalSize", entry.LogicalSize);
                                            newObj.SetValue("originalSize", entry.LogicalSize);
                                            newObj.SetValue("data", data);
                                            newObj.SetValue("dataCompressed", true);

                                            info.BaseAsset = (info.Modified) ? info.BaseAsset : info.Asset;
                                            info.Modified = true;
                                            info.Asset = newObj;
                                            isModified = true;
                                        }
                                    }
                                }

                                totalAssetInfo.AddRange(ebxAssetInfo);
                                totalAssetInfo.AddRange(resAssetInfo);
                                totalAssetInfo.AddRange(chunkAssetInfo);

                                // count asset types
                                int ebxTotalCount = 0;
                                foreach (AssetInfo info in ebxAssetInfo)
                                {
                                    if (!info.Removed)
                                        ebxTotalCount++;
                                }
                                int resTotalCount = 0;
                                foreach (AssetInfo info in resAssetInfo)
                                {
                                    if (!info.Removed)
                                        resTotalCount++;
                                }
                                int chunkTotalCount = 0;
                                foreach (AssetInfo info in chunkAssetInfo)
                                {
                                    if (!info.Removed)
                                        chunkTotalCount++;
                                }

                                // now write out new patched binary superbundle
                                using (NativeWriter writer = new NativeWriter(new FileStream(parent.fs.BasePath + modPath + "/" + superBundle + ".sb", FileMode.Append, FileAccess.Write)))
                                {
                                    writer.Write(0x01, Endian.Big);
                                    writer.Write(0x00, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);

                                    long startPos = writer.Position;

                                    writer.Write(0x9D798ED5, Endian.Big);
                                    writer.Write(ebxTotalCount + resTotalCount + chunkTotalCount, Endian.Big);
                                    writer.Write(ebxTotalCount, Endian.Big);
                                    writer.Write(resTotalCount, Endian.Big);
                                    writer.Write(chunkTotalCount, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);
                                    writer.Write(0xDEADBABE, Endian.Big);

                                    // sha1's
                                    foreach (AssetInfo info in totalAssetInfo)
                                    {
                                        if (!info.Removed)
                                            writer.Write(info.Asset.GetValue<Sha1>("sha1"));
                                    }

                                    // names
                                    long nameOffset = 0;
                                    Dictionary<uint, long> stringToOffsetMap = new Dictionary<uint, long>();
                                    List<string> stringsToPrint = new List<string>();
                                    foreach (AssetInfo info in totalAssetInfo)
                                    {
                                        if (!info.Removed && info.Name != null)
                                        {
                                            uint hash = (uint)Fnv1.HashString(info.Asset.GetValue<string>("name"));
                                            if (!stringToOffsetMap.ContainsKey(hash))
                                            {
                                                stringsToPrint.Add(info.Asset.GetValue<string>("name"));
                                                stringToOffsetMap.Add(hash, nameOffset);
                                                nameOffset += info.Asset.GetValue<string>("name").Length + 1;
                                            }
                                            writer.Write((uint)stringToOffsetMap[hash], Endian.Big);
                                            writer.Write(info.Asset.GetValue<int>("originalSize"), Endian.Big);
                                        }
                                    }

                                    // res
                                    foreach (AssetInfo info in resAssetInfo)
                                    {
                                        if (!info.Removed)
                                            writer.Write(info.Asset.GetValue<int>("resType"), Endian.Big);
                                    }
                                    foreach (AssetInfo info in resAssetInfo)
                                    {
                                        if (!info.Removed)
                                            writer.Write(info.Asset.GetValue<byte[]>("resMeta"));
                                    }
                                    foreach (AssetInfo info in resAssetInfo)
                                    {
                                        if (!info.Removed)
                                            writer.Write(info.Asset.GetValue<long>("resRid"), Endian.Big);
                                    }

                                    // chunks
                                    foreach (AssetInfo info in chunkAssetInfo)
                                    {
                                        if (!info.Removed)
                                        {
                                            writer.Write(info.Asset.GetValue<Guid>("id"), Endian.Big);
                                            writer.Write(info.Asset.GetValue<int>("logicalOffset"), Endian.Big);
                                            writer.Write(info.Asset.GetValue<int>("logicalSize"), Endian.Big);
                                        }
                                    }

                                    // meta
                                    long metaOffset = 0;
                                    long metaSize = 0;
                                    if (chunkAssetInfo.Count > 0)
                                    {
                                        DbObject meta = new DbObject(false);
                                        foreach (AssetInfo info in chunkAssetInfo)
                                        {
                                            if (!info.Removed)
                                                meta.Add(info.Meta);
                                        }

                                        metaOffset = writer.Position - startPos;
                                        using (DbWriter metaWriter = new DbWriter(new MemoryStream()))
                                            writer.Write(metaWriter.WriteDbObject("chunkMeta", meta));
                                        metaSize = ((writer.Position - startPos) - metaOffset);
                                    }

                                    // strings
                                    long stringsOffset = writer.Position - startPos;
                                    foreach (string str in stringsToPrint)
                                        writer.WriteNullTerminatedString(str);

                                    while ((writer.Position - (startPos - 4)) % 16 != 0)
                                        writer.Write((byte)0x00);

                                    // base bundle data offset
                                    if (baseBundleDataOffset > 0)
                                        writer.Write((uint)baseBundleDataOffset | 0x40000000, Endian.Big);

                                    // delta data offset
                                    long dataOffset = writer.Position - startPos;

                                    // write data
                                    for (int y = 0; y < totalAssetInfo.Count; y++)
                                    {
                                        AssetInfo info = totalAssetInfo[y];

                                        long size = info.Asset.GetValue<long>("originalSize");
                                        uint blockSize = (uint)((size / 0x10000) + ((size % 0x10000) != 0 ? 1 : 0));

                                        if (info.Removed)
                                        {
                                            if (blockSize > 0)
                                            {
                                                // asset was removed so skip over data
                                                writer.Write(blockSize | 0x40000000, Endian.Big);
                                            }
                                        }
                                        else if (info.Inserted || info.Modified)
                                        {
                                            // asset was added or modified, so use delta data
                                            byte[] data = (info.Asset.GetValue<bool>("dataCompressed"))
                                                ? info.Asset.GetValue<byte[]>("data")
                                                : Utils.CompressFile(info.Asset.GetValue<byte[]>("data"), resType: (ResourceType)info.Asset.GetValue<int>("resType", -1));

                                            if (!info.Inserted)
                                            {
                                                long origSize = info.BaseAsset.GetValue<long>("originalSize");
                                                uint origBlockSize = (uint)((origSize / 0x10000) + ((origSize % 0x10000) != 0 ? 1 : 0));
                                                if (origBlockSize > 0)
                                                    writer.Write(origBlockSize | 0x40000000, Endian.Big);
                                            }

                                            writer.Write(blockSize | 0x30000000, Endian.Big);
                                            writer.Write(data);
                                        }
                                        else
                                        {
                                            writer.Write(blockSize, Endian.Big);
                                        }
                                    }
                                    long dataSize = (writer.Position - startPos) - dataOffset;

                                    // update bundle data
                                    bundle.RemoveValue("base");
                                    bundle.SetValue("delta", true);
                                    bundle.SetValue("offset", bundleOffset);
                                    bundle.SetValue("size", (int)(writer.Position - (startPos - 0x18)));

                                    // update all relevant offsets
                                    writer.Position = startPos + 0x14;
                                    writer.Write((uint)(stringsOffset), Endian.Big);
                                    writer.Write((uint)(metaOffset), Endian.Big);
                                    writer.Write((uint)(metaSize), Endian.Big);

                                    writer.Position = startPos - 16;
                                    writer.Write((uint)(dataOffset + 8), Endian.Big);
                                    writer.Write((uint)(dataSize), Endian.Big);
                                    if (baseBundleDataOffset != 0)
                                        dataOffset -= 4;
                                    writer.Write((uint)(dataOffset), Endian.Big);
                                    writer.Write((uint)((dataOffset) | 0x80000000), Endian.Big);

                                    bundleOffset += bundle.GetValue<int>("size");
                                }
                            }

                            baseSb?.Dispose();
                            patchSb?.Dispose();

                            // remove non required fields (in case they exist)
                            toc.RemoveValue("tag");
                            toc.RemoveValue("name");
                            toc.RemoveValue("totalSize");
                            toc.RemoveValue("alwaysEmitSuperbundle");

                            tocChanged = true;
                        }
                        else
                        {
                            if (parent.addedBundles.ContainsKey(sbHash))
                            {
                                foreach (string bundle in parent.addedBundles[sbHash])
                                {
                                    DbObject tocEntry = new DbObject();
                                    tocEntry.SetValue("id", bundle);
                                    tocEntry.SetValue("offset", (long)0xDEADBEEF);
                                    tocEntry.SetValue("size", (long)0);
                                    toc.GetValue<DbObject>("bundles").Add(tocEntry);
                                    tocChanged = true;
                                }
                            }

                            MemoryStream outSbStream = new MemoryStream();
                            List<long> bundleOffsets = new List<long>();
                            List<long> bundleSizes = new List<long>();

                            // modify bundles
                            Stream sbStream = null;
                            foreach (DbObject bundle in toc.GetValue<DbObject>("bundles"))
                            {
                                cancelToken.ThrowIfCancellationRequested();

                                bool baseBundle = false;
                                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                                    baseBundle = !(bundle.GetValue<bool>("delta"));

                                DbObject sbBundle = null;
                                if (bundle.GetValue<long>("offset") == 0xDEADBEEF)
                                {
                                    sbBundle = new DbObject();
                                    sbBundle.SetValue("path", bundle.GetValue<string>("id"));
                                    sbBundle.SetValue("magicSalt", 0x7065636d);
                                    sbBundle.SetValue("ebx", new DbObject(false));
                                    sbBundle.SetValue("res", new DbObject(false));
                                    sbBundle.SetValue("chunks", new DbObject(false));
                                    sbBundle.SetValue("chunkMeta", new DbObject(false));
                                    sbBundle.SetValue("alignMembers", false);
                                    sbBundle.SetValue("ridSupport", true);
                                    sbBundle.SetValue("storeCompressedSizes", false);
                                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeed && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
                                    {
                                        sbBundle.SetValue("chunkBundleSize", 0);
                                        sbBundle.SetValue("resBundleSize", 0);
                                        sbBundle.SetValue("ebxBundleSize", 0);
                                        sbBundle.SetValue("dbxBundleSize", 0);
                                    }
                                    sbBundle.SetValue("totalSize", 0);
                                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeed && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
                                        sbBundle.SetValue("dbxTotalSize", 0);
                                }
                                else
                                {
                                    sbStream = null;
                                    if (baseBundle)
                                    {
                                        // In DAI/NFS a toc may reference either a base or delta bundle for each individual entry
                                        sbStream = new FileStream(parent.fs.ResolvePath("native_data/" + superBundle + ".sb"), FileMode.Open, FileAccess.Read);
                                    }
                                    else
                                    {
                                        // Other games tocs only specify all base or all delta
                                        sbStream = new FileStream((isBase)
                                            ? parent.fs.ResolvePath("native_data/" + superBundle + ".sb")
                                            : parent.fs.ResolvePath("native_patch/" + superBundle + ".sb"),
                                            FileMode.Open, FileAccess.Read);
                                    }
                                    using (DbReader reader = new DbReader(sbStream, parent.fs.CreateDeobfuscator()))
                                    {
                                        sbStream.Position = bundle.GetValue<long>("offset");
                                        sbBundle = reader.ReadDbObject();
                                    }
                                }

                                bool modified = false;
                                int bundleName = Fnv1.HashString(bundle.GetValue<string>("id").ToLower());

                                if (parent.modifiedBundles.ContainsKey(bundleName))
                                {
                                    sbChanged = true;
                                    modified = true;
                                    ModBundleInfo modBundle = parent.modifiedBundles[bundleName];

                                    long chunkBundleSize = 0;
                                    long resBundleSize = 0;
                                    long ebxBundleSize = 0;

                                    // @temp
                                    sbBundle.RemoveValue("bmm");
                                    sbBundle.RemoveValue("dbx");

                                    ModBundleInfo info = parent.modifiedBundles[bundleName];

                                    // modify ebx
                                    int ebxIndex = 0;
                                    List<int> ebxToRemove = new List<int>();
                                    foreach (DbObject ebx in sbBundle.GetValue<DbObject>("ebx"))
                                    {
                                        string name = ebx.GetValue<string>("name");
                                        ebxIndex++;

                                        if (modBundle.Remove.Ebx.Contains(name))
                                        {
                                            ebxToRemove.Add(ebxIndex - 1);
                                            ebxBundleSize -= ebx.GetValue<long>("size");
                                            continue;
                                        }
                                        
                                        if (modBundle.Modify.Ebx.Contains(name))
                                        {
                                            EbxAssetEntry ebxEntry = parent.modifiedEbx[name];

                                            ebx.SetValue("sha1", ebxEntry.Sha1);
                                            ebx.SetValue("size", ebxEntry.Size);
                                            ebx.SetValue("originalSize", ebxEntry.OriginalSize);

                                            if (ebxEntry.IsInline)
                                                ebx.SetValue("idata", parent.archiveData[ebxEntry.Sha1].Data);

                                            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                                            {
                                                ebx.SetValue("casPatchType", 1);
                                                ebx.RemoveValue("baseSha1");
                                                ebx.RemoveValue("deltaSha1");
                                            }

                                            if (!casRefs.Contains(ebxEntry.Sha1))
                                            {
                                                casRefs.Add(ebxEntry.Sha1);
                                                chunkEntries.Add(null);
                                            }
                                        }

                                        ebxBundleSize += ebx.GetValue<long>("size");
                                    }

                                    // remove ebx
                                    foreach (int ebxIdx in ebxToRemove)
                                        sbBundle.GetValue<DbObject>("ebx").RemoveAt(ebxIdx);

                                    // add new ebx
                                    foreach (string name in modBundle.Add.Ebx)
                                    {
                                        modified = true;
                                        EbxAssetEntry entry = parent.modifiedEbx[name];

                                        DbObject ebx = new DbObject();
                                        ebx.SetValue("name", entry.Name);
                                        ebx.SetValue("sha1", entry.Sha1);
                                        ebx.SetValue("size", entry.Size);
                                        ebx.SetValue("originalSize", entry.OriginalSize);

                                        if (!casRefs.Contains(entry.Sha1))
                                        {
                                            casRefs.Add(entry.Sha1);
                                            chunkEntries.Add(null);
                                        }

                                        sbBundle.GetValue<DbObject>("ebx").Add(ebx);
                                        ebxBundleSize += ebx.GetValue<long>("size");
                                    }

                                    // modify res
                                    int resIndex = 0;
                                    List<int> resToRemove = new List<int>();
                                    foreach (DbObject res in sbBundle.GetValue<DbObject>("res"))
                                    {
                                        string name = res.GetValue<string>("name");
                                        resIndex++;

                                        if (modBundle.Remove.Res.Contains(name))
                                        {
                                            resToRemove.Add(resIndex - 1);
                                            resBundleSize -= res.GetValue<long>("size");
                                            continue;
                                        }
                                        
                                        if (modBundle.Modify.Res.Contains(name))
                                        {
                                            ResAssetEntry resEntry = parent.modifiedRes[name];

                                            res.SetValue("sha1", resEntry.Sha1);
                                            res.SetValue("size", resEntry.Size);
                                            res.SetValue("originalSize", resEntry.OriginalSize);
                                            res.SetValue("resRid", (long)resEntry.ResRid);
                                            res.SetValue("resMeta", (byte[])resEntry.ResMeta);

                                            if (resEntry.IsInline)
                                                res.SetValue("idata", parent.archiveData[resEntry.Sha1].Data);

                                            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                                            {
                                                res.SetValue("casPatchType", 1);
                                                res.RemoveValue("baseSha1");
                                                res.RemoveValue("deltaSha1");
                                            }

                                            if (!casRefs.Contains(resEntry.Sha1))
                                            {
                                                casRefs.Add(resEntry.Sha1);
                                                chunkEntries.Add(null);
                                            }
                                        }

                                        resBundleSize += res.GetValue<long>("size");
                                    }

                                    // remove res
                                    foreach (int resIdx in resToRemove)
                                        sbBundle.GetValue<DbObject>("res").RemoveAt(resIdx);

                                    // add new res
                                    foreach (string name in modBundle.Add.Res)
                                    {
                                        modified = true;
                                        ResAssetEntry entry = parent.modifiedRes[name];

                                        DbObject res = new DbObject();
                                        res.SetValue("name", entry.Name);
                                        res.SetValue("sha1", entry.Sha1);
                                        res.SetValue("size", entry.Size);
                                        res.SetValue("originalSize", entry.OriginalSize);
                                        res.SetValue("resType", entry.ResType);
                                        res.SetValue("resMeta", (byte[])entry.ResMeta);
                                        res.SetValue("resRid", (long)entry.ResRid);
                                        if (entry.IsInline)
                                            res.SetValue("idata", parent.archiveData[entry.Sha1].Data);

                                        if (!casRefs.Contains(entry.Sha1))
                                        {
                                            casRefs.Add(entry.Sha1);
                                            chunkEntries.Add(null);
                                        }

                                        sbBundle.GetValue<DbObject>("res").Add(res);
                                        resBundleSize += res.GetValue<long>("size");
                                    }

                                    // modify chunks
                                    List<int> chunksToRemove = new List<int>();
                                    if (sbBundle.GetValue<DbObject>("chunks") != null)
                                    {
                                        int index = 0;
                                        foreach (DbObject chunk in sbBundle.GetValue<DbObject>("chunks"))
                                        {
                                            Guid id = chunk.GetValue<Guid>("id");
                                            if (modBundle.Remove.Chunks.Contains(id))
                                            {
                                                chunksToRemove.Add(index - 1);
                                                chunkBundleSize -= chunk.GetValue<long>("bundledSize");
                                                continue;
                                            }
                                            
                                            if (modBundle.Modify.Chunks.Contains(id))
                                            {
                                                ChunkAssetEntry chunkEntry = parent.modifiedChunks[id];

                                                chunk.SetValue("sha1", chunkEntry.Sha1);
                                                chunk.SetValue("size", (int)chunkEntry.Size);

                                                if (chunkEntry.FirstMip != -1)
                                                {
                                                    DbObject chunkMeta = null;
                                                    foreach (DbObject curMeta in sbBundle.GetValue<DbObject>("chunkMeta"))
                                                    {
                                                        if (curMeta.GetValue<int>("h32") == chunkEntry.H32)
                                                            curMeta.GetValue<DbObject>("meta").SetValue("firstMip", chunkEntry.FirstMip);
                                                    }
                                                    chunk.SetValue("rangeStart", (int)chunkEntry.RangeStart);
                                                    chunk.SetValue("rangeEnd", (int)chunkEntry.RangeEnd);

                                                    //chunkMeta.GetValue<DbObject>("meta").RemoveValue("firstMip");
                                                    //chunkMeta.GetValue<DbObject>("meta").RemoveValue("firstMip");
                                                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                                                        chunk.SetValue("bundledSize", (int)(chunkEntry.RangeEnd - chunkEntry.RangeStart)); chunk.SetValue("bundledSize", chunkEntry.Size);
                                                }
                                                else
                                                {
                                                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                                                    {
                                                        chunk.RemoveValue("rangeStart");
                                                        chunk.RemoveValue("rangeEnd");
                                                    }

                                                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                                                        chunk.SetValue("bundledSize", (int)chunkEntry.Size);
                                                }

                                                chunk.SetValue("logicalOffset", (int)chunkEntry.LogicalOffset);
                                                chunk.SetValue("logicalSize", (int)chunkEntry.LogicalSize);

                                                if (chunkEntry.IsInline)
                                                    chunk.SetValue("idata", parent.archiveData[chunkEntry.Sha1].Data);
                                                else
                                                    chunk.RemoveValue("idata");

                                                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                                                    chunk.SetValue("casPatchType", 1);

                                                if (!casRefs.Contains(chunkEntry.Sha1))
                                                {
                                                    casRefs.Add(chunkEntry.Sha1);
                                                    chunkEntries.Add(chunkEntry);
                                                }
                                            }

                                            chunkBundleSize += (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                                                ? chunk.GetValue<long>("bundledSize")
                                                : chunk.GetValue<long>("size");
                                        }
                                    }

                                    // remove chunks
                                    foreach (int chunkId in chunksToRemove)
                                        sbBundle.GetValue<DbObject>("chunks").RemoveAt(chunkId);

                                    // add new chunks
                                    if (modBundle.Add.Chunks.Count != 0 && sbBundle.GetValue<DbObject>("chunks") == null)
                                    {
                                        sbBundle.AddValue("chunks", new DbObject(false));
                                        sbBundle.AddValue("chunkMeta", new DbObject(false));
                                    }
                                    foreach (Guid id in modBundle.Add.Chunks)
                                    {
                                        modified = true;

                                        ChunkAssetEntry entry = parent.modifiedChunks[id];

                                        DbObject chunkMeta = new DbObject();
                                        chunkMeta.AddValue("h32", entry.H32);
                                        chunkMeta.AddValue("meta", new DbObject());

                                        DbObject chunk = new DbObject();
                                        chunk.SetValue("id", entry.Id);
                                        chunk.SetValue("sha1", entry.Sha1);
                                        chunk.SetValue("size", (int)entry.Size);

                                        if (entry.FirstMip != -1)
                                        {
                                            chunk.SetValue("rangeStart", (int)entry.RangeStart);
                                            chunk.SetValue("rangeEnd", (int)entry.RangeEnd);

                                            chunkMeta.GetValue<DbObject>("meta").SetValue("firstMip", entry.FirstMip);
                                            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                                                chunk.SetValue("bundledSize", (int)(entry.RangeEnd - entry.RangeStart));
                                        }
                                        else
                                        {
                                            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                                                chunk.SetValue("bundledSize", (int)entry.Size);
                                        }

                                        chunk.SetValue("logicalOffset", (int)entry.LogicalOffset);
                                        chunk.SetValue("logicalSize", (int)entry.LogicalSize);

                                        if (entry.IsInline)
                                            chunk.SetValue("idata", parent.archiveData[entry.Sha1].Data);

                                        if (!casRefs.Contains(entry.Sha1))
                                        {
                                            casRefs.Add(entry.Sha1);
                                            chunkEntries.Add(entry);
                                        }

                                        sbBundle.GetValue<DbObject>("chunks").Add(chunk);
                                        sbBundle.GetValue<DbObject>("chunkMeta").Add(chunkMeta);

                                        chunkBundleSize += (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                                            ? chunk.GetValue<long>("bundledSize")
                                            : chunk.GetValue<long>("size");
                                    }

                                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeed && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
                                    {
                                        sbBundle.SetValue("chunkBundleSize", chunkBundleSize);
                                        sbBundle.SetValue("resBundleSize", resBundleSize);
                                        sbBundle.SetValue("ebxBundleSize", ebxBundleSize);
                                        sbBundle.SetValue("dbxBundleSize", (long)0);
                                    }

                                    sbBundle.SetValue("totalSize", chunkBundleSize + resBundleSize + ebxBundleSize);
                                    if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeed && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals)
                                        sbBundle.SetValue("dbxTotalSize", (long)0);
                                }

                                // only add modified base bundles or delta bundles
                                if ((baseBundle && modified) || !baseBundle)
                                {
                                    bundleOffsets.Add(outSbStream.Position);
                                    using (DbWriter writer = new DbWriter(outSbStream, false, true))
                                        writer.Write(sbBundle);

                                    bundle.SetValue("size", (int)(outSbStream.Position - bundleOffsets[bundleOffsets.Count - 1]));

                                    if (baseBundle)
                                    {
                                        // turn base bundle into delte bundle
                                        bundle.RemoveValue("base");
                                        bundle.AddValue("delta", true);
                                    }
                                }
                                else
                                {
                                    // record base bundle offset
                                    bundleOffsets.Add(bundle.GetValue<long>("offset"));
                                    if (baseBundle)
                                    {
                                        // for DAI, mark it as a base bundle
                                        bundle.AddValue("base", true);
                                    }
                                }
                            }

                            if (sbChanged)
                            {
                                // write out actual SuperBundle
                                FileInfo fi = new FileInfo(parent.fs.BasePath + modPath + "/" + superBundle + ".sb");
                                Directory.CreateDirectory(fi.DirectoryName);

                                int bundleListSize = (int)(outSbStream.Length + 1);
                                int totalSize = Calc7BitEncodedIntSize(bundleListSize) + bundleListSize + 10;

                                long offset = 0;
                                outSbStream.Position = 0;

                                // need to write it out manually so that the offset to the first bundle can be obtained
                                using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
                                {
                                    writer.Write((byte)0x82);
                                    writer.Write7BitEncodedInt(totalSize);
                                    writer.Write((byte)0x01);
                                    writer.WriteNullTerminatedString("bundles");
                                    writer.Write7BitEncodedInt(bundleListSize);

                                    offset = writer.Position;

                                    writer.Write(outSbStream.ToArray());
                                    writer.Write((ushort)0x00);
                                }
                                outSbStream.Dispose();

                                // modify toc bundle offsets

                                int idx = 0;
                                foreach (DbObject bundle in toc.GetValue<DbObject>("bundles"))
                                    bundle.SetValue("offset", (offset + bundleOffsets[idx++]));

                                tocChanged = true;
                            }
                        }
                    }

                    if (tocChanged)
                    {
                        FileInfo fi = new FileInfo(parent.fs.BasePath + modPath + "/" + superBundle + ".toc");
                        Directory.CreateDirectory(fi.DirectoryName);

                        // write out modified toc file
                        using (DbWriter writer = new DbWriter(new FileStream(fi.FullName, FileMode.Create), true))
                            writer.Write(toc);
                    }

                    TocModified = tocChanged;
                    SbModified = sbChanged;
                }
                catch (Exception e)
                {
                    // thread exception caught
                    errorException = e;
                }
            }

            public void ThreadPoolCallback(object threadContext)
            {
                Run();

                // are all threads done?
                if (Interlocked.Decrement(ref parent.numTasks) == 0)
                    doneEvent.Set();
            }

            private int Calc7BitEncodedIntSize(int value)
            {
                int outSize = 0;
                uint v = (uint)value;
                while (v >= 0x80)
                {
                    outSize++;
                    v >>= 7;
                }
                outSize++;
                return outSize;
            }
        }
    }
}
