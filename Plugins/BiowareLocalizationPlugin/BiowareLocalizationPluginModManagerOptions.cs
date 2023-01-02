
using Frosty.Core;
using FrostySdk.Attributes;

namespace BiowareLocalizationPlugin
{
    [DisplayName("Bioware Localization Options")]
    public class BiowareLocalizationPluginOptions : OptionsExtension
    {

        // The name for the global mod manager variable.
        public static readonly string SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME = "BwLoMoShowIndividualTextIds";

        public static readonly string ASK_XML_EXPORT_OPTIONS = "BwLoEoAskXmlExportOptions";

        [Category("Mod Manager Options")]
        [Description("If enabled, all individual text ids in each resource (res) are shown in the mod manager's Actions tab. Otherwise only the resource iteself is shown as merged. This setting is only for the mod manager and has no effect in the editor.")]
        [DisplayName("Show Individual Text Ids")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool ShowIndividualTextIds { get; set; } = false;

        [Category("Editor Options")]
        [DisplayName("Ask for Xml Export Options")]
        [Description("If enabled, a popup prompt allows selecting whether to export all texts or only modified ones. If this value is false, then the default from below is used. This setting is only for the editor and has no effect in the mod manager.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool AskForXmlExportOptions { get; set; } = false;

        public override void Load()
        {
            // mod manager
            ShowIndividualTextIds = Config.Get(SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, false, ConfigScope.Global);

            // editor
            AskForXmlExportOptions = Config.Get(ASK_XML_EXPORT_OPTIONS, false, ConfigScope.Global);
        }

        public override void Save()
        {
            // mod manager
            Config.Add(SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, ShowIndividualTextIds, ConfigScope.Global);

            // editor
            Config.Add(ASK_XML_EXPORT_OPTIONS, AskForXmlExportOptions, ConfigScope.Global);
        }
    }
}
