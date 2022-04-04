using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls.Editors;
using Frosty.Core.Misc;
using FrostySdk.Attributes;
using FrostySdk.IO;
using MeshSetPlugin.Editors;
using System.Collections.Generic;

namespace MeshSetPlugin
{
    public class FrostyDisplayAdapterEditor : FrostyCustomComboDataEditor<string, string>
    {
    }

    [DisplayName("Mesh Options")]
    public class MeshOptions : OptionsExtension
    {
        [Category("Rendering")]
        [DisplayName("Display Adapter")]
        [Description("Which display adapter to use in the rendering viewport")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        [Editor(typeof(FrostyDisplayAdapterEditor))]
        public CustomComboData<string, string> DisplayAdapter { get; set; }
        [Category("Rendering")]
        [DisplayName("Shadows Enabled")]
        public bool RenderShadowsEnabled { get; set; } = true;
        [Category("Rendering")]
        [DisplayName("Shadow Resolution")]
        [DependsOn("RenderShadowsEnabled")]
        public int RenderShadowResolution { get; set; } = 2048;
        [Category("Rendering")]
        [DisplayName("HBAO Enabled")]
        public bool RenderHBAOEnabled { get; set; } = true;
        [Category("Rendering")]
        [DisplayName("TXAA Enabled")]
        public bool RenderTXAAEnabled { get; set; } = true;

        [Category("Viewer")]
        [DisplayName("Show Grid")]
        [Description("Determines wether or not the grid will be shown by default")]
        public bool ShowGrid { get; set; }
        [Category("Viewer")]
        [DisplayName("Show Ground")]
        [Description("Determines wether or not the ground will be shown by default")]
        public bool ShowFloor { get; set; }

        [Category("Export/Import")]
        [DisplayName("Export Skeleton")]
        [Description("Determines the default skeleton selected when exporting meshes")]
        [Editor(typeof(FrostySkeletonEditor))]
        public string ExportSkeletonAsset { get; set; }
        [Category("Export/Import")]
        [DisplayName("Import Skeleton")]
        [Description("Determines the default skeleton selected when importing meshes")]
        [Editor(typeof(FrostySkeletonEditor))]
        public string ImportSkeletonAsset { get; set; }

        public override void Load()
        {
            ShowGrid = Config.Get<bool>("MeshSetViewerShowGrid", true);
            ShowFloor = Config.Get<bool>("MeshSetViewerShowFloor", true);
            ExportSkeletonAsset = Config.Get<string>("MeshSetExportSkeleton", "", ConfigScope.Game);
            ImportSkeletonAsset = Config.Get<string>("MeshSetImportSkeleton", "", ConfigScope.Game);
            //ShowGrid = Config.Get<bool>("MeshSetViewer", "ShowGrid", true);
            //ShowFloor = Config.Get<bool>("MeshSetViewer", "ShowFloor", true);
            //ExportSkeletonAsset = Config.Get<string>("MeshSetExport", "Skeleton", "");
            //ImportSkeletonAsset = Config.Get<string>("MeshSetImport", "Skeleton", "");

            List<string> adapters = GetDisplayAdapters();
            DisplayAdapter = new CustomComboData<string, string>(adapters, adapters) {SelectedIndex = Config.Get<int>("RenderAdapterIndex", 0)};
            RenderShadowsEnabled = Config.Get<bool>("RenderShadowsEnabled", true);
            RenderShadowResolution = Config.Get<int>("RenderShadowRes", 2048);
            RenderHBAOEnabled = Config.Get<bool>("RenderHBAOEnabled", true);
            RenderTXAAEnabled = Config.Get<bool>("RenderTXAAEnabled", true);

            //DisplayAdapter = new CustomComboData<string, string>(adapters, adapters) {SelectedIndex = Config.Get<int>("Render", "AdapterIndex", 0)};
            //RenderShadowsEnabled = Config.Get<bool>("Render", "ShadowsEnabled", true);
            //RenderShadowResolution = Config.Get<int>("Render", "ShadowRes", 2048);
            //RenderHBAOEnabled = Config.Get<bool>("Render", "HBAOEnabled", true);
            //RenderTXAAEnabled = Config.Get<bool>("Render", "TXAAEnabled", true);
        }

        public override void Save()
        {
            Config.Add("MeshSetViewerShowGrid", ShowGrid);
            Config.Add("MeshSetViewerShowFloor", ShowFloor);
            Config.Add("MeshSetExportSkeleton", ExportSkeletonAsset, ConfigScope.Game);
            Config.Add("MeshSetImportSkeleton", ImportSkeletonAsset, ConfigScope.Game);

            Config.Add("RenderAdapterIndex", DisplayAdapter.SelectedIndex);
            Config.Add("RenderShadowsEnabled", RenderShadowsEnabled);
            Config.Add("RenderShadowRes", RenderShadowResolution);
            Config.Add("RenderHBAOEnabled", RenderHBAOEnabled);
            Config.Add("RenderTXAAEnabled", RenderTXAAEnabled);

            Config.Save();

            //Config.Add("MeshSetViewer", "ShowGrid", ShowGrid);
            //Config.Add("MeshSetViewer", "ShowFloor", ShowFloor);
            //Config.Add("MeshSetExport", "Skeleton", ExportSkeletonAsset);
            //Config.Add("MeshSetImport", "Skeleton", ImportSkeletonAsset);

            //Config.Add("Render", "AdapterIndex", DisplayAdapter.SelectedIndex);
            //Config.Add("Render", "ShadowsEnabled", RenderShadowsEnabled);
            //Config.Add("Render", "ShadowRes", RenderShadowResolution);
            //Config.Add("Render", "HBAOEnabled", RenderHBAOEnabled);
            //Config.Add("Render", "TXAAEnabled", RenderTXAAEnabled);
        }

        public override bool Validate()
        {
            if (RenderShadowResolution > 4096)
            {
                FrostyMessageBox.Show("Shadow resolution exceeds maximum allowed size of 4096", "Frosty Editor");
                return false;
            }
            
            if (RenderShadowResolution == 0 || (RenderShadowResolution & (RenderShadowResolution - 1)) != 0)
            {
                FrostyMessageBox.Show("Shadow resolution must be power of 2", "Frosty Editor");
                return false;
            }
            return true;
        }

        private static List<string> GetDisplayAdapters()
        {
            List<string> displayAdapters = new List<string>();
            SharpDX.DXGI.Factory factory = new SharpDX.DXGI.Factory1();

            foreach (var adapter in factory.Adapters)
            {
                displayAdapters.Add(adapter.Description.Description);
            }

            factory.Dispose();
            return displayAdapters;
        }
    }
}
