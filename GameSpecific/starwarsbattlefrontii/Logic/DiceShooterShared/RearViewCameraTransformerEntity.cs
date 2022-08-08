using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RearViewCameraTransformerEntityData))]
	public class RearViewCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.RearViewCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.RearViewCameraTransformerEntityData Data => data as FrostySdk.Ebx.RearViewCameraTransformerEntityData;
		public override string DisplayName => "RearViewCameraTransformer";

		public RearViewCameraTransformerEntity(FrostySdk.Ebx.RearViewCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

