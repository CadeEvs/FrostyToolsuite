using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGroupListEntityData))]
	public class ClientGroupListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGroupListEntityData>
	{
		public new FrostySdk.Ebx.ClientGroupListEntityData Data => data as FrostySdk.Ebx.ClientGroupListEntityData;
		public override string DisplayName => "ClientGroupList";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGroupListEntity(FrostySdk.Ebx.ClientGroupListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

