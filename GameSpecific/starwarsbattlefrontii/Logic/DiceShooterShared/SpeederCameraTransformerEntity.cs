using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpeederCameraTransformerEntityData))]
	public class SpeederCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.SpeederCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.SpeederCameraTransformerEntityData Data => data as FrostySdk.Ebx.SpeederCameraTransformerEntityData;
		public override string DisplayName => "SpeederCameraTransformer";

		public SpeederCameraTransformerEntity(FrostySdk.Ebx.SpeederCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

