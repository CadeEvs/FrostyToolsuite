using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestPointEntityData))]
	public class TestPointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.TestPointEntityData>
	{
		public new FrostySdk.Ebx.TestPointEntityData Data => data as FrostySdk.Ebx.TestPointEntityData;

		public TestPointEntity(FrostySdk.Ebx.TestPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

