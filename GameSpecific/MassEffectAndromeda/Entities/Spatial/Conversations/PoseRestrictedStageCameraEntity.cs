using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PoseRestrictedStageCameraEntityData))]
	public class PoseRestrictedStageCameraEntity : StageCameraEntity, IEntityData<FrostySdk.Ebx.PoseRestrictedStageCameraEntityData>
	{
		public new FrostySdk.Ebx.PoseRestrictedStageCameraEntityData Data => data as FrostySdk.Ebx.PoseRestrictedStageCameraEntityData;

		public PoseRestrictedStageCameraEntity(FrostySdk.Ebx.PoseRestrictedStageCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

