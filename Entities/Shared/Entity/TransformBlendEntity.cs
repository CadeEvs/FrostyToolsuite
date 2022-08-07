using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformBlendEntityData))]
	public class TransformBlendEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformBlendEntityData>
	{
		public new FrostySdk.Ebx.TransformBlendEntityData Data => data as FrostySdk.Ebx.TransformBlendEntityData;
		public override string DisplayName => "TransformBlend";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformBlendEntity(FrostySdk.Ebx.TransformBlendEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

