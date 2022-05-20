using Frosty.Core;
using Frosty.Core.IO;
using Frosty.Core.Mod;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using TestPlugin.Managers;

namespace TestPlugin.Handlers
{
    public class InitFsCustomActionHandler : ICustomAssetCustomActionHandler
    {
        public HandlerUsage Usage => HandlerUsage.Merge;

        public class InitFsresource : EditorModResource
        {
            public override ModResourceType Type => ModResourceType.FsFile;
            public InitFsresource(FsFileEntry entry, FrostyModWriter.Manifest manifest)
                : base(entry)
            {
                name = entry.Name;
                byte[] data = null;
                if (entry.HasModifiedData)
                {
                    userData = entry.ModifiedEntry.UserData;
                    using (DbWriter dbWriter = new DbWriter(new MemoryStream()))
                    {
                        dbWriter.Write(entry.ModifiedEntry.DataObject as DbObject);

                        size = dbWriter.Length;

                        data = dbWriter.ToByteArray();

                        resourceIndex = manifest.Add(data);
                        sha1 = Utils.GenerateSha1(data);
                    }
                }
            }
        }

        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {
            return new List<string>(0);
        }

        public object Load(object existing, byte[] newData)
        {
            throw new NotImplementedException();
        }

        // dont bother
        public void LoadFromProject(DbObject project)
        {
            throw new NotImplementedException();
        }

        public void LoadFromProject(uint version, NativeReader reader, string type)
        {
            if (type != "fs")
                return;

            int numItems = reader.ReadInt();
            for (int i = 0; i < numItems; i++)
            {
                DbObject fileStub = null;
                using (DbReader dbReader = new DbReader(reader.BaseStream, null))
                    fileStub = dbReader.ReadDbObject();

                DbObject file = fileStub.GetValue<DbObject>("$file");
                string name = file.GetValue<string>("name");

                FsFileEntry entry = App.AssetManager.GetCustomAssetEntry<FsFileEntry>("fs", name);

                if (entry != null)
                {
                    MemoryStream ms = new MemoryStream();
                    using (DbWriter dbWriter = new DbWriter(ms))
                        dbWriter.Write(fileStub);

                    App.AssetManager.ModifyCustomAsset("fs", name, ms.ToArray());
                }
            }
        }

        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            throw new NotImplementedException();
        }

        public void SaveToMod(FrostyModWriter writer)
        {
            foreach (FsFileEntry fsEntry in App.AssetManager.EnumerateCustomAssets("fs", true))
            {
                writer.AddResource(new InitFsresource(fsEntry, writer.ResourceManifest));
            }

        }

        public bool SaveToProject(NativeWriter writer)
        {
            writer.WriteNullTerminatedString("fs");

            long sizePosition = writer.Position;
            writer.Write(0xDEADBEEF);

            int count = 0;
            using (DbWriter bdWriter = new DbWriter(writer.BaseStream))
            {
                foreach (FsFileEntry fsEntry in App.AssetManager.EnumerateCustomAssets("fs", true))
                {
                    bdWriter.Write(fsEntry.ModifiedEntry.DataObject as DbObject);
                    count++;
                }
            }

            writer.Position = sizePosition;
            writer.Write(count);
            writer.Position = writer.Length;
            return true;
        }
    }
}
