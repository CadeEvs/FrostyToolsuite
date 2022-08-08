using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientLootBoxStatusEntityData))]
	public class ClientLootBoxStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientLootBoxStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientLootBoxStatusEntityData Data => data as FrostySdk.Ebx.ClientLootBoxStatusEntityData;
		public override string DisplayName => "ClientLootBoxStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientLootBoxStatusEntity(FrostySdk.Ebx.ClientLootBoxStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

