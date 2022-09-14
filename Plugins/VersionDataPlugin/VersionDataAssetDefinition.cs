using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace VersionDataPlugin
{
    public class VersionDataAssetDefinition : AssetDefinition
    {
        protected static ImageSource versionDataImageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/VersionDataPlugin;component/Images/VersionDataFileType.png") as ImageSource;

        public override void GetSupportedExportTypes(List<AssetExportType> exportTypes)
        {
            exportTypes.Add(new AssetExportType("txt", "Text Files"));
            base.GetSupportedExportTypes(exportTypes);
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new VersionDataEditor(logger);
        }

        public override ImageSource GetIcon()
        {
            return versionDataImageSource;
        }

        public override bool Export(EbxAssetEntry entry, string path, string filterType)
        {
            if (!base.Export(entry, path, filterType))
            {
                if (filterType == "txt")
                {
                    EbxAsset versionAsset = App.AssetManager.GetEbx(entry);
                    dynamic versionData = versionAsset.RootObject;

                    using (NativeWriter writer = new NativeWriter(new FileStream(path, FileMode.Create)))
                    {
                        writer.WriteLine("DataVersion");
                        writer.WriteLine("-----------");
                        writer.WriteLine("Generated " + DateTime.Now);
                        writer.WriteLine("disclaimer: " + versionData.disclaimer);
                        writer.WriteLine("version: " + versionData.Version);
                        writer.WriteLine("datetime: " + versionData.DateTime);
                        writer.WriteLine("branchid: " + versionData.BranchId);
                        writer.WriteLine("databranchid: " + versionData.DataBranchId);
                        writer.WriteLine("gamename: " + versionData.GameName);
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
