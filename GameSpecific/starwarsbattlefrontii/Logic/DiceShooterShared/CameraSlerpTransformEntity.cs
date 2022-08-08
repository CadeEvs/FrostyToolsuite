using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraSlerpTransformEntityData))]
	public class CameraSlerpTransformEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.CameraSlerpTransformEntityData>
	{
		public new FrostySdk.Ebx.CameraSlerpTransformEntityData Data => data as FrostySdk.Ebx.CameraSlerpTransformEntityData;
		public override string DisplayName => "CameraSlerpTransform";

		public CameraSlerpTransformEntity(FrostySdk.Ebx.CameraSlerpTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

