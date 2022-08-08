using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PerformancePreviewControlEntityData))]
	public class PerformancePreviewControlEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PerformancePreviewControlEntityData>
	{
		public new FrostySdk.Ebx.PerformancePreviewControlEntityData Data => data as FrostySdk.Ebx.PerformancePreviewControlEntityData;

		public PerformancePreviewControlEntity(FrostySdk.Ebx.PerformancePreviewControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

