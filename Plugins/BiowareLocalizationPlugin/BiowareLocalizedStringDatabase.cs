using BiowareLocalizationPlugin.Controls;
using BiowareLocalizationPlugin.LocalizedResources;
using Frosty.Core;
using FrostySdk.Managers.Entries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace BiowareLocalizationPlugin
{
    public class BiowareLocalizedStringDatabase : ILocalizedStringDatabase
    {

        /// <summary>
        /// The default language to operate with if no other one is given.
        /// </summary>
        public string DefaultLanguage { get; private set; }

        /// <summary>
        /// Holds all the languages supported by the local game and their bundles
        /// </summary>
        private SortedDictionary<string, HashSet<string>> m_languageLocalizationBundles;

        /// <summary>
        /// Dictionary of all currently loaded localized texts.
        /// </summary>
        private readonly Dictionary<string, LanguageTextsDB> m_loadedLocalizedTextDBs = new Dictionary<string, LanguageTextsDB>();

        /// <summary>
        /// marker whether or not this was already initialized.
        /// </summary>
        private bool m_initialized = false;

        /// <summary>
        /// Initializes the db.
        /// </summary>
        public void Initialize()
        {

            DefaultLanguage = "LanguageFormat_" + Config.Get<string>("Language", "English", scope: ConfigScope.Game);

            if (m_initialized)
            {
                return;
            }

            m_languageLocalizationBundles = GetLanguageDictionary();

            LanguageTextsDB defaultLocalizedTexts = new LanguageTextsDB();
            defaultLocalizedTexts.Init(DefaultLanguage, m_languageLocalizationBundles[DefaultLanguage]);

            m_loadedLocalizedTextDBs.Add(DefaultLanguage, defaultLocalizedTexts);

            m_initialized = true;
        }

        /// <summary>
        /// Tries to return the text for the given uid, throws an exception if the text id is not known.
        /// @see #FindText
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetString(uint id)
        {
            return GetText(DefaultLanguage, id);
        }

        public string GetString(string stringId)
        {

            bool canRead = uint.TryParse(stringId, NumberStyles.HexNumber, null, out uint textId);
            if (canRead)
            {
                return GetString(textId);
            }

            App.Logger.LogError("Cannot read given textId <{0}>", stringId);
            return stringId;

        }

        /// <summary>
        /// Returns the language db for the requested language format, loading it if necessary.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <returns></returns>
        public LanguageTextsDB GetLocalizedTextDB(string languageFormat)
        {
            bool isLoaded = m_loadedLocalizedTextDBs.TryGetValue(languageFormat, out LanguageTextsDB localizedTextDb);
            if (!isLoaded)
            {
                if (!m_languageLocalizationBundles.ContainsKey(languageFormat))
                {
                    throw new ArgumentException(string.Format("LanguageFormat <{0}> does not exist in this game!", languageFormat));
                }

                localizedTextDb = new LanguageTextsDB();
                localizedTextDb.Init(languageFormat, m_languageLocalizationBundles[languageFormat]);

                m_loadedLocalizedTextDBs.Add(languageFormat, localizedTextDb);
            }
            return localizedTextDb;
        }

        /// <summary>
        /// Tries to return the text for the given uid. Returns an error message if the text does not exist.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="textId"></param>
        /// <returns></returns>
        public string GetText(string languageFormat, uint textId)
        {
            return GetLocalizedTextDB(languageFormat).GetText(textId);
        }

        public IEnumerable<uint> EnumerateStrings()
        {
            return GetAllTextIds(DefaultLanguage);
        }

        /// <summary>
        /// Returns a language specific list of all text ids.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <returns></returns>
        public IEnumerable<uint> GetAllTextIds(string languageFormat)
        {
            return GetLocalizedTextDB(languageFormat).GetAllTextIds();
        }

        /// <summary>
        /// Returns only the ids of modified or new texts.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <returns></returns>
        public IEnumerable<uint> GetAllModifiedTextsIds(string languageFormat)
        {
            return GetLocalizedTextDB(languageFormat).GetAllModifiedTextsIds();
        }

        /// <summary>
        /// Tries to return the text for the given uid, returns null if the textid does not exist.
        /// @see #GetString
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="textId"></param>
        /// <returns></returns>
        public string FindText(string languageFormat, uint textId)
        {
            return GetLocalizedTextDB(languageFormat).FindText(textId);
        }

        /// <summary>
        /// Returns the list of LocalizedStringResource in which the given text id can be found.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="textId">The text id to look for.</param>
        /// <returns>All resources in which the text id can be found.</returns>
        public IEnumerable<LocalizedStringResource> GetAllLocalizedStringResourcesForTextId(string languageFormat, uint textId)
        {
            return GetLocalizedTextDB(languageFormat).GetAllResourcesForTextId(textId);
        }

        /// <summary>
        /// Returns the list of LocalizedStringResource in which the given text id can be found by default.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="textId">The text id to look for.</param>
        /// <returns>All resources in which the text id can be found by default.</returns>
        public IEnumerable<LocalizedStringResource> GetDefaultLocalizedStringResourcesForTextId(string languageFormat, uint textId)
        {
            return GetLocalizedTextDB(languageFormat).GetDefaultResourcesForTextId(textId);
        }

        /// <summary>
        /// Returns the list of LocalizedStringResource in which the given text id can be found due to a mod.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="textId">The text id to look for.</param>
        /// <returns>All resources in which the text id can be found due to a mod.</returns>
        public IEnumerable<LocalizedStringResource> GetAddedLocalizedStringResourcesForTextId(string languageFormat, uint textId)
        {
            return GetLocalizedTextDB(languageFormat).GetAddedResourcesForTextId(textId);
        }

        /// <summary>
        /// Returns the names of all found resources
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllResourceNames(string languageFormat)
        {
            return GetLocalizedTextDB(languageFormat).GetAllResourceNames();
        }

        /// <summary>
        /// Sets a text into a single resource
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="resourceNames"></param>
        /// <param name="textId"></param>
        /// <param name="text"></param>
        public void SetText(string languageFormat, IEnumerable<string> resourceNames, uint textId, string text)
        {

            LanguageTextsDB localizedDB = GetLocalizedTextDB(languageFormat);
            foreach (string resourceName in resourceNames)
            {
                localizedDB.SetText(resourceName, textId, text);
            }

            localizedDB.UpdateTextCache(textId, text);
        }

        /// <summary>
        /// Removes the given text with the given id from the given resources for the given language.
        /// </summary>
        /// <param name="languageFormat"></param>
        /// <param name="resourceNames"></param>
        /// <param name="textId"></param>
        public void RemoveText(string languageFormat, IEnumerable<string> resourceNames, uint textId)
        {
            LanguageTextsDB localizedDB = GetLocalizedTextDB(languageFormat);
            foreach (string resourceName in resourceNames)
            {
                localizedDB.RemoveText(resourceName, textId);
            }

            localizedDB.RemoveTextFromCache(textId);
        }

        public void RevertText(string languageFormat, uint textId)
        {
            LanguageTextsDB localizedDB = GetLocalizedTextDB(languageFormat);
            localizedDB.RevertText(textId);
        }

        public IEnumerable<string> GellAllLanguages()
        {
            return new List<string>(m_languageLocalizationBundles.Keys);
        }

        // basically identical to SetText, this method was added in the 1.06 beta interface
        public void SetString(uint id, string value)
        {
            LanguageTextsDB localizedDB = GetLocalizedTextDB(DefaultLanguage);

            IEnumerable<LocalizedStringResource> allTextResources = localizedDB.GetAllResourcesForTextId(id);
            IEnumerable<string> textResourceNames = allTextResources.Select(resource => resource.Name);

            SetText(DefaultLanguage, textResourceNames, id, value);
        }

        // // Basically identical to SetText, this method was added in the 1.06 beta interface
        public void SetString(string id, string value)
        {
            bool canRead = uint.TryParse(id, NumberStyles.HexNumber, null, out uint textId);
            if (canRead)
            {
                SetString(textId, value);
            }

            App.Logger.LogError("Cannot read given textId <{0}>", id);
        }

        // // Basically identical to RevertText, this method was added in the 1.06 beta interface
        public void RevertString(uint id)
        {
            RevertText(DefaultLanguage, id);
        }

        // Returns whether the text with the given id was altered.
        // Implements the interface method added in 1.0.6beta
        public bool isStringEdited(uint id)
        {
            LanguageTextsDB localizedDB = GetLocalizedTextDB(DefaultLanguage);

            IEnumerable<uint> modifiedTextsIds = localizedDB.GetAllModifiedTextsIds();
            foreach (uint textId in modifiedTextsIds)
            {
                if (textId == id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Opens a window to add strings to the localized string database.
        /// </summary>
        /// Note This method came with the 1.06.beta1 and i feel really uncomfortable displaying an edit dialog directly from what is supposed to be abackend class >_<
        public void AddStringWindow()
        {

            AddEditWindow editWindow = new AddEditWindow(this, DefaultLanguage)
            {
                Owner = Application.Current.MainWindow
            };
            editWindow.Init(0);
            _ = editWindow.ShowDialog();
        }

        // This method came with 1.06.beta1, and i still believe bulk operations to be more a risk of breaking texts than working properly
        // - or at least my implementation of that function would be ;D
        // But now sequencial replacements should work somewhat decently
        public void BulkReplaceWindow()
        {
            List<string> textsWithId = new List<string>();

            List<uint> textIds = GetAllTextIds(DefaultLanguage).ToList();
            textIds.Sort();

            foreach (uint textId in textIds)
            {
                string text = GetText(DefaultLanguage, textId);
                textsWithId.Add(textId.ToString("X8") + " - " + text);
            }

            ReplaceWindow replaceWindow = new ReplaceWindow(this, DefaultLanguage, textsWithId)
            {
                Owner = Application.Current.MainWindow
            };
            replaceWindow.ShowDialog();
        }

        /// <summary>
        /// Retrieves a collection of string IDs that were modified from the localized string database.
        /// This method came into the interface in 1.06.beta1, and is virtually identical to GetAllModifiedTextsIds
        /// </summary>
        /// <returns>A collection of string IDs, or an empty collection if no modified strings exist.</returns>
        public IEnumerable<uint> EnumerateModifiedStrings()
        {
            return GetAllModifiedTextsIds(DefaultLanguage);
        }

        public IEnumerable<string> GetAllResourceNamesWithDeclinatedAdjectives(string languageFormat)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetAllResourceNamesWithDeclinatedAdjectives();
        }

        public IEnumerable<string> GetAllResourceNamesWithModifiedDeclinatedAdjectives(string languageFormat)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetAllResourceNamesWithModifiedDeclinatedAdjectives();
        }

        public IEnumerable<uint> GetAllDeclinatedAdjectiveIdsFromResource(string languageFormat, string resourceName)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetAllDeclinatedAdjectiveIdsFromResource(resourceName);
        }

        public IEnumerable<uint> GetModifiedDeclinatedAdjectiveIdsFromResource(string languageFormat, string resourceName)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetModifiedDeclinatedAdjectiveIdsFromResource(resourceName);
        }

        public List<string> GetDeclinatedAdjectives(string languageFormat, string resourceName, uint adjectiveId)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetDeclinatedAdjectives(resourceName, adjectiveId);
        }

        public void SetDeclinatedAdjectve(string languageFormat, string resourceName, uint adjectiveId, List<string> aAdjectives)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            textDb.SetDeclinatedAdjectve(resourceName, adjectiveId, aAdjectives);
        }

        public void RevertDeclinatedAdjective(string languageFormat, string resourceName, uint adjectiveId)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            textDb.RevertDeclinatedAdjective(resourceName, adjectiveId);
        }

        public IEnumerable<uint> GetAllTextIdsFromResource(string languageFormat, string resourceName)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetAllTextIdsFromResource(resourceName);
        }

        public IEnumerable<uint> GetAllModifiedTextIdsFromResource(string languageFormat, string resourceName)
        {
            LanguageTextsDB textDb = GetLocalizedTextDB(languageFormat);
            return textDb.GetAllModifiedTextIdsFromResource(resourceName);
        }

        /// <summary>
        /// Fills the language dictionary with all available languages and their bundles.
        /// </summary>
        /// <returns>Sorted Dictionary of LangugeFormat names and their text super bundles paths.</returns>
        private static SortedDictionary<string, HashSet<string>> GetLanguageDictionary()
        {

            var languagesRepository = new SortedDictionary<string, HashSet<string>>();

            // There is no need to also search for 'LocalizedStringPatchTranslationsConfiguration', these are also found via their base type
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx("LocalizedStringTranslationsConfiguration"))
            {
                // read localization config
                dynamic localizationAsset = App.AssetManager.GetEbx(entry).RootObject;

                // iterate through language to bundle lists
                foreach (dynamic languageBundleListEntry in localizationAsset.LanguagesToBundlesList)
                {
                    string languageName = languageBundleListEntry.Language.ToString();
                    HashSet<string> bundleNames;
                    if (languagesRepository.ContainsKey(languageName))
                    {
                        bundleNames = languagesRepository[languageName];
                    }
                    else
                    {
                        bundleNames = new HashSet<string>();
                        languagesRepository[languageName] = bundleNames;
                    }

                    foreach (string bundlepath in languageBundleListEntry.BundlePaths)
                    {
                        bundleNames.Add(bundlepath);
                    }
                }
            }

            return languagesRepository;
        }
    }
}
