using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformLookAtEntityData))]
	public class TransformLookAtEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformLookAtEntityData>
	{
		public new FrostySdk.Ebx.TransformLookAtEntityData Data => data as FrostySdk.Ebx.TransformLookAtEntityData;
		public override string DisplayName => "TransformLookAt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformLookAtEntity(FrostySdk.Ebx.TransformLookAtEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

