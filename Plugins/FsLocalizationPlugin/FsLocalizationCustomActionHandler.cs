using Frosty.Core;
using Frosty.Core.IO;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrostySdk.Managers.Entries;

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
            string AssetName = name;
            if (AssetName.ToLower().StartsWith("Localization/WSLocalization_".ToLower()))
            {
                AssetName = AssetName.Remove(0, 28) + " string:";
            }
            foreach (uint stringId in newFs.EnumerateStrings())
            {
                string resourceName = stringId.ToString("x8");
                string resourceType = "ebx";
                string action = "Add";

                actions.Add(AssetName + " (0x" + resourceName + ") - (" + newFs.GetString(stringId).Replace(";", ":") + ");" + resourceType + ";" + action);
                //actions.Add(AssetName + " (0x" + resourceName + ") - (" + newFs.GetString(stringId) + ");" + resourceType + ";" + action);
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
            ChunkAssetEntry HistogramEntry = am.GetChunkEntry(localizedText.HistogramChunk);

            byte[] buf2 = NativeReader.ReadInStream(am.GetChunk(HistogramEntry));
            List<char> values = ModifyHistogram(buf2, modFs);

            byte[] buf = NativeReader.ReadInStream(am.GetChunk(chunkEntry));
            buf = ModifyChunk(buf, modFs, values);

            // create new chunk entry so we can create a new runtime resource, this chunk is not actually added
            ChunkAssetEntry newChunkEntry = new ChunkAssetEntry();

            localizedText.BinaryChunkSize = (uint)buf.Length;
            newChunkEntry.LogicalSize = (uint)buf.Length;
            buf = Utils.CompressFile(buf);

            newChunkEntry.Id = chunkEntry.Id;
            newChunkEntry.Sha1 = Utils.GenerateSha1(buf);
            newChunkEntry.Size = buf.Length;
            newChunkEntry.H32 = Fnv1.HashString(origEntry.Name.ToLower());
            newChunkEntry.FirstMip = -1;

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
        private List<char> ModifyHistogram(byte[] histogramData, ModifiedFsLocalizationAsset modifiedData)
        {
            List<char> values = new List<char>();
            using (NativeReader reader = new NativeReader(new MemoryStream(histogramData)))
            {
                uint unk = reader.ReadUInt();
                long size = reader.ReadUInt();
                uint unk2 = reader.ReadUInt();

                for (int i = 0; i < (size / 2); i++)
                    values.Add(reader.ReadWideChar());
            }
            //using (NativeWriter chkwriter = new NativeWriter(new FileStream("D:\\Document\\Frosty Editor Alpha 4\\Exported.txt", FileMode.Create), false, true))
            //{
            //    foreach (ushort val in values)
            //    {
            //        chkwriter.WriteLine(val.ToString());
            //    }
            //}
            return values;
        }

        public List<byte> GetShifts(List<char> Values)
        {
            List<byte> retVals = new List<byte>();
            for (int i = 0x1FE; i >= 0x80; i--)
            {
                if (Values[i] < 0x80)
                    retVals.Add((byte)i);
            }

            return retVals;
        }

        private byte[] ModifyChunk(byte[] chunkData, ModifiedFsLocalizationAsset modifiedData, List<char> values)
        {
            string tag = null;
            List<uint> ids = null;
            List<uint> offsets = null;
            Dictionary<uint, string> strings = new Dictionary<uint, string>();

            using (NativeReader reader = new NativeReader(new MemoryStream(chunkData)))
            {
                uint magic = reader.ReadUInt();
                if (magic != 0x00039000)
                    throw new InvalidDataException();

                uint size = reader.ReadUInt();
                int count = reader.ReadInt();
                uint dataOffset = reader.ReadUInt();
                uint stringsOffset = reader.ReadUInt();
                tag = reader.ReadNullTerminatedString();

                reader.Position = dataOffset + 8;

                ids = new List<uint>(count);
                offsets = new List<uint>(count);

                for (int i = 0; i < count; i++)
                {
                    ids.Add(reader.ReadUInt());
                    if (!strings.ContainsKey(ids[i]))
                        strings.Add(ids[i], null);
                    offsets.Add(reader.ReadUInt());
                }

                for (int i = 0; i < count; i++)
                {
                    reader.Position = stringsOffset + offsets[i] + 8;
                    strings[ids[i]] = reader.ReadNullTerminatedString();

                }
            }

            foreach (uint key in modifiedData.EnumerateStrings())
            {
                string str = modifiedData.GetString(key);
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < str.Length; j++)
                {
                    char b = str[j];

                    if (b < 0x80)
                        sb.Append(b);
                    else
                    {
                        int index = values.FindIndex((char a) => { return a.Equals(b); });
                        if (index == -1)
                        {
                            App.Logger.LogWarning("Character not supported: " + b + " from string: " + key.ToString("X8"));
                            continue;
                        }

                        if (index <= 0xFF)
                            sb.Append((char)((byte)index));
                        else
                        {
                            List<byte> list = GetShifts(values);
                            foreach (byte shift in list)
                            {
                                if ((index - (values[shift] << 7)) < 0x80)
                                {
                                    sb.Append((char)shift);
                                    sb.Append((char)((byte)(index - (values[shift] << 7) + 0x80)));
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!ids.Contains(key))
                {
                    ids.Add(key);
                    strings.Add(key, sb.ToString());
                }
                else
                    strings[key] = sb.ToString();
            }

            ids.Sort();
            offsets.Clear();

            byte[] stringData = null;
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    offsets.Add((uint)writer.Position);
                    writer.WriteNullTerminatedOneBytePerCharString(strings[ids[i]]);
                }
                stringData = writer.ToByteArray();
            }
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(0x00039000);
                writer.Write(0xdeadbeef); // size
                writer.Write(ids.Count);
                writer.Write(0x8C); // dataOffset
                writer.Write((uint)(0x8C + (8 * ids.Count))); // stringOffset
                writer.WriteNullTerminatedString(tag);
                while (writer.Position < 0x8C + 8)
                    writer.Write((byte)0x00);

                for (int i = 0; i < ids.Count; i++)
                {
                    writer.Write(ids[i]);
                    writer.Write(offsets[i]);
                }

                writer.Write(stringData);
                uint size = (uint)(writer.Position - 8);
                writer.Position = 4;
                writer.Write(size);

                return writer.ToByteArray();
            }
        }
    }

    public static class WriterStringExtension
    {
        public static void WriteNullTerminatedOneBytePerCharString(this NativeWriter writer, string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                writer.Write((byte)str[i]);
            }
            writer.Write((byte)0x00);
        }
    }
}
