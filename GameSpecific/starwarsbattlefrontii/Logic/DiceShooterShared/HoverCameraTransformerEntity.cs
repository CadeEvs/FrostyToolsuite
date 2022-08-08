using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HoverCameraTransformerEntityData))]
	public class HoverCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.HoverCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.HoverCameraTransformerEntityData Data => data as FrostySdk.Ebx.HoverCameraTransformerEntityData;
		public override string DisplayName => "HoverCameraTransformer";

		public HoverCameraTransformerEntity(FrostySdk.Ebx.HoverCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

