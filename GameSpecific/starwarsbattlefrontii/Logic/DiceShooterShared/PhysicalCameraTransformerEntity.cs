using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicalCameraTransformerEntityData))]
	public class PhysicalCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.PhysicalCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.PhysicalCameraTransformerEntityData Data => data as FrostySdk.Ebx.PhysicalCameraTransformerEntityData;
		public override string DisplayName => "PhysicalCameraTransformer";

		public PhysicalCameraTransformerEntity(FrostySdk.Ebx.PhysicalCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

