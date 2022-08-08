using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CockpitCameraTransformerEntityData))]
	public class CockpitCameraTransformerEntity : CameraTransformerEntity, IEntityData<FrostySdk.Ebx.CockpitCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.CockpitCameraTransformerEntityData Data => data as FrostySdk.Ebx.CockpitCameraTransformerEntityData;
		public override string DisplayName => "CockpitCameraTransformer";

		public CockpitCameraTransformerEntity(FrostySdk.Ebx.CockpitCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

