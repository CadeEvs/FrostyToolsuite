using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineInventoryEntityData))]
	public class OnlineInventoryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineInventoryEntityData>
	{
		public new FrostySdk.Ebx.OnlineInventoryEntityData Data => data as FrostySdk.Ebx.OnlineInventoryEntityData;
		public override string DisplayName => "OnlineInventory";

		public OnlineInventoryEntity(FrostySdk.Ebx.OnlineInventoryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

