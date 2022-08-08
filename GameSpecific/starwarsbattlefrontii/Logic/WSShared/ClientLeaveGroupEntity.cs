using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientLeaveGroupEntityData))]
	public class ClientLeaveGroupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientLeaveGroupEntityData>
	{
		public new FrostySdk.Ebx.ClientLeaveGroupEntityData Data => data as FrostySdk.Ebx.ClientLeaveGroupEntityData;
		public override string DisplayName => "ClientLeaveGroup";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientLeaveGroupEntity(FrostySdk.Ebx.ClientLeaveGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

