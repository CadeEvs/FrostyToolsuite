using Frosty.Core;
using FrostySdk.Ebx;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;

namespace BiowareLocalizationPlugin
{
    public class BiowareLocalizedStringDatabase : ILocalizedStringDatabase
    {
        private Dictionary<uint, string> strings = new Dictionary<uint, string>();

        public void Initialize()
        {
            LoadLocalizedStringConfiguration("LocalizedStringTranslationsConfiguration");
            LoadLocalizedStringConfiguration("LocalizedStringPatchTranslationsConfiguration");
        }

        public List<string> GetLanguages()
        {
            List<string> languages = new List<string>();
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx("LocalizedStringTranslationsConfiguration"))
            {
                // read master localization asset
                dynamic localizationAsset = App.AssetManager.GetEbx(entry).RootObject;

                // iterate through localized texts
                foreach (PointerRef pointer in localizationAsset.Languages)
                {
                    EbxAssetEntry textEntry = App.AssetManager.GetEbxEntry(pointer.External.FileGuid);
                    if (textEntry == null)
                        continue;

                    // read localized text asset
                    dynamic localizedText = App.AssetManager.GetEbx(textEntry).RootObject;

                    string lang = localizedText.Language.ToString();
                    lang = lang.Replace("LanguageFormat_", "");

                    if (!languages.Contains(lang))
                        languages.Add(lang);
                }
            }

            if (languages.Count == 0)
                languages.Add("English");

            return languages;
        }

        public IEnumerable<uint> EnumerateStrings()
        {
            foreach (uint key in strings.Keys)
                yield return key;
        }

        public IEnumerable<uint> EnumerateModifiedStrings()
        {
            throw new NotImplementedException();
        }

        public string GetString(uint id)
        {
            if (!strings.ContainsKey(id))
            {
                if (id == 0)
                    return "";
                return string.Format("Invalid StringId: {0}", id.ToString("X8"));
            }
            return strings[id];
        }

        public string GetString(string stringId)
        {
            throw new NotImplementedException();
        }

        public void SetString(uint id, string value)
        {
            throw new NotImplementedException();
        }

        public void SetString(string id, string value)
        {
            throw new NotImplementedException();
        }

        public void RevertString(uint id)
        {
            throw new NotImplementedException();
        }

        public bool isStringEdited(uint id)
        {
            return false;
        }

        public void AddStringWindow()
        {
            throw new NotImplementedException();
        }

        public void BulkReplaceWindow()
        {
            throw new NotImplementedException();
        }

        private void LoadLocalizedStringConfiguration(string type)
        {
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(type))
            {
                // read localization config
                dynamic localizationAsset = App.AssetManager.GetEbx(entry).RootObject;

                // iterate thru language to bundle lists
                foreach (dynamic languageBundleList in localizationAsset.LanguagesToBundlesList)
                {
                    if (languageBundleList.Language.ToString().Equals("LanguageFormat_English"))
                    {
                        foreach (string bundlePath in languageBundleList.BundlePaths)
                        {
                            string bundleFullPath = "win32/" + bundlePath.ToLower();
                            foreach (ResAssetEntry resEntry in App.AssetManager.EnumerateRes(resType: (uint)ResourceType.LocalizedStringResource))
                            {
                                bool bFound = false;
                                foreach (int bindex in resEntry.EnumerateBundles())
                                {
                                    BundleEntry be = App.AssetManager.GetBundleEntry(bindex);
                                    if (be.Name.StartsWith(bundleFullPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        bFound = true;
                                        break;
                                    }
                                }

                                if (bFound)
                                {
                                    LocalizedStringResource resource = App.AssetManager.GetResAs<LocalizedStringResource>(resEntry);
                                    if (resource != null)
                                    {
                                        foreach (KeyValuePair<uint, string> kvp in resource.Strings)
                                        {
                                            if (!strings.ContainsKey(kvp.Key))
                                            {
                                                strings.Add(kvp.Key, kvp.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
