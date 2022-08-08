using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SurfaceNoiseControllerData))]
	public class SurfaceNoiseController : NoiseController, IEntityData<FrostySdk.Ebx.SurfaceNoiseControllerData>
	{
		public new FrostySdk.Ebx.SurfaceNoiseControllerData Data => data as FrostySdk.Ebx.SurfaceNoiseControllerData;
		public override string DisplayName => "SurfaceNoiseController";

		public SurfaceNoiseController(FrostySdk.Ebx.SurfaceNoiseControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

