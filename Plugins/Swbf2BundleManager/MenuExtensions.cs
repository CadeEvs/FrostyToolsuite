using BundleManagerRewrite;
using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BundleManager
{
    internal class MenuExtensions
    {
        public static string BMMenuName => "Tools";
        public static string SubBMMenuName => "Bundle Manager";

        public class BundleManagerMenuExtension : MenuExtension
        {
            public override string TopLevelMenuName => BMMenuName;

            public override string SubLevelMenuName => SubBMMenuName;

            public override string MenuItemName => "Complete Bundles";

            public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Compile.png") as ImageSource;

            public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
            {
                FrostyTaskWindow.Show("Completing Bundles", "", (task) =>
                {
                    if (Cache.LoadCache(task))
                    {
                        BundleManager BM = new BundleManager(task);
                        BM.CompleteBundleManage();
                    }
                });
            });
        
        }

        public class BundleManagerLevelsMenuExtension : MenuExtension
        {
            public override string TopLevelMenuName => BMMenuName;

            public override string SubLevelMenuName => SubBMMenuName;

            public override string MenuItemName => "Complete Level Bundles";

            public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Compile.png") as ImageSource;

            public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
            {
            LevelBundlesPopup win = new LevelBundlesPopup();
            if (win.ShowDialog() == false)
                return;

                string name = win.LevelName;
                if (win.LevelName.Contains(" "))
                    name = win.LevelName.Substring(0, win.LevelName.IndexOf(' '));
                FrostyTaskWindow.Show(String.Format("Completing {0} bundles", name), "", (task) =>
                {
                    if (Cache.LoadCache(task))
                    {
                        BundleManager BM = new BundleManager(task);
                        BM.CompleteBundleManage(win.Bundles.Select(bunName => App.AssetManager.GetBundleId(bunName)).ToList());
                    }
                });
            });

        }

        public class ClearBundlesMenuExtension : MenuExtension
        {
            public override string TopLevelMenuName => BMMenuName;

            public override string SubLevelMenuName => SubBMMenuName;

            public override string MenuItemName => "Clear Bundles";

            public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Clear.png") as ImageSource;

            public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
            {
                FrostyTaskWindow.Show("Clearing Bundles", "", (task) =>
                {
                    BundleManager BM = new BundleManager(task);
                    BM.ClearBundleEdits();
                });
            });

        }

        public class ExportPrerequisitsMenuExtension : MenuExtension
        {
            public override string TopLevelMenuName => BMMenuName;

            public override string SubLevelMenuName => SubBMMenuName;

            public override string MenuItemName => "Export Preqrequists";

            public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Database.png") as ImageSource;

            public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
            {
                FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Bundle Manager Prerequisites", "*.bmpre (Bundle Manager Prerequisites File)|*.bmpre", "My Mod");
                if (sfd.ShowDialog())
                {
                    FrostyTaskWindow.Show("Exporting Prerequisites", "", (task) =>
                    {
                        BundleManagerPrerequisites prerequistes = new BundleManagerPrerequisites();
                        prerequistes.FindBundleEdits();
                        prerequistes.WriteToFile(sfd.FileName);
                    });
                };
            });

        }

        //public class ClearBundlesMenuExtension : MenuExtension
        //{
        //    public override string TopLevelMenuName => BMMenuName;

        //    public override string SubLevelMenuName => SubBMMenuName;

        //    public override string MenuItemName => "Clear Bundles";

        //    public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Clear.png") as ImageSource;

        //    public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        //    {
        //        FrostyTaskWindow.Show("Completing Bundles", "", (task) =>
        //        {
        //            BundleManager BM = new BundleManager(task, false);
        //            BM.ClearBundleEdits();
        //        });
        //    });

        //}

        //public class BundleManagerCacheGeneratorMenuExtension : MenuExtension
        //{
        //    public override string TopLevelMenuName => BMMenuName;

        //    public override string SubLevelMenuName => SubBMMenuName;

        //    public override string MenuItemName => "Generate Cache";

        //    public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Database.png") as ImageSource;

        //    public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        //    {
        //        FrostyTaskWindow.Show("Completing Bundles", "", (task) =>
        //        {
        //            BundleManager BM = new BundleManager(task, false);
        //            BM.CreateCache();
        //        });
        //    });

        //}

    }
}
