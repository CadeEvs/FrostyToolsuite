using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StageCameraEntityData))]
	public class StageCameraEntity : StageObjectEntity, IEntityData<FrostySdk.Ebx.StageCameraEntityData>
	{
		public new FrostySdk.Ebx.StageCameraEntityData Data => data as FrostySdk.Ebx.StageCameraEntityData;

		public StageCameraEntity(FrostySdk.Ebx.StageCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

