using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JointTransformQueryEntityData))]
	public class JointTransformQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JointTransformQueryEntityData>
	{
		public new FrostySdk.Ebx.JointTransformQueryEntityData Data => data as FrostySdk.Ebx.JointTransformQueryEntityData;
		public override string DisplayName => "JointTransformQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public JointTransformQueryEntity(FrostySdk.Ebx.JointTransformQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

