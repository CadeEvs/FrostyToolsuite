using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientFirstPartyStoreEntityData))]
	public class ClientFirstPartyStoreEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientFirstPartyStoreEntityData>
	{
		public new FrostySdk.Ebx.ClientFirstPartyStoreEntityData Data => data as FrostySdk.Ebx.ClientFirstPartyStoreEntityData;
		public override string DisplayName => "ClientFirstPartyStore";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientFirstPartyStoreEntity(FrostySdk.Ebx.ClientFirstPartyStoreEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

