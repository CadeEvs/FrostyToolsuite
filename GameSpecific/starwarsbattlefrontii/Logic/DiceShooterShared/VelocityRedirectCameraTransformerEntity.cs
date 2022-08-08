using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VelocityRedirectCameraTransformerEntityData))]
	public class VelocityRedirectCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.VelocityRedirectCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.VelocityRedirectCameraTransformerEntityData Data => data as FrostySdk.Ebx.VelocityRedirectCameraTransformerEntityData;
		public override string DisplayName => "VelocityRedirectCameraTransformer";

		public VelocityRedirectCameraTransformerEntity(FrostySdk.Ebx.VelocityRedirectCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

