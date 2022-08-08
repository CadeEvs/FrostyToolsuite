using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareTransformEntityData))]
	public class CompareTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CompareTransformEntityData>
	{
		public new FrostySdk.Ebx.CompareTransformEntityData Data => data as FrostySdk.Ebx.CompareTransformEntityData;
		public override string DisplayName => "CompareTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareTransformEntity(FrostySdk.Ebx.CompareTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

