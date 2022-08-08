using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCreateGroupEntityData))]
	public class ClientCreateGroupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientCreateGroupEntityData>
	{
		public new FrostySdk.Ebx.ClientCreateGroupEntityData Data => data as FrostySdk.Ebx.ClientCreateGroupEntityData;
		public override string DisplayName => "ClientCreateGroup";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientCreateGroupEntity(FrostySdk.Ebx.ClientCreateGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

