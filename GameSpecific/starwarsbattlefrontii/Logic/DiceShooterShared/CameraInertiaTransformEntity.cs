using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraInertiaTransformEntityData))]
	public class CameraInertiaTransformEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.CameraInertiaTransformEntityData>
	{
		public new FrostySdk.Ebx.CameraInertiaTransformEntityData Data => data as FrostySdk.Ebx.CameraInertiaTransformEntityData;
		public override string DisplayName => "CameraInertiaTransform";

		public CameraInertiaTransformEntity(FrostySdk.Ebx.CameraInertiaTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

