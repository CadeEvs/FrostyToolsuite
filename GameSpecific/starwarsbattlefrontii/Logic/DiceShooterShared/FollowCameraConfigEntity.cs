using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FollowCameraConfigEntityData))]
	public class FollowCameraConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FollowCameraConfigEntityData>
	{
		public new FrostySdk.Ebx.FollowCameraConfigEntityData Data => data as FrostySdk.Ebx.FollowCameraConfigEntityData;
		public override string DisplayName => "FollowCameraConfig";

		public FollowCameraConfigEntity(FrostySdk.Ebx.FollowCameraConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

