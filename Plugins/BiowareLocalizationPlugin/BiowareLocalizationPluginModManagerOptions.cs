
using Frosty.Core;
using FrostySdk.Attributes;

namespace BiowareLocalizationPlugin
{
    [DisplayName("Bioware Localization Options")]
    public class BiowareLocalizationPluginOptions : OptionsExtension
    {

        // The name for the global mod manager variable.
        public static readonly string SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME = "BwLoMoShowIndividualTextIds";

        // The name of the option to enable exporting everything in a resource to xml.
        public static readonly string ASK_XML_EXPORT_OPTIONS = "BwLoEoAskXmlExportOptions";

        // The name of the option to enable verification text printing.
        public static readonly string PRINT_VERIFICATION_TEXTS = "BwLocalizationPlugin.PrintVerificationTexts";

        // Taken from Margolissa, this option keeps the original data for the texts behind the dataoffset, only adding edits after that. Might be faster than writing everything from scratch.
        public static readonly string REUSE_ORIGINAL_RESOURCE_TEXTBITS = "BwLocalizationPlugin.ReuseOriginalResourceTextBits";

        [Category("Mod Manager Options")]
        [DisplayName("Show Individual Text Ids")]
        [Description("If enabled, all individual text ids in each resource (res) are shown in the mod manager's Actions tab. Otherwise only the resource iteself is shown as merged. This setting is only for the mod manager and has no effect in the editor. This is a global setting.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool ShowIndividualTextIds { get; set; } = false;

        [Category("Editor Options")]
        [DisplayName("Ask for Xml Export Options")]
        [Description("If enabled, a popup prompt allows selecting whether to export all texts or only modified ones. If this value is false, then the default from below is used. This setting is only for the editor and has no effect in the mod manager. This is a global setting.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool AskForXmlExportOptions { get; set; } = false;

        [Category("General Options")]
        [DisplayName("Log verification texts")]
        [Description("If enabled, a lot of verification log entries will be created for each loaded and editet text resources. Should only be enabled to try and debug issues. This is a game specific setting.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool PrintVerificationTexts { get; set; } = false;

        [Category("General Options")]
        [DisplayName("Reuse original resource's text bits")]
        [Description("If enabled, the original encoded texts are used again and altered texts only appended to the end. Faster than writing everything from scratch when there are few edits that do not affect the encoding. Based on Margolissas idea. This is a game specific setting.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool ReuseOriginalResourceTextBits { get; set; } = false;

        public override void Load()
        {
            // mod manager
            ShowIndividualTextIds = Config.Get(SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, false, ConfigScope.Global);

            // editor
            AskForXmlExportOptions = Config.Get(ASK_XML_EXPORT_OPTIONS, false, ConfigScope.Global);

            // both
            PrintVerificationTexts = Config.Get(PRINT_VERIFICATION_TEXTS, false, ConfigScope.Game);
            ReuseOriginalResourceTextBits = Config.Get(REUSE_ORIGINAL_RESOURCE_TEXTBITS, false, ConfigScope.Game);
        }

        public override void Save()
        {
            // mod manager
            Config.Add(SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, ShowIndividualTextIds, ConfigScope.Global);

            // editor
            Config.Add(ASK_XML_EXPORT_OPTIONS, AskForXmlExportOptions, ConfigScope.Global);

            // both
            Config.Add(PRINT_VERIFICATION_TEXTS, PrintVerificationTexts, ConfigScope.Game);
            Config.Add(REUSE_ORIGINAL_RESOURCE_TEXTBITS, ReuseOriginalResourceTextBits, ConfigScope.Game);
        }
    }
}
