using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetCameraEntityData))]
	public class TargetCameraEntity : CameraEntityBase, IEntityData<FrostySdk.Ebx.TargetCameraEntityData>
	{
		public new FrostySdk.Ebx.TargetCameraEntityData Data => data as FrostySdk.Ebx.TargetCameraEntityData;

		public TargetCameraEntity(FrostySdk.Ebx.TargetCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

