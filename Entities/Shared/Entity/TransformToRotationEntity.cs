using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformToRotationEntityData))]
	public class TransformToRotationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformToRotationEntityData>
	{
		public new FrostySdk.Ebx.TransformToRotationEntityData Data => data as FrostySdk.Ebx.TransformToRotationEntityData;
		public override string DisplayName => "TransformToRotation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformToRotationEntity(FrostySdk.Ebx.TransformToRotationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

