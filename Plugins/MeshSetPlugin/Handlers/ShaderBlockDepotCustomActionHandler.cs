using Frosty.Core.IO;
using Frosty.Core.Mod;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using MeshSetPlugin.Resources;
using System.Collections.Generic;
using FrostySdk.Managers.Entries;

namespace MeshSetPlugin.Handlers
{
    public class ShaderBlockDepotCustomActionHandler : ICustomActionHandler
    {
        public HandlerUsage Usage => HandlerUsage.Merge;

        private class ShaderBlockDepotResource : EditorModResource
        {
            public static uint Hash => 0x89ef2205;
            public override ModResourceType Type => ModResourceType.Res;

            private uint resType;
            private ulong resRid;
            private byte[] resMeta;

            public ShaderBlockDepotResource(ResAssetEntry entry, FrostyModWriter.Manifest manifest)
                : base(entry)
            {
                ModifiedResource md = entry.ModifiedEntry.DataObject as ModifiedResource;
                byte[] data = md.Save();

                name = entry.Name.ToLower();
                sha1 = Utils.GenerateSha1(data);
                resourceIndex = manifest.Add(sha1, data);
                size = data.Length;
                handlerHash = (int)Hash;

                resType = entry.ResType;
                resRid = entry.ResRid;
                resMeta = (entry.ModifiedEntry.ResMeta != null) ? entry.ModifiedEntry.ResMeta : entry.ResMeta;
            }

            public override void Write(NativeWriter writer)
            {
                base.Write(writer);

                writer.Write(resType);
                writer.Write(resRid);
                writer.Write(resMeta.Length);
                writer.Write(resMeta);
            }
        }

        public void SaveToMod(FrostyModWriter writer, AssetEntry entry)
        {
            writer.AddResource(new ShaderBlockDepotResource(entry as ResAssetEntry, writer.ResourceManifest));
        }

        public object Load(object existing, byte[] newData)
        {
            ModifiedShaderBlockDepot newMsbd = (ModifiedShaderBlockDepot)ModifiedResource.Read(newData);
            ModifiedShaderBlockDepot oldMsbd = (ModifiedShaderBlockDepot)existing;

            if (oldMsbd == null)
            {
                return newMsbd;
            }

            oldMsbd.Merge(newMsbd);
            return oldMsbd;
        }

        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            ModifiedShaderBlockDepot msbd = data as ModifiedShaderBlockDepot;
            ShaderBlockDepot sbd = am.GetResAs<ShaderBlockDepot>(am.GetResEntry(origEntry.Name), msbd);

            byte[] newData = sbd.ToBytes();
            outData = Utils.CompressFile(newData);

            ResAssetEntry entry = origEntry as ResAssetEntry;
            entry.Sha1 = Utils.GenerateSha1(outData);
            entry.OriginalSize = newData.Length;
            entry.Size = outData.Length;
            entry.ResMeta = sbd.ResourceMeta;
        }

        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {
            return new List<string>();
        }
    }
}
