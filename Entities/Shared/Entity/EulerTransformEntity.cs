using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EulerTransformEntityData))]
	public class EulerTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EulerTransformEntityData>
	{
		public new FrostySdk.Ebx.EulerTransformEntityData Data => data as FrostySdk.Ebx.EulerTransformEntityData;
		public override string DisplayName => "EulerTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EulerTransformEntity(FrostySdk.Ebx.EulerTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

