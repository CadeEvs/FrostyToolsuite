using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientInventoryStatusEntityData))]
	public class ClientInventoryStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientInventoryStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientInventoryStatusEntityData Data => data as FrostySdk.Ebx.ClientInventoryStatusEntityData;
		public override string DisplayName => "ClientInventoryStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientInventoryStatusEntity(FrostySdk.Ebx.ClientInventoryStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

