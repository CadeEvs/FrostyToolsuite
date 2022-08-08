using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientBuyLootBoxEntityData))]
	public class ClientBuyLootBoxEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientBuyLootBoxEntityData>
	{
		public new FrostySdk.Ebx.ClientBuyLootBoxEntityData Data => data as FrostySdk.Ebx.ClientBuyLootBoxEntityData;
		public override string DisplayName => "ClientBuyLootBox";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientBuyLootBoxEntity(FrostySdk.Ebx.ClientBuyLootBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

