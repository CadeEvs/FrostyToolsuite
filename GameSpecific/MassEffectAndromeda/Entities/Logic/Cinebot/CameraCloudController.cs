using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraCloudControllerData))]
	public class CameraCloudController : CinebotController, IEntityData<FrostySdk.Ebx.CameraCloudControllerData>
	{
		public new FrostySdk.Ebx.CameraCloudControllerData Data => data as FrostySdk.Ebx.CameraCloudControllerData;
		public override string DisplayName => "CameraCloudController";

		public CameraCloudController(FrostySdk.Ebx.CameraCloudControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

