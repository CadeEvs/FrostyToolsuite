using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicHeightAdjustmentControllerData))]
	public class CinematicHeightAdjustmentController : CinebotController, IEntityData<FrostySdk.Ebx.CinematicHeightAdjustmentControllerData>
	{
		public new FrostySdk.Ebx.CinematicHeightAdjustmentControllerData Data => data as FrostySdk.Ebx.CinematicHeightAdjustmentControllerData;
		public override string DisplayName => "CinematicHeightAdjustmentController";

		public CinematicHeightAdjustmentController(FrostySdk.Ebx.CinematicHeightAdjustmentControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

