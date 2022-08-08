using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeightAdjustmentControllerData))]
	public class HeightAdjustmentController : CinebotController, IEntityData<FrostySdk.Ebx.HeightAdjustmentControllerData>
	{
		public new FrostySdk.Ebx.HeightAdjustmentControllerData Data => data as FrostySdk.Ebx.HeightAdjustmentControllerData;
		public override string DisplayName => "HeightAdjustmentController";

		public HeightAdjustmentController(FrostySdk.Ebx.HeightAdjustmentControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

