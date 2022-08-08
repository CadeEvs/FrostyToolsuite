using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCharacterInventoryStatusEntityData))]
	public class ClientCharacterInventoryStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientCharacterInventoryStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientCharacterInventoryStatusEntityData Data => data as FrostySdk.Ebx.ClientCharacterInventoryStatusEntityData;
		public override string DisplayName => "ClientCharacterInventoryStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientCharacterInventoryStatusEntity(FrostySdk.Ebx.ClientCharacterInventoryStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

