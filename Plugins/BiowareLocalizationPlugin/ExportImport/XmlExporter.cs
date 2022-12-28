
using BiowareLocalizationPlugin.LocalizedResources;
using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace BiowareLocalizationPlugin.ExportImport
{
    public class XmlExporter
    {
        public static void Export(BiowareLocalizedStringDatabase textDB, string languageFormat)
        {

            bool exportAll = false;
            if (Config.Get(BiowareLocalizationPluginOptions.ASK_XML_EXPORT_OPTIONS, false, ConfigScope.Global))
            {
                var result = FrostyMessageBox.Show("Export All Texts?\r\nSelecting 'no' will export modified texts only.", "Export Options", MessageBoxButton.YesNo);
                exportAll = MessageBoxResult.Yes == result;
            }

            FrostySaveFileDialog saveDialog = new FrostySaveFileDialog("Save Custom Texts", "*.xml (XML File)|*.xml", "LocalizedTexts_", languageFormat+"_texts");
            if (saveDialog.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting Custom Texts", "", (task) =>
                {

                    TextFile textFile = FillTextFile(textDB, languageFormat, exportAll);

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

        private static TextFile FillTextFile(BiowareLocalizedStringDatabase textDB, string languageFormat, bool exportAll)
        {

            LanguageTextsDB localizedTextDB = textDB.GetLocalizedTextDB(languageFormat);

            TextFile textFile = new TextFile
            {
                LanguageFormat = languageFormat
            };

            TextRepresentation[] textsArray = GetAllTextsToExport(localizedTextDB, exportAll);
            textFile.Texts = textsArray;

            DeclinatedAdjectiveRepresentation[] declinatedAdjectives = GetAllDeclinatedAdjectivesToExport(localizedTextDB, exportAll);
            textFile.DeclinatedAdjectives = declinatedAdjectives;

            return textFile;
        }

        private static List<uint> GetAllTextIdsToExport(LanguageTextsDB localizedTextDB, bool exportAll)
        {

            List<uint> textIdList;
            if (exportAll)
            {
                textIdList = localizedTextDB.GetAllTextIds().ToList();
            }
            else
            {
                textIdList = localizedTextDB.GetAllModifiedTextsIds().ToList();
            }

            textIdList.Sort();
            return textIdList;
        }

        private static TextRepresentation[] GetAllTextsToExport(LanguageTextsDB localizedTextDB, bool exportAll)
        {

            List<uint> textIdList = GetAllTextIdsToExport(localizedTextDB, exportAll);
            List<TextRepresentation> textList = new List<TextRepresentation>(textIdList.Count);


            foreach (uint textId in textIdList)
            {
                TextRepresentation textRepresentation = new TextRepresentation()
                {
                    TextId = textId.ToString("X8"),
                    Text = localizedTextDB.GetText(textId)
                };

                List<string> resourceNames = new List<string>();
                foreach (var resource in localizedTextDB.GetAllResourcesForTextId(textId))
                {
                    resourceNames.Add(resource.Name);
                }
                textRepresentation.Resources = resourceNames.ToArray();

                textList.Add(textRepresentation);
            }

            return textList.ToArray();
        }

        private static DeclinatedAdjectiveRepresentation[] GetAllDeclinatedAdjectivesToExport(LanguageTextsDB localizedTextDB, bool exportAll)
        {

            List<string> resourceNames;
            if(exportAll)
            {
                resourceNames = localizedTextDB.GetAllResourceNamesWithDeclinatedAdjectives().ToList();
            }
            else
            {
                resourceNames = localizedTextDB.GetAllResourceNamesWithModifiedDeclinatedAdjectives().ToList();
            }

            if(resourceNames.Count == 0)
            {
                return null;
            }

            return GetAllDeclinatedAdjectivesToExport(localizedTextDB, resourceNames, exportAll);
        }

        private static DeclinatedAdjectiveRepresentation[] GetAllDeclinatedAdjectivesToExport(LanguageTextsDB localizedTextDB, IEnumerable<string> resourceNames, bool exportAll)
        {

            List< DeclinatedAdjectiveRepresentation > adjectiveRepresentations = new List< DeclinatedAdjectiveRepresentation >();
            foreach(string resourceName in resourceNames)
            {

                IEnumerable<uint> adjectiveIds;

                if(exportAll)
                {
                    adjectiveIds = localizedTextDB.GetAllDeclinatedAdjectiveIdsFromResource(resourceName);
                }
                else
                {
                    adjectiveIds = localizedTextDB.GetModifiedDeclinatedAdjectiveIdsFromResource(resourceName);
                }

                foreach(uint adjectiveId in adjectiveIds)
                {

                    adjectiveRepresentations.Add(CreateDeclinationEntry(localizedTextDB, resourceName, adjectiveId));
                }
            }

            return adjectiveRepresentations.ToArray();
        }

        private static DeclinatedAdjectiveRepresentation CreateDeclinationEntry(LanguageTextsDB languageTextsDB, string resourceName, uint adjectiveId)
        {
            List<string> declinations = languageTextsDB.GetDeclinatedAdjectives(resourceName, adjectiveId);

            return new DeclinatedAdjectiveRepresentation()
            {
                Resource = resourceName,
                AdjectiveId = adjectiveId.ToString("X8"),
                Declinations = declinations.ToArray()
            };
        }

    }
}
