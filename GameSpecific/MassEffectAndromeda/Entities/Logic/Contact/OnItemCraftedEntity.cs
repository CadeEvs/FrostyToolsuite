using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnItemCraftedEntityData))]
	public class OnItemCraftedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnItemCraftedEntityData>
	{
		public new FrostySdk.Ebx.OnItemCraftedEntityData Data => data as FrostySdk.Ebx.OnItemCraftedEntityData;
		public override string DisplayName => "OnItemCrafted";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OnItemCraftedEntity(FrostySdk.Ebx.OnItemCraftedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

