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
using System.Linq;

namespace EbxToXmlPlugin
{
    public class ExportResMenuExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => "Export Bulk";

        public override string MenuItemName => "Export Res";
        public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Database.png") as ImageSource;

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string outDir = fbd.SelectedPath;
                FrostyTaskWindow.Show("Exporting Res", "", (task) =>
                {
                    uint totalCount = (uint)App.AssetManager.EnumerateRes().ToList().Count;
                    uint idx = 0;

                    foreach (ResAssetEntry entry in App.AssetManager.EnumerateRes())
                    {
                        task.Update(entry.Name, (idx++ / (double)totalCount) * 100.0d);

                        string fullPath = outDir + "/" + entry.Path + "/";

                        string filename = entry.Filename + ".res";
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
                                using (NativeReader reader = new NativeReader(App.AssetManager.GetRes(entry)))
                                    writer.Write(reader.ReadToEnd());
                            }

                        }
                        catch (Exception)
                        {
                            App.Logger.Log("Failed to export {0}", entry.Name);
                        }
                    }
                });

                FrostyMessageBox.Show("Successfully exported res to " + outDir, "Frosty Editor");
            }
        });
    }
}
