using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformSnapToGroundEntityData))]
	public class TransformSnapToGroundEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformSnapToGroundEntityData>
	{
		public new FrostySdk.Ebx.TransformSnapToGroundEntityData Data => data as FrostySdk.Ebx.TransformSnapToGroundEntityData;
		public override string DisplayName => "TransformSnapToGround";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformSnapToGroundEntity(FrostySdk.Ebx.TransformSnapToGroundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

