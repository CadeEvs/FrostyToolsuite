

using BiowareLocalizationPlugin.Controls;
using BiowareLocalizationPlugin.LocalizedResources;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Xml.Serialization;


namespace BiowareLocalizationPlugin.ExportImport
{
    public class XmlImporter
    {
        public static void Import(BiowareLocalizedStringDatabase textDb)
        {
            FrostyOpenFileDialog openDialog = new FrostyOpenFileDialog("Import Custom Texts", "*.xml (XML File)|*.xml", "LocalizedTexts_");
            if (openDialog.ShowDialog())
            {

                string fileName = openDialog.FileName;
                TextFile textFile = ReadTextFile(fileName);

                if (textFile != null)
                {
                    App.Logger.Log("Importing localized texts form File <{0}>", fileName);
                    textFile = AdaptTextFile(textDb, textFile);
                    if (textFile != null)
                    {
                        ImportTexts(textDb, textFile);
                    }
                }
            }
        }

        /// <summary>
        /// Step one: Try to parse the file - if there are problems they likely occur here, resulting in an exception that rolls back the whole process.
        /// </summary>
        /// <param name="fileUri"></param>
        /// <returns></returns>
        private static TextFile ReadTextFile(string fileUri)
        {

            TextFile textFile = null;
            FrostyTaskWindow.Show("Reading Custom Texts", "", (task) =>
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(TextFile));
                try
                {
                    using (FileStream stream = new FileStream(fileUri, FileMode.Open, FileAccess.Read))
                    {
                        textFile = deserializer.Deserialize(stream) as TextFile;
                    }
                }
                catch (Exception e) when
                (
                    e is IOException
                    || e is SecurityException
                    || e is UnauthorizedAccessException
                    || e is InvalidOperationException
                )
                {
                    textFile = null;
                    App.Logger.LogError("Could not read the given file <{0}> or deserialize it!", fileUri);
                    App.Logger.LogError("Exception was: <{0}> {1}", e.GetType(), e.Message);
                }
            });

            return textFile;
        }

        /// <summary>
        /// Step two: It will likely be necessary to replace certain entries during import, like language, or resource names. This is the place to do that.
        /// </summary>
        /// <param name="originalFile"></param>
        /// <returns>The adapted textfile or null, if the import should be aborted.</returns>
        private static TextFile AdaptTextFile(BiowareLocalizedStringDatabase textDb, TextFile originalFile)
        {

            ImportTargetDialog importDialog = new ImportTargetDialog(textDb, originalFile);

            bool? alteredResult = importDialog.ShowDialog();
            if (alteredResult == true)
            {
                return importDialog.SaveValue;
            }

            App.Logger.Log("Import Aborted");
            return null;

        }

        /// <summary>
        /// Step three: Actually import the parsed texts. There still might be errors preventing certain texts, but that should only prevent individual texts from importing properly.
        /// </summary>
        /// <param name="textDb"></param>
        /// <param name="textFile"></param>
        private static void ImportTexts(BiowareLocalizedStringDatabase textDb, TextFile textFile)
        {

            FrostyTaskWindow.Show("Importing Custom Texts", "", (task) =>
            {

                string language = textFile.LanguageFormat;

                try
                {
                    foreach (TextRepresentation textRepresentation in textFile.Texts)
                    {
                        ImportText(textDb, language, textRepresentation);
                    }

                    ImportDeclinatedAdjectives(textDb, language, textFile.DeclinatedAdjectives);

                    App.Logger.Log("Texts imported into <{0}>", language);
                }
                catch (InvalidOperationException e)
                {
                    // this is thrown e.g., if the language does not exist in the local game copy
                    App.Logger.LogError("Could not import the texts: {0}", e.Message);
                }

            });
        }

        private static void ImportText(BiowareLocalizedStringDatabase textDb, string language, TextRepresentation textRepresentation)
        {

            string stringIdAsText = textRepresentation.TextId;
            bool canRead = uint.TryParse(stringIdAsText, NumberStyles.HexNumber, null, out uint textId);
            if (!canRead)
            {
                App.Logger.LogWarning("Text with id <{0}> could not be imported! Text Id cannot be parsed!", stringIdAsText);
                return;
            }

            try
            {
                textDb.SetText(language, textRepresentation.Resources, textId, textRepresentation.Text);
            }
            catch (Exception e)
            {
                // ArgumentException or KeyException are thrown if the language or resource do not exist
                App.Logger.LogError("Text with id <{0}> could not be imported: {1}", stringIdAsText, e.Message);
            }
        }

        private static void ImportDeclinatedAdjectives(BiowareLocalizedStringDatabase textDb, string language, DeclinatedAdjectiveRepresentation[] nullableAdjectiveRepresentations)
        {
            if (nullableAdjectiveRepresentations != null)
            {
                LanguageTextsDB localizedTextDb = textDb.GetLocalizedTextDB(language);
                foreach (var adjectiveRepresentation in nullableAdjectiveRepresentations)
                {
                    ImportDeclinatedAdjective(localizedTextDb, adjectiveRepresentation);
                }
            }
        }

        private static void ImportDeclinatedAdjective(LanguageTextsDB localizedTextDb, DeclinatedAdjectiveRepresentation adjectiveRepresentation)
        {

            string adjectiveIdAsText = adjectiveRepresentation.AdjectiveId;
            bool canRead = uint.TryParse(adjectiveIdAsText, NumberStyles.HexNumber, null, out uint adjectiveId);
            if (!canRead)
            {
                App.Logger.LogWarning("Adjective with id <{0}> could not be imported! Adjective Id cannot be parsed!", adjectiveIdAsText);
                return;
            }

            List<string> declinationsList = new List<string>(adjectiveRepresentation.Declinations);

            try
            {
                localizedTextDb.SetDeclinatedAdjectve(adjectiveRepresentation.Resource, adjectiveId, declinationsList);
            }
            catch (Exception e)
            {
                // this is thrown if the resource does not exist - should not actually happen with the selector dialog before...
                App.Logger.LogError("Declinated adjective with id <{0}> could not be imported: {1}", adjectiveIdAsText, e.Message);
            }
        }

    }
}
