using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace FrostySdk.Managers
{
    public partial class AssetManager
    {
        internal class FifaAssetLoader : IAssetLoader
        {
            internal struct BundleFileInfo
            {
                public BundleFileInfo(int index, int offset, int size)
                {
                    Index = index;
                    Offset = offset;
                    Size = size;
                }

                public int Index;
                public int Offset;
                public int Size;
            }

            public void Load(AssetManager parent, BinarySbDataHelper helper)
            {
                // get second encryption key
                byte[] key = KeyManager.Instance.GetKey("Key2");
                foreach (CatalogInfo catalog in parent.fs.EnumerateCatalogInfos())
                {
                    foreach (string sbName in catalog.SuperBundles.Keys)
                    {
                        SuperBundleEntry sbe = parent.superBundles.Find((SuperBundleEntry a) => a.Name == sbName);
                        int sbIndex = -1;

                        if (sbe != null)
                        {
                            sbIndex = parent.superBundles.IndexOf(sbe);
                        }
                        else
                        {
                            parent.superBundles.Add(new SuperBundleEntry { Name = sbName });
                            sbIndex = parent.superBundles.Count - 1;
                        }

                        parent.WriteToLog("Loading data ({0})", sbName);
                        parent.superBundles.Add(new SuperBundleEntry { Name = sbName });

                        string sbPath = sbName;
                        if (catalog.SuperBundles[sbName])
                            sbPath = sbName.Replace("win32", catalog.Name);

#if ENABLE_LCU
                        string tocPath = parent.fs.ResolvePath(string.Format("LCU/{0}.toc", sbPath));
                        if (tocPath == "")
                            tocPath = parent.fs.ResolvePath(string.Format("{0}.toc", sbPath));
#else
                        string tocPath = parent.fs.ResolvePath(string.Format("{0}.toc", sbPath));
#endif

                        if (tocPath != "")
                        {
                            int bundlesOffset = 0;
                            int chunksOffset = 0;
                            byte[] buffer = null;

                            using (NativeReader reader = new NativeReader(new FileStream(tocPath, FileMode.Open, FileAccess.Read), parent.fs.CreateDeobfuscator()))
                            {
                                uint magic = reader.ReadUInt();
                                bundlesOffset = reader.ReadInt() - 0x0C;
                                chunksOffset = reader.ReadInt() - 0x0C;
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

                            if (buffer.Length != 0)
                            {
                                using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
                                {
                                    //List<int> unkList = new List<int>();

                                    if (bundlesOffset > 0)
                                    {
                                        reader.Position = bundlesOffset;
                                        int bundleCount = reader.ReadInt();

                                        for (int i = 0; i < bundleCount; i++)
                                            reader.ReadInt(); //unkList.Add(reader.ReadInt());

                                        for (int i = 0; i < bundleCount; i++)
                                        {
                                            parent.logger.Log("progress:{0}", (i / (double)bundleCount) * 100.0d);
                                            int offset = reader.ReadInt() - 0x0C;

                                            long pos = reader.Position;
                                            reader.Position = offset;

                                            int nameOffset = reader.ReadInt() - 1;

                                            List<BundleFileInfo> files = new List<BundleFileInfo>();
                                            MemoryStream ms = new MemoryStream();

                                            while (true)
                                            {
                                                int fileIndex = reader.ReadInt();
                                                int fileOffset = reader.ReadInt();
                                                int fileSize = reader.ReadInt();

                                                using (NativeReader casReader = new NativeReader(new FileStream(parent.fs.ResolvePath(parent.fs.GetFilePath(fileIndex & 0x7FFFFFFF)), FileMode.Open, FileAccess.Read)))
                                                {
                                                    casReader.Position = fileOffset;
                                                    ms.Write(casReader.ReadBytes(fileSize), 0, fileSize);
                                                }

                                                files.Add(new BundleFileInfo(fileIndex & 0x7FFFFFFF, fileOffset, fileSize));
                                                if ((fileIndex & 0x80000000) == 0)
                                                    break;
                                            }

                                            reader.Position = nameOffset - 0x0C;

                                            int off = 0;
                                            string bundleName = "";

                                            do
                                            {
                                                string tmp = reader.ReadNullTerminatedString();
                                                off = reader.ReadInt() - 1;

                                                bundleName += tmp;
                                                if (off != -1)
                                                    reader.Position = off - 0x0C;

                                            } while (off != -1);

                                            bundleName = Utils.ReverseString(bundleName);
                                            reader.Position = pos;

                                            // Now process bundle
                                            BundleEntry be = new BundleEntry { Name = bundleName, SuperBundleId = sbIndex };
                                            parent.bundles.Add(be);

                                            using (BinarySbReader bundleReader = new BinarySbReader(ms, 0, parent.fs.CreateDeobfuscator()))
                                            {
                                                DbObject bundle = bundleReader.ReadDbObject();

                                                BundleFileInfo fileInfo = files[0];
                                                long dataOffset = fileInfo.Offset + (bundle.GetValue<long>("dataOffset") + 4);
                                                long sizeLeft = fileInfo.Size - (bundle.GetValue<long>("dataOffset") + 4);
                                                int fileIndex = 0;

                                                foreach (DbObject ebx in bundle.GetValue<DbObject>("ebx"))
                                                {
                                                    if (sizeLeft == 0)
                                                    {
                                                        fileInfo = files[++fileIndex];
                                                        sizeLeft = fileInfo.Size;
                                                        dataOffset = fileInfo.Offset;
                                                    }

                                                    int size = ebx.GetValue<int>("size");
                                                    ebx.SetValue("offset", dataOffset);
                                                    ebx.SetValue("cas", fileInfo.Index);
                                                    dataOffset += size;
                                                    sizeLeft -= size;
                                                }
                                                foreach (DbObject res in bundle.GetValue<DbObject>("res"))
                                                {
                                                    if (sizeLeft == 0)
                                                    {
                                                        fileInfo = files[++fileIndex];
                                                        sizeLeft = fileInfo.Size;
                                                        dataOffset = fileInfo.Offset;
                                                    }

                                                    int size = res.GetValue<int>("size");
                                                    res.SetValue("offset", dataOffset);
                                                    res.SetValue("cas", fileInfo.Index);
                                                    dataOffset += size;
                                                    sizeLeft -= size;
                                                }
                                                foreach (DbObject chunk in bundle.GetValue<DbObject>("chunks"))
                                                {
                                                    if (sizeLeft == 0)
                                                    {
                                                        fileInfo = files[++fileIndex];
                                                        sizeLeft = fileInfo.Size;
                                                        dataOffset = fileInfo.Offset;
                                                    }

                                                    int size = chunk.GetValue<int>("size");
                                                    chunk.SetValue("offset", dataOffset);
                                                    chunk.SetValue("cas", fileInfo.Index);
                                                    dataOffset += size;
                                                    sizeLeft -= size;
                                                }

                                                Debug.Assert(sizeLeft == 0);

                                                // process assets
                                                parent.ProcessBundleEbx(bundle, parent.bundles.Count - 1, helper);
                                                parent.ProcessBundleRes(bundle, parent.bundles.Count - 1, helper);
                                                parent.ProcessBundleChunks(bundle, parent.bundles.Count - 1, helper);
                                            }

                                        }
                                    }

                                    if (chunksOffset > 0)
                                    {
                                        reader.Position = chunksOffset;
                                        int chunksCount = reader.ReadInt();

                                        //unkList = new List<int>();
                                        for (int i = 0; i < chunksCount; i++)
                                            reader.ReadInt(); //unkList.Add(reader.ReadInt());

                                        for (int i = 0; i < chunksCount; i++)
                                        {
                                            int offset = reader.ReadInt();

                                            long pos = reader.Position;
                                            reader.Position = offset - 0x0C;

                                            Guid guid = reader.ReadGuid();
                                            int fileIndex = reader.ReadInt();
                                            int dataOffset = reader.ReadInt();
                                            int dataSize = reader.ReadInt();

                                            if (!parent.chunkList.ContainsKey(guid))
                                                parent.chunkList.Add(guid, new ChunkAssetEntry());

                                            ChunkAssetEntry chunk = parent.chunkList[guid];
                                            chunk.Id = guid;
                                            chunk.Size = dataSize;
                                            chunk.Location = AssetDataLocation.CasNonIndexed;
                                            chunk.ExtraData = new AssetExtraData
                                            {
                                                CasPath = parent.fs.GetFilePath(fileIndex),
                                                DataOffset = dataOffset
                                            };

                                            parent.chunkList[guid].IsTocChunk = true;
                                            reader.Position = pos;
                                        }
                                    }
                                }
                            }
                        }
                        sbIndex++;
                    }
                }
            }
        }
    }
}
