using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemPickupEntityData))]
	public class MEItemPickupEntity : MEItemActionEntity, IEntityData<FrostySdk.Ebx.MEItemPickupEntityData>
	{
		public new FrostySdk.Ebx.MEItemPickupEntityData Data => data as FrostySdk.Ebx.MEItemPickupEntityData;
		public override string DisplayName => "MEItemPickup";
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

		public MEItemPickupEntity(FrostySdk.Ebx.MEItemPickupEntityData inData, Entity inParent)
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

