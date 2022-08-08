using Frosty.Core;
using LevelEditorPlugin.Controls;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.ItemData))]
    public class ItemData : Asset, IAssetData<FrostySdk.Ebx.ItemData>
    {
        public FrostySdk.Ebx.ItemData Data => data as FrostySdk.Ebx.ItemData;

        public static Dictionary<uint, string> itemHashDict = new Dictionary<uint, string>();

        public ItemData(Guid fileGuid, FrostySdk.Ebx.ItemData inData)
            : base(fileGuid, inData)
        {
        }

        public static string GetItemNameFromHash(uint hash)
        {
            if (!itemHashDict.ContainsKey(hash))
                return "";
            return itemHashDict[hash];
        }
    }
}
