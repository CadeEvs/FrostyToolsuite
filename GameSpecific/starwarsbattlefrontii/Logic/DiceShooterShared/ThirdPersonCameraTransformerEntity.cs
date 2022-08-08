using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ThirdPersonCameraTransformerEntityData))]
	public class ThirdPersonCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.ThirdPersonCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.ThirdPersonCameraTransformerEntityData Data => data as FrostySdk.Ebx.ThirdPersonCameraTransformerEntityData;
		public override string DisplayName => "ThirdPersonCameraTransformer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ThirdPersonCameraTransformerEntity(FrostySdk.Ebx.ThirdPersonCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

