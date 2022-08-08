using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OverrideJointEntityData))]
	public class OverrideJointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OverrideJointEntityData>
	{
		public new FrostySdk.Ebx.OverrideJointEntityData Data => data as FrostySdk.Ebx.OverrideJointEntityData;
		public override string DisplayName => "OverrideJoint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OverrideJointEntity(FrostySdk.Ebx.OverrideJointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

