
using Frosty.Core;
using FrostySdk.Attributes;

namespace BiowareLocalizationPlugin
{
    [DisplayName("Bioware Localization Options")]
    public class BiowareLocalizationPluginModManagerOptions : OptionsExtension
    {

        // The name for the global mod manager variable.
        public static readonly string SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME = "BwLoMoShowIndividualTextIds";

        [Category("General")]
        [Description("If enabled, all individual text ids in each resource (res) are shown in the Actions tab. Otherwise only the resource iteself is shown as merged.")]
        [DisplayName("Show Individual Text Ids")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool ShowIndividualTextIds { get; set; } = false;

        public override void Load()
        {
            ShowIndividualTextIds = Config.Get(SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, false, ConfigScope.Global);
        }

        public override void Save()
        {
            Config.Add(SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, ShowIndividualTextIds, ConfigScope.Global);
        }
    }
}
