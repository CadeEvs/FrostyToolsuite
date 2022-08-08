using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LookAtCameraEntityData))]
	public class LookAtCameraEntity : CameraEntity, IEntityData<FrostySdk.Ebx.LookAtCameraEntityData>
	{
		public new FrostySdk.Ebx.LookAtCameraEntityData Data => data as FrostySdk.Ebx.LookAtCameraEntityData;

		public LookAtCameraEntity(FrostySdk.Ebx.LookAtCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

