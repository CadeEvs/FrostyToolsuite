using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JointOpsEntityData))]
	public class JointOpsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JointOpsEntityData>
	{
		public new FrostySdk.Ebx.JointOpsEntityData Data => data as FrostySdk.Ebx.JointOpsEntityData;
		public override string DisplayName => "JointOps";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public JointOpsEntity(FrostySdk.Ebx.JointOpsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

