using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGameGroupStatusEntityData))]
	public class ClientGameGroupStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGameGroupStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientGameGroupStatusEntityData Data => data as FrostySdk.Ebx.ClientGameGroupStatusEntityData;
		public override string DisplayName => "ClientGameGroupStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGameGroupStatusEntity(FrostySdk.Ebx.ClientGameGroupStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

