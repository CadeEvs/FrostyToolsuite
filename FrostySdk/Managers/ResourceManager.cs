using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrostySdk.Managers
{
    public class ResourceManager : ILoggable
    {
        private readonly FileSystem fs;
        private ILogger logger;

        private Dictionary<Sha1, CatResourceEntry> resourceEntries = new Dictionary<Sha1, CatResourceEntry>();
        private Dictionary<Sha1, CatPatchEntry> patchEntries = new Dictionary<Sha1, CatPatchEntry>();
        private Dictionary<int, string> casFiles = new Dictionary<int, string>();
        //private Dictionary<string, byte[]> keys = new Dictionary<string, byte[]>();

        public ResourceManager(FileSystem inFs)
        {
            fs = inFs;
        }

        // initialize manager
        public void Initialize()
        {
            // bind compression libraries
            ZStd.Bind();
            Oodle.Bind(fs.BasePath);

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedEdge)
            {
                LoadDas();
            }

            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.Fifa19 && ProfilesLibrary.DataVersion != (int)ProfileVersion.Madden20)
            {
                WriteToLog("Loading Catalogs");
                foreach (string catalogName in fs.Catalogs)
                {
                    LoadCatalog("native_data/" + catalogName + "/cas.cat");
                    LoadCatalog("native_patch/" + catalogName + "/cas.cat");
                }
            }

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20)
            {
                // load dictionary from memoryFs (used for decompressing ebx)
                ZStd.SetDictionary(fs.GetFileFromMemoryFs("Dictionaries/ebx.dict"));
            }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
            {
                // load casencrypt.yaml from memoryFs
                using (NativeReader reader = new NativeReader(new MemoryStream(fs.GetFileFromMemoryFs("Scripts/CasEncrypt.yaml"))))
                {
                    byte[] key = null;
                    while (reader.Position < reader.Length)
                    {
                        string line = reader.ReadLine();
                        if (line.Contains("keyid:"))
                        {
                            string[] arr = line.Split(':');
                            KeyManager.Instance.AddKey(arr[1].Trim(), key);
                        }
                        else if (line.Contains("key:"))
                        {
                            string[] arr = line.Split(':');
                            string keyStr = arr[1].Trim();

                            key = new byte[keyStr.Length / 2];
                            for(int i = 0; i < keyStr.Length / 2; i++)
                                key[i] = Convert.ToByte(keyStr.Substring(i * 2, 2), 16);
                        }
                    }
                }
            }
        }

        // unpatched data from cas
        public Stream GetResourceData(Sha1 sha1)
        {
            if (patchEntries.ContainsKey(sha1))
            {
                CatPatchEntry patchEntry = patchEntries[sha1];
                return GetResourceData(patchEntry.BaseSha1, patchEntry.DeltaSha1);
            }

            if (!resourceEntries.ContainsKey(sha1))
                return null;

            CatResourceEntry entry = resourceEntries[sha1];
            byte[] buffer = null;

            if (entry.IsEncrypted && !KeyManager.Instance.HasKey(entry.KeyId))
                return null;

            using (NativeReader casReader = new NativeReader(new FileStream(casFiles[entry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
            {
                using (CasReader reader = new CasReader(casReader.CreateViewStream(entry.Offset, entry.Size), (entry.IsEncrypted) ? KeyManager.Instance.GetKey(entry.KeyId) : null, entry.EncryptedSize))
                    buffer = reader.Read();
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }
        public Stream GetRawResourceData(Sha1 sha1)
        {
            if (!resourceEntries.ContainsKey(sha1))
                return null;

            CatResourceEntry entry = resourceEntries[sha1];
            byte[] buffer = null;

            if (entry.IsEncrypted && !KeyManager.Instance.HasKey(entry.KeyId))
                return null;

            using (NativeReader casReader = new NativeReader(new FileStream(casFiles[entry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
            {
                using (NativeReader reader = new NativeReader(casReader.CreateViewStream(entry.Offset, entry.Size)))
                    buffer = reader.ReadToEnd();
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }

        // patched data from cas
        public Stream GetResourceData(Sha1 baseSha1, Sha1 deltaSha1)
        {
            if (!resourceEntries.ContainsKey(baseSha1) || !resourceEntries.ContainsKey(deltaSha1))
                return null;

            CatResourceEntry baseEntry = resourceEntries[baseSha1];
            CatResourceEntry deltaEntry = resourceEntries[deltaSha1];

            byte[] buffer = null;
            using (NativeReader baseReader = new NativeReader(new FileStream(casFiles[baseEntry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
            {
                using (NativeReader deltaReader = (deltaEntry.ArchiveIndex == baseEntry.ArchiveIndex) ? baseReader : new NativeReader(new FileStream(casFiles[deltaEntry.ArchiveIndex], FileMode.Open, FileAccess.Read)))
                {
                    byte[] baseKey = (baseEntry.IsEncrypted && KeyManager.Instance.HasKey(baseEntry.KeyId)) ? KeyManager.Instance.GetKey(baseEntry.KeyId) : null;
                    byte[] deltaKey = (deltaEntry.IsEncrypted && KeyManager.Instance.HasKey(deltaEntry.KeyId)) ? KeyManager.Instance.GetKey(deltaEntry.KeyId) : null;

                    if (baseEntry.IsEncrypted && baseKey == null || deltaEntry.IsEncrypted && deltaKey == null)
                        return null;

                    using (CasReader reader = new CasReader(baseReader.CreateViewStream(baseEntry.Offset, baseEntry.Size), baseKey, baseEntry.EncryptedSize, deltaReader.CreateViewStream(deltaEntry.Offset, deltaEntry.Size), deltaKey, deltaEntry.EncryptedSize))
                        buffer = reader.Read();
                }
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }

        // unpatched data from binary superbundle
        public Stream GetResourceData(string superBundleName, long offset, long size)
        {
            byte[] buffer = null;
            using (NativeReader bufferReader = new NativeReader(new FileStream(fs.ResolvePath(string.Format("{0}", superBundleName)), FileMode.Open, FileAccess.Read)))
            {
                using (CasReader reader = new CasReader(bufferReader.CreateViewStream(offset, size)))
                    buffer = reader.Read();
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }
        public Stream GetRawResourceData(string superBundleName, long offset, long size)
        {
            byte[] buffer = null;
            using (NativeReader bufferReader = new NativeReader(new FileStream(fs.ResolvePath(string.Format("{0}", superBundleName)), FileMode.Open, FileAccess.Read)))
            {
                using (NativeReader reader = new NativeReader(bufferReader.CreateViewStream(offset, size)))
                    buffer = reader.ReadToEnd();
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }

        // patched data from binary superbundle (stored in cache)
        public Stream GetResourceData(long offset, long size)
        {
            byte[] buffer = null;
            using (NativeReader bufferReader = new NativeReader(new FileStream(fs.CacheName + "_sbdata.cas", FileMode.Open, FileAccess.Read)))
            {
                using (CasReader reader = new CasReader(bufferReader.CreateViewStream(offset, size)))
                    buffer = reader.Read();
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }
        public Stream GetRawResourceData(long offset, long size)
        {
            byte[] buffer = null;
            using (NativeReader bufferReader = new NativeReader(new FileStream(fs.CacheName + "_sbdata.cas", FileMode.Open, FileAccess.Read)))
            {
                using (NativeReader reader = new NativeReader(bufferReader.CreateViewStream(offset, size)))
                    buffer = reader.ReadToEnd();
            }

            return (buffer != null) ? new MemoryStream(buffer) : null;
        }

        // data from buffer (ie. modified or inline data)
        public Stream GetResourceData(byte[] buffer)
        {
            byte[] outBuffer = null;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (CasReader reader = new CasReader(ms))
                    outBuffer = reader.Read();
            }

            return (outBuffer != null) ? new MemoryStream(outBuffer) : null;
        }

        public Sha1 GetBaseSha1(Sha1 sha1) => patchEntries.ContainsKey(sha1) ? patchEntries[sha1].BaseSha1 : sha1;

        public bool IsEncrypted(Sha1 sha1)
        {
            if (resourceEntries.ContainsKey(sha1) && resourceEntries[sha1].IsEncrypted)
                return true;
            if (patchEntries.ContainsKey(sha1))
            {
                // is patched
                CatPatchEntry patchEntry = patchEntries[sha1];

                // doesnt matter if base or delta are encrypted, asset is considered encrypted
                if (resourceEntries.ContainsKey(patchEntry.BaseSha1) && resourceEntries[patchEntry.BaseSha1].IsEncrypted)
                    return true;
                if (resourceEntries.ContainsKey(patchEntry.DeltaSha1) && resourceEntries[patchEntry.DeltaSha1].IsEncrypted)
                    return true;
            }
            return false;
        }

        public void SetLogger(ILogger inLogger) => logger = inLogger;

        public void ClearLogger() => logger = null;

        // adds catalog entires to manager
        private void LoadCatalog(string filename)
        {
            string fullPath = fs.ResolvePath(filename);
            if (!File.Exists(fullPath))
                return;

            using (CatReader reader = new CatReader(new FileStream(fullPath, FileMode.Open, FileAccess.Read), fs.CreateDeobfuscator()))
            {
                for (int i = 0; i < reader.ResourceCount; i++)
                {
                    CatResourceEntry entry = reader.ReadResourceEntry();
                    entry.ArchiveIndex = AddCas(filename, entry.ArchiveIndex);

                    if (entry.LogicalOffset == 0 && !resourceEntries.ContainsKey(entry.Sha1))
                        resourceEntries.Add(entry.Sha1, entry);
                }

                for (int i = 0; i < reader.EncryptedCount; i++)
                {
                    CatResourceEntry entry = reader.ReadEncryptedEntry();
                    entry.ArchiveIndex = AddCas(filename, entry.ArchiveIndex);

                    if (entry.LogicalOffset == 0 && !resourceEntries.ContainsKey(entry.Sha1))
                        resourceEntries.Add(entry.Sha1, entry);
                }

                for (int i = 0; i < reader.PatchCount; i++)
                {
                    CatPatchEntry entry = reader.ReadPatchEntry();
                    if (!patchEntries.ContainsKey(entry.Sha1))
                        patchEntries.Add(entry.Sha1, entry);
                }
            }
        }

        private void LoadDas()
        {
            string dasDalPath = fs.ResolvePath("das.dal");
            List<Tuple<string, int>> dasFiles = new List<Tuple<string, int>>();

            using (NativeReader reader = new NativeReader(new FileStream(dasDalPath, FileMode.Open, FileAccess.Read)))
            {
                int numFiles = reader.ReadByte();
                for (int i = 0; i < numFiles; i++)
                {
                    string filename = reader.ReadSizedString(0x40);
                    int numEntries = reader.ReadInt();

                    string dasPath = fs.ResolvePath("das_" + filename + ".das");
                    int hash = Fnv1.HashString(dasPath);
                    casFiles.Add(i, dasPath);

                    using (NativeReader dasReader = new NativeReader(new FileStream(dasPath, FileMode.Open, FileAccess.Read)))
                    {
                        long runningOffset = numEntries * 0x18;
                        for (int j = 0; j < numEntries; j++)
                        {
                            Sha1 sha1 = dasReader.ReadSha1();
                            uint size = dasReader.ReadUInt();
                            long offset = runningOffset;

                            runningOffset += size;
                            CatResourceEntry entry = new CatResourceEntry()
                            {
                                Sha1 = sha1,
                                Offset = (uint)offset,
                                Size = size,
                                ArchiveIndex = casFiles.Count - 1
                            };
                            resourceEntries.Add(sha1, entry);
                        }
                    }
                }
            }
        }

        // adds a cas file to the manager
        private int AddCas(string catPath, int archiveIndex)
        {
            string casFilename = catPath.Substring(0, catPath.Length - 7) + "cas_" + archiveIndex.ToString("d2") + ".cas";
            int hash = Fnv1.HashString(casFilename);

            if (!casFiles.ContainsKey(hash))
                casFiles.Add(hash, fs.ResolvePath(casFilename));

            return hash;
        }

        private void WriteToLog(string text, params object[] vars) => logger?.Log(text, vars);
    }
}
