using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerCameraEntityData))]
	public class PlayerCameraEntity : CameraEntityBase, IEntityData<FrostySdk.Ebx.PlayerCameraEntityData>
	{
		public new FrostySdk.Ebx.PlayerCameraEntityData Data => data as FrostySdk.Ebx.PlayerCameraEntityData;

		public PlayerCameraEntity(FrostySdk.Ebx.PlayerCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

