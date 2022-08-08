using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VelocityFromTransformEntityData))]
	public class VelocityFromTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VelocityFromTransformEntityData>
	{
		public new FrostySdk.Ebx.VelocityFromTransformEntityData Data => data as FrostySdk.Ebx.VelocityFromTransformEntityData;
		public override string DisplayName => "VelocityFromTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VelocityFromTransformEntity(FrostySdk.Ebx.VelocityFromTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

