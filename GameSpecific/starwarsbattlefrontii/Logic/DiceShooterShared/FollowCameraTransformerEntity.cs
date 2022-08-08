using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FollowCameraTransformerEntityData))]
	public class FollowCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.FollowCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.FollowCameraTransformerEntityData Data => data as FrostySdk.Ebx.FollowCameraTransformerEntityData;
		public override string DisplayName => "FollowCameraTransformer";

		public FollowCameraTransformerEntity(FrostySdk.Ebx.FollowCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

