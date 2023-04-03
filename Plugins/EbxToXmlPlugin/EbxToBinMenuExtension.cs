using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace EbxToXmlPlugin
{
    public class EbxToBinMenuExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => "Export Bulk";

        public override string MenuItemName => "Export EBX to BIN";
        public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Database.png") as ImageSource;

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

                        string filename = entry.Filename + ".bin";
                        filename = string.Concat(filename.Split(Path.GetInvalidFileNameChars()));

                        if (File.Exists(fullPath + filename))
                            continue;

                        try
                        {
                            DirectoryInfo di = new DirectoryInfo(fullPath);
                            if (!di.Exists)
                                Directory.CreateDirectory(di.FullName);
                            using (NativeWriter writer = new NativeWriter(new FileStream(fullPath + filename, FileMode.Create), false, true))
                            {
                                using (NativeReader reader = new NativeReader(App.AssetManager.GetEbxStream(entry)))
                                    writer.Write(reader.ReadToEnd());
                            }

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
