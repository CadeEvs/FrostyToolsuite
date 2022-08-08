using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialTargetEntityData))]
	public class SpatialTargetEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SpatialTargetEntityData>
	{
		public new FrostySdk.Ebx.SpatialTargetEntityData Data => data as FrostySdk.Ebx.SpatialTargetEntityData;

		public SpatialTargetEntity(FrostySdk.Ebx.SpatialTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

