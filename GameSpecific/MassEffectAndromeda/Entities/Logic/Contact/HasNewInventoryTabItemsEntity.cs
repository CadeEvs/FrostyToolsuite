using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HasNewInventoryTabItemsEntityData))]
	public class HasNewInventoryTabItemsEntity : HasNewInventoryWatcher, IEntityData<FrostySdk.Ebx.HasNewInventoryTabItemsEntityData>
	{
		public new FrostySdk.Ebx.HasNewInventoryTabItemsEntityData Data => data as FrostySdk.Ebx.HasNewInventoryTabItemsEntityData;
		public override string DisplayName => "HasNewInventoryTabItems";

		public HasNewInventoryTabItemsEntity(FrostySdk.Ebx.HasNewInventoryTabItemsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

