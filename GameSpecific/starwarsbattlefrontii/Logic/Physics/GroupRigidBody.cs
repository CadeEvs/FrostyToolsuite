using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroupRigidBodyData))]
	public class GroupRigidBody : RigidBody, IEntityData<FrostySdk.Ebx.GroupRigidBodyData>
	{
		public new FrostySdk.Ebx.GroupRigidBodyData Data => data as FrostySdk.Ebx.GroupRigidBodyData;
		public override string DisplayName => "GroupRigidBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GroupRigidBody(FrostySdk.Ebx.GroupRigidBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

