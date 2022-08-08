using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InventoryDataProviderData))]
	public class InventoryDataProvider : MeshSpawner, IEntityData<FrostySdk.Ebx.InventoryDataProviderData>
	{
		public new FrostySdk.Ebx.InventoryDataProviderData Data => data as FrostySdk.Ebx.InventoryDataProviderData;
		public override string DisplayName => "InventoryDataProvider";

		public InventoryDataProvider(FrostySdk.Ebx.InventoryDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

