using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NoiseControllerData))]
	public class NoiseController : CinebotController, IEntityData<FrostySdk.Ebx.NoiseControllerData>
	{
		public new FrostySdk.Ebx.NoiseControllerData Data => data as FrostySdk.Ebx.NoiseControllerData;
		public override string DisplayName => "NoiseController";

		public NoiseController(FrostySdk.Ebx.NoiseControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

