using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicOverTheShoulderControllerData))]
	public class CinematicOverTheShoulderController : CinematicHeightAdjustmentController, IEntityData<FrostySdk.Ebx.CinematicOverTheShoulderControllerData>
	{
		public new FrostySdk.Ebx.CinematicOverTheShoulderControllerData Data => data as FrostySdk.Ebx.CinematicOverTheShoulderControllerData;
		public override string DisplayName => "CinematicOverTheShoulderController";

		public CinematicOverTheShoulderController(FrostySdk.Ebx.CinematicOverTheShoulderControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

