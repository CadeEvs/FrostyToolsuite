using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientConnectionStatusEntityData))]
	public class ClientConnectionStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientConnectionStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientConnectionStatusEntityData Data => data as FrostySdk.Ebx.ClientConnectionStatusEntityData;
		public override string DisplayName => "ClientConnectionStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientConnectionStatusEntity(FrostySdk.Ebx.ClientConnectionStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

