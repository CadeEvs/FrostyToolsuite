using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VectorShapeInfoEntityData))]
	public class VectorShapeInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VectorShapeInfoEntityData>
	{
		public new FrostySdk.Ebx.VectorShapeInfoEntityData Data => data as FrostySdk.Ebx.VectorShapeInfoEntityData;
		public override string DisplayName => "VectorShapeInfo";

		public VectorShapeInfoEntity(FrostySdk.Ebx.VectorShapeInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

