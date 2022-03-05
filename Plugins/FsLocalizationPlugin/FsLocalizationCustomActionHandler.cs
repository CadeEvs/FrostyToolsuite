using Frosty.Core.IO;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.IO;

namespace FsLocalizationPlugin
{
    public class FsLocalizationCustomActionHandler : ICustomActionHandler
    {
        public HandlerUsage Usage => HandlerUsage.Merge;

        private class FsLocalizationResource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.Ebx;
            public FsLocalizationResource(EbxAssetEntry entry, FrostyModWriter.Manifest manifest)
                : base(entry)
            {
                ModifiedResource md = entry.ModifiedEntry.DataObject as ModifiedResource;
                byte[] data = md.Save();

                name = entry.Name.ToLower();
                sha1 = Utils.GenerateSha1(data);
                resourceIndex = manifest.Add(sha1, data);
                size = data.Length;
                handlerHash = Fnv1.HashString(entry.Type.ToLower());
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);
            }
        }

        #region -- Editor Specific --

        public void SaveToMod(FrostyModWriter writer, AssetEntry entry)
        {
            writer.AddResource(new FsLocalizationResource(entry as EbxAssetEntry, writer.ResourceManifest));
        }

        #endregion

        #region -- Mod Manager Specific --

        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {
            ModifiedFsLocalizationAsset newFs = (ModifiedFsLocalizationAsset)ModifiedResource.Read(data);
            List<string> actions = new List<string>();

            foreach (uint stringId in newFs.EnumerateStrings())
            {
                string resourceName = stringId.ToString("x8");
                string resourceType = "ebx";
                string action = "Add";

                actions.Add(name + " (0x" + resourceName + ");" + resourceType + ";" + action);
            }

            return actions;
        }

        public object Load(object existing, byte[] newData)
        {
            ModifiedFsLocalizationAsset newFs = (ModifiedFsLocalizationAsset)ModifiedResource.Read(newData);
            ModifiedFsLocalizationAsset oldFs = (ModifiedFsLocalizationAsset)existing;

            if (oldFs == null)
                return newFs;

            oldFs.Merge(newFs);
            return oldFs;
        }

        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            ModifiedFsLocalizationAsset modFs = data as ModifiedFsLocalizationAsset;

            EbxAsset ebxAsset = am.GetEbx(am.GetEbxEntry(origEntry.Name));
            dynamic localizedText = ebxAsset.RootObject;

            ChunkAssetEntry chunkEntry = am.GetChunkEntry(localizedText.BinaryChunk);
            ChunkAssetEntry newChunkEntry = new ChunkAssetEntry();

            byte[] buf = NativeReader.ReadInStream(am.GetChunk(chunkEntry));
            buf = ModifyChunk(buf, modFs);

            localizedText.BinaryChunkSize = (uint)buf.Length;
            newChunkEntry.LogicalSize = (uint)buf.Length;
            buf = Utils.CompressFile(buf);

            newChunkEntry.Id = chunkEntry.Id;
            newChunkEntry.Sha1 = Utils.GenerateSha1(buf);
            newChunkEntry.Size = buf.Length;
            newChunkEntry.H32 = Fnv1.HashString(origEntry.Name.ToLower());
            newChunkEntry.FirstMip = -1;
            newChunkEntry.IsTocChunk = true;

            runtimeResources.AddResource(new RuntimeChunkResource(newChunkEntry), buf);
            
            using (EbxBaseWriter writer = EbxBaseWriter.CreateWriter(new MemoryStream()))
            {
                writer.WriteAsset(ebxAsset);
                origEntry.OriginalSize = writer.Length;
                outData = Utils.CompressFile(writer.ToByteArray());
            }

            origEntry.Size = outData.Length;
            origEntry.Sha1 = Utils.GenerateSha1(outData);
        }

        #endregion

        private byte[] ModifyChunk(byte[] chunkData, ModifiedFsLocalizationAsset modifiedData)
        {
            using (NativeReader reader = new NativeReader(new MemoryStream(chunkData)))
            {
                uint magic = reader.ReadUInt();
                if (magic != 0x00039000)
                    throw new InvalidDataException();

                uint size = reader.ReadUInt();
                uint unk = reader.ReadUInt();
                uint dataOffset = reader.ReadUInt();
                uint stringsOffset = reader.ReadUInt();
                string tag = reader.ReadNullTerminatedString();

                reader.Position = dataOffset + 8;

                List<uint> ids = new List<uint>();
                List<uint> offsets = new List<uint>();
                Dictionary<uint, string> strings = new Dictionary<uint, string>();

                while (reader.Position != stringsOffset + 8)
                {
                    ids.Add(reader.ReadUInt());
                    offsets.Add(reader.ReadUInt());
                }

                for (int i = 0; i < ids.Count; i++)
                {
                    reader.Position = stringsOffset + offsets[i] + 8;

                    string str = reader.ReadNullTerminatedString();
                    string dstStr = "";

                    for (int j = 0; j < str.Length; j++)
                    {
                        byte b = (byte)str[j];

                        if (b < 0x80)
                            dstStr += (char)b;
                        //else
                        //{
                        //    ushort tmp = values[b];
                        //    if (tmp >= 0x80)
                        //        dstStr += (char)tmp;
                        //    else
                        //    {
                        //        b = (byte)str[++j];
                        //        if (b >= 0x80)
                        //        {
                        //            b -= 0x80;
                        //            dstStr += (char)values[(b + (tmp << 7))];
                        //        }
                        //    }
                        //}
                    }

                    if (!strings.ContainsKey(ids[i]))
                    {
                        strings.Add(ids[i], dstStr);
                    }
                }

                foreach (uint key in modifiedData.EnumerateStrings())
                {
                    if (!ids.Contains(key))
                    {
                        ids.Add(key);
                        strings.Add(key, modifiedData.GetString(key));
                    }
                    else
                    {
                        strings[key] = modifiedData.GetString(key);
                    }
                }

                ids.Sort();
                offsets.Clear();

                long stringsSize = 0;
                stringsOffset = 0;
                size = 0;

                byte[] stringData;
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        offsets.Add((uint)writer.BaseStream.Position);
                        stringsSize += strings[ids[i]].Length + 1;

                        writer.WriteNullTerminatedString(strings[ids[i]]);
                    }
                    stringData = writer.ToByteArray();
                }
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    stringsOffset = (uint)(0x8C + (8 * ids.Count));
                    size = (uint)(0x8C + (8 * ids.Count) + stringsSize);

                    writer.Write(0x00039000);
                    writer.Write(size); // size
                    writer.Write(ids.Count);
                    writer.Write(0x8C); // dataOffset
                    writer.Write(stringsOffset); // stringOffset
                    writer.WriteNullTerminatedString(tag);
                    for (int i = 0; i < 0x79; i++)
                        writer.Write((byte)0x00);

                    for (int i = 0; i < ids.Count; i++)
                    {
                        writer.Write(ids[i]);
                        writer.Write(offsets[i]);
                    }

                    writer.Write(stringData);

                    return writer.ToByteArray();
                }
            }
        }
    }
}
