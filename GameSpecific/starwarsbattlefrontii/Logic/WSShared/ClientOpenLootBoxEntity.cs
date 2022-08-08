using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientOpenLootBoxEntityData))]
	public class ClientOpenLootBoxEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientOpenLootBoxEntityData>
	{
		public new FrostySdk.Ebx.ClientOpenLootBoxEntityData Data => data as FrostySdk.Ebx.ClientOpenLootBoxEntityData;
		public override string DisplayName => "ClientOpenLootBox";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientOpenLootBoxEntity(FrostySdk.Ebx.ClientOpenLootBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

