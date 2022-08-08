using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LootControllerEntityData))]
	public class LootControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LootControllerEntityData>
	{
		public new FrostySdk.Ebx.LootControllerEntityData Data => data as FrostySdk.Ebx.LootControllerEntityData;
		public override string DisplayName => "LootController";

		public LootControllerEntity(FrostySdk.Ebx.LootControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

