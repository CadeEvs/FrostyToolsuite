using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ArmCollisionCameraTransformerEntityData))]
	public class ArmCollisionCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.ArmCollisionCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.ArmCollisionCameraTransformerEntityData Data => data as FrostySdk.Ebx.ArmCollisionCameraTransformerEntityData;
		public override string DisplayName => "ArmCollisionCameraTransformer";

		public ArmCollisionCameraTransformerEntity(FrostySdk.Ebx.ArmCollisionCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

