using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCharacterCraftingEntityData))]
	public class ClientCharacterCraftingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientCharacterCraftingEntityData>
	{
		public new FrostySdk.Ebx.ClientCharacterCraftingEntityData Data => data as FrostySdk.Ebx.ClientCharacterCraftingEntityData;
		public override string DisplayName => "ClientCharacterCrafting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientCharacterCraftingEntity(FrostySdk.Ebx.ClientCharacterCraftingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

