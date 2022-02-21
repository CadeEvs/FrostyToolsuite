using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Interfaces;
using System.Windows;

namespace VersionDataPlugin
{
    [Description("This asset shows the version info for the build of this game")]
    public class VersionDataOverride : BaseTypeOverride
    {
        [Category("Version")]
        [DisplayName("Disclaimer")]
        public BaseFieldOverride disclaimer { get; set; }
        [Category("Version")]
        public BaseFieldOverride Version { get; set; }
        [Category("Version")]
        public BaseFieldOverride DateTime { get; set; }
        [Category("Version")]
        public BaseFieldOverride BranchId { get; set; }
        [Category("Version")]
        public BaseFieldOverride DataBranchId { get; set; }
        [Category("Version")]
        public BaseFieldOverride GameName { get; set; }
    }

    public class VersionDataEditor : FrostyAssetEditor
    {
        static VersionDataEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VersionDataEditor), new FrameworkPropertyMetadata(typeof(VersionDataEditor)));
        }

        public VersionDataEditor(ILogger inLogger)
            : base(inLogger)
        {
        }
    }
}
