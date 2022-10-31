
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BiowareLocalizationPlugin.ExportImport
{
    public class XmlExporter
    {
        public static void Export(BiowareLocalizedStringDatabase textDB, string languageFormat)
        {

            FrostySaveFileDialog saveDialog = new FrostySaveFileDialog("Save Custom Texts", "*.xml (XML File)|*.xml", "LocalizedTexts_", languageFormat+"_texts");
            if (saveDialog.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting Custom Texts", "", (task) =>
                {

                    TextFile textFile = FillTextFile(textDB, languageFormat);

                    XmlSerializer serializer = new XmlSerializer(typeof(TextFile));

                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Encoding = Encoding.UTF8,
                        Indent = true
                    };

                    using (FileStream fileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                        {
                            serializer.Serialize(writer, textFile, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                        }
                    }
                });

                App.Logger.Log("Custom texts saved to {0}", saveDialog.FileName);
            }
        }

        private static TextFile FillTextFile(BiowareLocalizedStringDatabase textDB, string languageFormat)
        {
            TextFile textFile = new TextFile
            {
                LanguageFormat = languageFormat
            };

            List<TextRepresentation> textList = new List<TextRepresentation>();

            List<uint> textIdList = textDB.GetAllModifiedTextsIds(languageFormat).ToList();
            textIdList.Sort();
            foreach (uint textId in textIdList)
            {
                TextRepresentation textRepresentation = new TextRepresentation()
                {
                    TextId = textId.ToString("X8"),
                    Text = textDB.GetText(languageFormat, textId)
                };

                List<string> resourceNames = new List<string>();
                foreach(var resource in textDB.GetAllLocalizedStringResourcesForTextId(languageFormat, textId))
                {
                    resourceNames.Add(resource.Name);
                }
                textRepresentation.Resources = resourceNames.ToArray();

                textList.Add(textRepresentation);
            }

            textFile.Texts = textList.ToArray();

            return textFile;
        }
    }
}
