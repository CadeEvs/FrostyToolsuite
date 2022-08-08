using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterCameraTransformerEntityData))]
	public class StarfighterCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.StarfighterCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.StarfighterCameraTransformerEntityData Data => data as FrostySdk.Ebx.StarfighterCameraTransformerEntityData;
		public override string DisplayName => "StarfighterCameraTransformer";

		public StarfighterCameraTransformerEntity(FrostySdk.Ebx.StarfighterCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

