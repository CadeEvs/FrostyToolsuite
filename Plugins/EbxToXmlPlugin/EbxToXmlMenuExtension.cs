using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;

namespace EbxToXmlPlugin
{
    public class EbxToXmlMenuExtension : MenuExtension
    {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/EbxToXmlPlugin;component/Images/EbxToXml.png") as ImageSource;

        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Export EBX to XML";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string outDir = fbd.SelectedPath;
                FrostyTaskWindow.Show("Exporting EBX", "", (task) =>
                {
                    uint totalCount = App.AssetManager.GetEbxCount();
                    uint idx = 0;

                    foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx())
                    {
                        task.Update(entry.Name, (idx++ / (double)totalCount) * 100.0d);

                        string fullPath = outDir + "/" + entry.Path + "/";

                        string filename = entry.Filename + ".xml";
                        filename = string.Concat(filename.Split(Path.GetInvalidFileNameChars()));

                        if (File.Exists(fullPath + filename))
                            continue;

                        try
                        {
                            DirectoryInfo di = new DirectoryInfo(fullPath);
                            if (!di.Exists)
                                Directory.CreateDirectory(di.FullName);

                            EbxAsset asset = App.AssetManager.GetEbx(entry);
                            using (EbxXmlWriter writer = new EbxXmlWriter(new FileStream(fullPath + filename, FileMode.Create), App.AssetManager))
                                writer.WriteObjects(asset.Objects);
                        }
                        catch (Exception)
                        {
                            App.Logger.Log("Failed to export {0}", entry.Filename);
                        }
                    }
                });

                FrostyMessageBox.Show("Successfully exported EBX to " + outDir, "Frosty Editor");
            }
        });
    }
}
