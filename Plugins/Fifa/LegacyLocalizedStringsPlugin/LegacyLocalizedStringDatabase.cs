using Frosty.Core;
using Frosty.Core.Legacy;
using Frosty.Core.Windows;
using LegacyDatabasePlugin.Database;
using LegacyDatabasePlugin.IO;
using System;
using System.Collections.Generic;

namespace LegacyLocalizedStringsPlugin
{
    public class LegacyLocalizedStringDatabase : ILocalizedStringDatabase
    {
        private Dictionary<uint, string> strings = new Dictionary<uint, string>();

        public void Initialize()
        {
            Dictionary<string, string> languages = new Dictionary<string, string>()
            {
                { "English", "eng_us" },
                { "French", "fre_fr" },
                { "Italian", "ita_it" },
                { "German", "ger_de" },
                { "Spanish", "spa_es" },
                { "Portuguese", "por_pt" },
                { "Japanese", "jpn_jp" },
                { "Czech", "cze_cz" },
                { "TraditionalChinese", "chi_hk" },
                { "Turkish", "tur_tr" },
                { "Polish", "pol_pl" },
                { "Russian", "rus_ru" },
                { "Norwegian", "nor_no" },
                { "Dutch", "dut_nl" },
                { "BrazilianPortuguese", "por_bz" },
                { "Swedish", "swe_se" },
                { "SpanishMex", "spa_mx" },
                { "Danish", "dan_dk" },
                { "ArabicSA", "ara_sa" },
                { "SimplifiedChinese", "chs_cn" },
            };
            //string langName = languages[Config.Get("Init", "Language", "English")];
            string langName = languages[Config.Get("Language", "English", ConfigScope.Game)];
            LegacyFileEntry metaEntry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", "data/loc/" + langName + "-meta.xml");
            LegacyFileEntry dbEntry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", "data/loc/" + langName + ".db");

            if (dbEntry != null)
            {
                dbEntry.AssetModified -= UpdateDatabase;
                dbEntry.AssetModified += UpdateDatabase;

                using (LegacyDbReader reader = new LegacyDbReader(App.AssetManager.GetCustomAsset("legacy", metaEntry), App.AssetManager.GetCustomAsset("legacy", dbEntry)))
                {
                    LegacyDb db = reader.ReadDb();
                    foreach (LegacyDbRow row in db["LanguageStrings"].Rows)
                    {
                        int hash = (int)row["hashid"];
                        if (strings.ContainsKey((uint)hash))
                            continue;
                        strings.Add((uint)hash, (string)row["sourcetext"]);
                    }
                }
            }
        }

        public List<string> GetLanguages()
        {
            Dictionary<string, string> languages = new Dictionary<string, string>()
            {
                { "eng_us", "English" },
                { "fre_fr", "French" },
                { "ita_it", "Italian" },
                { "ger_de", "German" },
                { "spa_es", "Spanish" },
                { "por_pt", "Portuguese" },
                { "jpn_jp", "Japanese" },
                { "cze_cz", "Czech" },
                { "chi_hk", "TraditionalChinese" },
                { "tur_tr", "Turkish" },
                { "pol_pl", "Polish" },
                { "rus_ru", "Russian" },
                { "nor_no", "Norwegian" },
                { "dut_nl", "Dutch" },
                { "por_bz", "BrazilianPortuguese" },
                { "swe_se", "Swedish" },
                { "spa_mx", "SpanishMex" },
                { "dan_dk", "Danish" },
                { "ara_sa", "ArabicSA" },
                { "chs_cn", "SimplifiedChinese" },
            };

            List<string> retValues = new List<string>();
            
            foreach (var entry in App.AssetManager.EnumerateCustomAssets("legacy"))
            {
                if (entry.Name.StartsWith("data/loc/"))
                {
                    string language = languages[entry.Name.Substring(entry.Name.LastIndexOf("/") + 1, 6)];
                    if (!retValues.Contains(language))
                        retValues.Add(language);
                }
            }

            if (retValues.Count == 0)
                retValues.Add("English");
            
            return retValues;
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

        private void UpdateDatabase(object sender, EventArgs e)
        {
            strings.Clear();
            FrostyTaskWindow.Show("Updating localized strings", "", (task) => { Initialize(); });
        }
    }
}
