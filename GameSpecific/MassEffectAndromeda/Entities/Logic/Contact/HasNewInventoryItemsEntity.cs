using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HasNewInventoryItemsEntityData))]
	public class HasNewInventoryItemsEntity : HasNewInventoryWatcher, IEntityData<FrostySdk.Ebx.HasNewInventoryItemsEntityData>
	{
		public new FrostySdk.Ebx.HasNewInventoryItemsEntityData Data => data as FrostySdk.Ebx.HasNewInventoryItemsEntityData;
		public override string DisplayName => "HasNewInventoryItems";

		public HasNewInventoryItemsEntity(FrostySdk.Ebx.HasNewInventoryItemsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

