using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPerformanceTestPointEntityData))]
	public class WSPerformanceTestPointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WSPerformanceTestPointEntityData>
	{
		public new FrostySdk.Ebx.WSPerformanceTestPointEntityData Data => data as FrostySdk.Ebx.WSPerformanceTestPointEntityData;

		public WSPerformanceTestPointEntity(FrostySdk.Ebx.WSPerformanceTestPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

