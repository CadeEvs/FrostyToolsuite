using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LootContainerEntityData))]
	public class LootContainerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LootContainerEntityData>
	{
		public new FrostySdk.Ebx.LootContainerEntityData Data => data as FrostySdk.Ebx.LootContainerEntityData;
        public override IEnumerable<string> HeaderRows
        {
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (loot != null)
                {
					outHeaderRows.Add($"Loot: {Path.GetFileName(loot.Name)}");
                }
				return outHeaderRows;
			}
        }

        private Assets.LootDropObject loot;

		public LootContainerEntity(FrostySdk.Ebx.LootContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			loot = LoadedAssetManager.Instance.LoadAsset<Assets.LootDropObject>(this, Data.Loot);
		}

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(loot);
        }
    }
}

