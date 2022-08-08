using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECraftingDataProviderData))]
	public class MECraftingDataProvider : MeshSpawner, IEntityData<FrostySdk.Ebx.MECraftingDataProviderData>
	{
		public new FrostySdk.Ebx.MECraftingDataProviderData Data => data as FrostySdk.Ebx.MECraftingDataProviderData;
		public override string DisplayName => "MECraftingDataProvider";

		public MECraftingDataProvider(FrostySdk.Ebx.MECraftingDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

