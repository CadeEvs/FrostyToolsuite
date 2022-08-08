using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCardCraftingEntityData))]
	public class ClientCardCraftingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientCardCraftingEntityData>
	{
		public new FrostySdk.Ebx.ClientCardCraftingEntityData Data => data as FrostySdk.Ebx.ClientCardCraftingEntityData;
		public override string DisplayName => "ClientCardCrafting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientCardCraftingEntity(FrostySdk.Ebx.ClientCardCraftingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

