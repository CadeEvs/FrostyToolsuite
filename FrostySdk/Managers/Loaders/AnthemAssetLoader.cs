using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrostySdk.Managers
{
    public partial class AssetManager
    {
        internal class AnthemAssetLoader : IAssetLoader
        {
            public void Load(AssetManager parent, BinarySbDataHelper helper)
            {
                foreach (CatalogInfo catalog in parent.m_fileSystem.EnumerateCatalogInfos())
                {
                    foreach (string sbName in catalog.SuperBundles.Keys)
                    {
                        SuperBundleEntry sbe = parent.m_superBundles.Find((SuperBundleEntry a) => a.Name == sbName);
                        int sbIndex = -1;

                        if (sbe != null)
                        {
                            sbIndex = parent.m_superBundles.IndexOf(sbe);
                        }
                        else
                        {
                            parent.m_superBundles.Add(new SuperBundleEntry() { Name = sbName });
                            sbIndex = parent.m_superBundles.Count - 1;
                        }

                        parent.WriteToLog("Loading data ({0})", sbName);


                        string sbPath = sbName.Replace("win32", catalog.Name);
                        string tmp = parent.m_fileSystem.ResolvePath("native_data/" + sbPath + ".toc");

                        if (tmp == "")
                            sbPath = sbName;

                        List<BaseBundleInfo> baseBundles = new List<BaseBundleInfo>();
                        List<BaseBundleInfo> patchBundles = new List<BaseBundleInfo>();

                        string tocPath = parent.m_fileSystem.ResolvePath(string.Format("native_data/{0}.toc", sbPath));
                        if (tocPath != "")
                        {
                            int[] values = new int[12];
                            byte[] buffer = null;

                            using (NativeReader reader = new NativeReader(new FileStream(tocPath, FileMode.Open, FileAccess.Read), parent.m_fileSystem.CreateDeobfuscator()))
                            {
                                for (int i = 0; i < 12; i++)
                                    values[i] = reader.ReadInt(Endian.Big);
                                buffer = reader.ReadToEnd();
                            }

                            if (buffer.Length != 0)
                            {
                                using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
                                {
                                    List<int> unkList = new List<int>();

                                    if (values[0] != 0)
                                    {
                                        for (int i = 0; i < values[2]; i++)
                                            unkList.Add(reader.ReadInt(Endian.Big));
                                        reader.Position = values[1] - 0x30;

                                        for (int i = 0; i < values[2]; i++)
                                        {
                                            int nameOffset = reader.ReadInt(Endian.Big);
                                            uint bundleSize = reader.ReadUInt(Endian.Big);
                                            long bundleOffset = reader.ReadLong(Endian.Big);

                                            long curPos = reader.Position;
                                            reader.Position = values[8] - 0x30 + nameOffset;

                                            string name = reader.ReadNullTerminatedString();
                                            reader.Position = curPos;

                                            BaseBundleInfo bi = new BaseBundleInfo()
                                            {
                                                Name = name,
                                                Offset = bundleOffset,
                                                Size = bundleSize
                                            };
                                            baseBundles.Add(bi);

                                            // Now process bundle
                                            //BundleEntry be = new BundleEntry() { Name = name, SuperBundleId = sbIndex };
                                            //bundles.Add(be);

                                            //Stream stream = baseMf.CreateViewStream(vals[2] + 0x20, vals[0] - 0x20);
                                            //using (BinarySbReader bundleReader = new BinarySbReader(stream, 0, parent.fs.CreateDeobfuscator()))
                                            //{
                                            //    DbObject bundle = bundleReader.ReadDbObject();

                                            //    // process assets
                                            //    ProcessBundleEbx(bundle, bundles.Count - 1, helper);
                                            //    ProcessBundleRes(bundle, bundles.Count - 1, helper);
                                            //    ProcessBundleChunks(bundle, bundles.Count - 1, helper);
                                            //}
                                        }
                                    }
                                    if (values[3] != 0)
                                    {
                                        reader.Position = values[3] - 0x30;
                                        List<int> unkValues = new List<int>();
                                        for (int i = 0; i < values[5]; i++)
                                            unkValues.Add(reader.ReadInt(Endian.Big));

                                        reader.Position = values[4] - 0x30;
                                        List<Guid> chunkGuids = new List<Guid>();
                                        for (int i = 0; i < values[5]; i++)
                                        {
                                            byte[] b = reader.ReadBytes(16);
                                            Guid guid = new Guid(new byte[]
                                            {
                                                b[15], b[14], b[13], b[12],
                                                b[11], b[10], b[9], b[8],
                                                b[7], b[6], b[5], b[4],
                                                b[3], b[2], b[1], b[0]
                                            });
                                            int unk = reader.ReadInt(Endian.Big) & 0xFFFFFF;
                                            while (chunkGuids.Count <= unk)
                                                chunkGuids.Add(Guid.Empty);
                                            chunkGuids[unk / 3] = guid;
                                        }
                                        reader.Position = values[6] - 0x30;
                                        for (int i = 0; i < values[5]; i++)
                                        {
                                            byte unk = reader.ReadByte();
                                            bool isPatch = reader.ReadBoolean();
                                            byte catalogIndex = reader.ReadByte();
                                            byte casIndex = reader.ReadByte();
                                            uint offset = reader.ReadUInt(Endian.Big);
                                            uint size = reader.ReadUInt(Endian.Big);

                                            ChunkAssetEntry chunk = new ChunkAssetEntry
                                            {
                                                Id = chunkGuids[i],
                                                Size = size,
                                                Location = AssetDataLocation.CasNonIndexed,
                                                ExtraData = new AssetExtraData
                                                {
                                                    CasPath = parent.m_fileSystem.GetFilePath(catalogIndex, casIndex, isPatch),
                                                    DataOffset = offset
                                                }
                                            };

                                            if (parent.m_chunkList.ContainsKey(chunk.Id))
                                                parent.m_chunkList.Remove(chunk.Id);
                                            parent.m_chunkList.Add(chunk.Id, chunk);
                                        }
                                    }
                                }
                            }
                        }

                        tocPath = parent.m_fileSystem.ResolvePath(string.Format("native_patch/{0}.toc", sbPath));
                        if (tocPath != "")
                        {
                            int[] values = new int[12];
                            byte[] buffer = null;

                            using (NativeReader reader = new NativeReader(new FileStream(tocPath, FileMode.Open, FileAccess.Read), parent.m_fileSystem.CreateDeobfuscator()))
                            {
                                for (int i = 0; i < 12; i++)
                                    values[i] = reader.ReadInt(Endian.Big);
                                buffer = reader.ReadToEnd();
                            }

                            if (buffer.Length != 0)
                            {
                                NativeReader baseMf = new NativeReader(new FileStream(parent.m_fileSystem.ResolvePath(string.Format("{0}.sb", sbPath)), FileMode.Open, FileAccess.Read));
                                using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
                                {
                                    List<int> unkList = new List<int>();

                                    if (values[0] != 0)
                                    {
                                        for (int i = 0; i < values[2]; i++)
                                            unkList.Add(reader.ReadInt(Endian.Big));
                                        reader.Position = values[1] - 0x30;

                                        for (int i = 0; i < values[2]; i++)
                                        {
                                            int nameOffset = reader.ReadInt(Endian.Big);
                                            uint bundleSize = reader.ReadUInt(Endian.Big);
                                            long bundleOffset = reader.ReadLong(Endian.Big);

                                            long curPos = reader.Position;
                                            reader.Position = values[8] - 0x30 + nameOffset;

                                            string name = reader.ReadNullTerminatedString();
                                            reader.Position = curPos;

                                            int idx = baseBundles.FindIndex((BaseBundleInfo bbi) => bbi.Name.Equals(name));
                                            if (idx != -1)
                                                baseBundles.RemoveAt(idx);

                                            BaseBundleInfo bi = new BaseBundleInfo()
                                            {
                                                Name = name,
                                                Offset = bundleOffset,
                                                Size = bundleSize
                                            };
                                            patchBundles.Add(bi);
                                        }
                                    }
                                    if (values[3] != 0)
                                    {
                                        reader.Position = values[3] - 0x30;
                                        List<int> unkValues = new List<int>();
                                        for (int i = 0; i < values[5]; i++)
                                            unkValues.Add(reader.ReadInt(Endian.Big));

                                        reader.Position = values[4] - 0x30;
                                        List<Guid> chunkGuids = new List<Guid>();
                                        for (int i = 0; i < values[5]; i++)
                                        {
                                            byte[] b = reader.ReadBytes(16);
                                            Guid guid = new Guid(new byte[]
                                            {
                                                b[15], b[14], b[13], b[12],
                                                b[11], b[10], b[9], b[8],
                                                b[7], b[6], b[5], b[4],
                                                b[3], b[2], b[1], b[0]
                                            });
                                            int unk = reader.ReadInt(Endian.Big) & 0xFFFFFF;
                                            while (chunkGuids.Count <= unk)
                                                chunkGuids.Add(Guid.Empty);
                                            chunkGuids[unk / 3] = guid;
                                        }
                                        reader.Position = values[6] - 0x30;
                                        for (int i = 0; i < values[5]; i++)
                                        {
                                            byte unk = reader.ReadByte();
                                            bool isPatch = reader.ReadBoolean();
                                            byte catalogIndex = reader.ReadByte();
                                            byte casIndex = reader.ReadByte();
                                            uint offset = reader.ReadUInt(Endian.Big);
                                            uint size = reader.ReadUInt(Endian.Big);

                                            ChunkAssetEntry chunk = new ChunkAssetEntry
                                            {
                                                Id = chunkGuids[i],
                                                Size = size,
                                                Location = AssetDataLocation.CasNonIndexed,
                                                ExtraData = new AssetExtraData
                                                {
                                                    CasPath = parent.m_fileSystem.GetFilePath(catalogIndex, casIndex, isPatch),
                                                    DataOffset = offset
                                                }
                                            };
                                            if (parent.m_chunkList.ContainsKey(chunk.Id))
                                                parent.m_chunkList.Remove(chunk.Id);
                                            parent.m_chunkList.Add(chunk.Id, chunk);
                                        }
                                    }
                                }
                            }
                        }

                        if (baseBundles.Count > 0)
                        {
                            using (NativeReader baseMf = new NativeReader(new FileStream(parent.m_fileSystem.ResolvePath(string.Format("native_data/{0}.sb", sbPath)), FileMode.Open, FileAccess.Read)))
                            {
                                foreach (BaseBundleInfo bi in baseBundles)
                                {
                                    // Now process bundle
                                    BundleEntry be = new BundleEntry() { Name = bi.Name, SuperBundleId = sbIndex };
                                    parent.m_bundles.Add(be);

                                    Stream stream = baseMf.CreateViewStream(bi.Offset, bi.Size);

                                    DbObject bundle = null;
                                    using (BinarySbReader bundleReader = new BinarySbReader(stream, parent.m_fileSystem.CreateDeobfuscator()))
                                    {
                                        uint headerSize = bundleReader.ReadUInt(Endian.Big);
                                        uint dataOffset = bundleReader.ReadUInt(Endian.Big) + headerSize;
                                        uint locationOffset = bundleReader.ReadUInt(Endian.Big);
                                        bundleReader.Position += 0x14;

                                        bundle = bundleReader.ReadDbObject();
                                        bundleReader.Position = locationOffset;

                                        bool[] list = new bool[bundleReader.TotalCount];
                                        for (uint i = 0; i < bundleReader.TotalCount; i++)
                                            list[i] = bundleReader.ReadBoolean();

                                        bundleReader.Position = dataOffset;

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
                                            if (isPatch)
                                                chunk.SetValue("patch", true);
                                        }
                                    }

                                    // process assets
                                    parent.ProcessBundleEbx(bundle, parent.m_bundles.Count - 1, helper);
                                    parent.ProcessBundleRes(bundle, parent.m_bundles.Count - 1, helper);
                                    parent.ProcessBundleChunks(bundle, parent.m_bundles.Count - 1, helper);
                                }
                            }
                        }

                        if (patchBundles.Count > 0)
                        {
                            using (NativeReader patchMf = new NativeReader(new FileStream(parent.m_fileSystem.ResolvePath(string.Format("native_patch/{0}.sb", sbPath)), FileMode.Open, FileAccess.Read)))
                            {
                                foreach (BaseBundleInfo bi in patchBundles)
                                {
                                    // Now process bundle
                                    BundleEntry be = new BundleEntry() { Name = bi.Name, SuperBundleId = sbIndex };
                                    parent.m_bundles.Add(be);

                                    Stream stream = patchMf.CreateViewStream(bi.Offset, bi.Size);

                                    DbObject bundle = null;
                                    using (BinarySbReader bundleReader = new BinarySbReader(stream, parent.m_fileSystem.CreateDeobfuscator()))
                                    {
                                        uint headerSize = bundleReader.ReadUInt(Endian.Big);
                                        uint dataOffset = bundleReader.ReadUInt(Endian.Big) + headerSize;
                                        uint locationOffset = bundleReader.ReadUInt(Endian.Big);
                                        bundleReader.Position += 0x14;

                                        bundle = bundleReader.ReadDbObject();
                                        bundleReader.Position = locationOffset;

                                        bool[] list = new bool[bundleReader.TotalCount];
                                        for (uint i = 0; i < bundleReader.TotalCount; i++)
                                            list[i] = bundleReader.ReadBoolean();

                                        bundleReader.Position = dataOffset;

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
                                            if (isPatch)
                                                chunk.SetValue("patch", true);
                                        }
                                    }

                                    // process assets
                                    parent.ProcessBundleEbx(bundle, parent.m_bundles.Count - 1, helper);
                                    parent.ProcessBundleRes(bundle, parent.m_bundles.Count - 1, helper);
                                    parent.ProcessBundleChunks(bundle, parent.m_bundles.Count - 1, helper);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
