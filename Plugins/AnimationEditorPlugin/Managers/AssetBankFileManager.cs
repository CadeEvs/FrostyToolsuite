using System.Collections.Generic;
using System.IO;
using AnimationEditorPlugin.Formats;
using Frosty.Core;
using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;

namespace AnimationEditorPlugin.Managers
{
    public class AssetBankFileManager : ICustomAssetManager
    {
        private Dictionary<int, AssetBankFileEntry> m_entries = new Dictionary<int, AssetBankFileEntry>();

        #region -- ICustomAssetManager --

        public void Initialize(ILogger logger)
        {
            logger.Log("Loading asset banks");
            
            uint totalCount = App.AssetManager.GetResCount((uint)ResourceType.AssetBank);
            uint index = 0;
            foreach (ResAssetEntry resEntry in App.AssetManager.EnumerateRes((uint)ResourceType.AssetBank))
            {
                uint progress = (uint)((index / (float)totalCount) * 100);
                logger.Log("progress:" + progress);

                if (!resEntry.Name.Contains("arctic_01_win32_antstate")) 
                    continue;
                //if (!resEntry.Name.Contains("level_rush_suburbia_win32_antstate")) 
                //    continue;

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
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}