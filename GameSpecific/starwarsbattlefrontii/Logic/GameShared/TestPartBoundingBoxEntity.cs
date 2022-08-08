using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestPartBoundingBoxEntityData))]
	public class TestPartBoundingBoxEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestPartBoundingBoxEntityData>
	{
		public new FrostySdk.Ebx.TestPartBoundingBoxEntityData Data => data as FrostySdk.Ebx.TestPartBoundingBoxEntityData;
		public override string DisplayName => "TestPartBoundingBox";

		public TestPartBoundingBoxEntity(FrostySdk.Ebx.TestPartBoundingBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

