using System.Collections.Generic;
using System.IO;
using Frosty.Core;
using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.Managers;

namespace AnimationEditorPlugin.Managers
{
    public class AssetBankFileManager : ICustomAssetManager
    {
        private Dictionary<int, AssetBankFileEntry> m_entries = new Dictionary<int, AssetBankFileEntry>();
        
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

                Stream resStream = App.AssetManager.GetRes(resEntry);
                
                // @TODO: read asset bank

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
    }
}