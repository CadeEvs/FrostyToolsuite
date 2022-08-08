using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineInventoryConsumeItemEntityData))]
	public class OnlineInventoryConsumeItemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineInventoryConsumeItemEntityData>
	{
		public new FrostySdk.Ebx.OnlineInventoryConsumeItemEntityData Data => data as FrostySdk.Ebx.OnlineInventoryConsumeItemEntityData;
		public override string DisplayName => "OnlineInventoryConsumeItem";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ConsumeItem", Direction.In)
			};
		}

		public OnlineInventoryConsumeItemEntity(FrostySdk.Ebx.OnlineInventoryConsumeItemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

