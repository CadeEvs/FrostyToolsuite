using System.Collections.Generic;
using System.IO;
using AnimationEditorPlugin.Formats;
using AnimationEditorPlugin.Formats.Sections;
using Frosty.Core;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace AnimationEditorPlugin.Managers
{
    public class AssetBankFileManager : ICustomAssetManager
    {
        private Dictionary<int, AssetBankFileEntry> m_entries = new Dictionary<int, AssetBankFileEntry>();

        #region -- ICustomAssetManager --

        public bool ShouldInitializeOnStartup => false;

        public void Initialize(ILogger logger)
        {
            logger.Log("Loading asset banks");
            
            uint totalCount = App.AssetManager.GetResCount((uint)ResourceType.AssetBank);
            uint index = 0;
            foreach (ResAssetEntry resEntry in App.AssetManager.EnumerateRes((uint)ResourceType.AssetBank))
            {
                uint progress = (uint)((index / (float)totalCount) * 100);
                logger.Log("progress:" + progress);

                // temporarily get specific res from testing games
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
                {
                    if (!resEntry.Name.Contains("deathstar02_01_win32_antstate"))
                    {
                        continue;
                    }
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefront))
                {
                    if (!resEntry.Name.Contains("arctic_01_win32_antstate"))
                    {
                        continue;
                    }
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare2))
                {
                    if (!resEntry.Name.Contains("level_rush_suburbia_win32_antstate"))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                
                using (NativeReader reader = new NativeReader(App.AssetManager.GetRes(resEntry)))
                {
                    uint version = reader.ReadUInt(Endian.Big);
                    if (version == 3)
                    {
                        uint size = reader.ReadUInt(Endian.Big);
                        
                        // actual start of asset bank data (potentially?)
                        reader.Position = size + 4;
                        
                        SectionHeader header = new SectionHeader();
                        header.Read(reader);
                        
                        // STRM
                        if (header.Format == SectionFormat.STRM)
                        {
                            Section_STRM strm = new Section_STRM(header);
                            strm.Read(reader);
                            
                            List<Bank> banks;
                            
                            // REFL / REF2
                            header.Read(reader);
                            if (header.Format == SectionFormat.REFL)
                            {
                                Section_REFL refl = new Section_REFL(header);
                                refl.Read(reader);

                                banks = refl.Banks;
                            }
                            else if (header.Format == SectionFormat.REF2)
                            {
                                Section_REF2 ref2 = new Section_REF2(header);
                                ref2.Read(reader);
                                
                                banks = ref2.Banks;
                            }
                            else
                            {
                                banks = new List<Bank>();
                            }

                            foreach (Bank bank in banks)
                            {
                                int hash = Fnv1.HashString(bank.Name);
                                
                                m_entries.Add(hash, new AssetBankFileEntry() { Bank = bank});
                            }
                        }
                    }
                }

                index++;
            }
        }

        public AssetEntry GetAssetEntry(string key)
        {
            int hash = Fnv1.HashString(key);
            if (m_entries.ContainsKey(hash))
            {
                return m_entries[hash];
            }
            
            return null;
        }

        public Stream GetAsset(AssetEntry entry)
        {
            throw new System.NotImplementedException();
        }

        public void ModifyAsset(string key, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AssetEntry> EnumerateAssets(bool modifiedOnly)
        {
            foreach (AssetBankFileEntry entry in m_entries.Values)
            {
                if (modifiedOnly && !entry.IsModified)
                {
                    continue;
                }
                
                yield return entry;
            }
        }

        public void OnCommand(string command, params object[] value)
        {
            switch (command)
            {
                case "initialize": Initialize((ILogger)value[0]); break;
            }
        }
        
        #endregion
    }
}