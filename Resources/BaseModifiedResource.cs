using Frosty.Core;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Managers.Entries;

namespace LevelEditorPlugin.Resources
{
    // @todo: Need to clear modification count on saving of project

    public class BaseModifiedResource : ModifiedResource
    {
        private int modificationCount;
        private ResAssetEntry resEntry;

        public BaseModifiedResource(ResAssetEntry inEntry)
        {
            resEntry = inEntry;
            modificationCount = 0;
        }

        public void UpdateData(Resource resource)
        {
            App.AssetManager.ModifyRes(resEntry.ResRid, resource);
            modificationCount++;
        }

        public void UndoData(Resource resource)
        {
            modificationCount--;
            if (modificationCount > 0)
            {
                App.AssetManager.ModifyRes(resEntry.ResRid, resource);
            }
            else
            {
                resEntry.ClearModifications();
            }
        }
    }
}
