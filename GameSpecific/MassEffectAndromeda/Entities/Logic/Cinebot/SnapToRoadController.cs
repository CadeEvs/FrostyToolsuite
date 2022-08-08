using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SnapToRoadControllerData))]
	public class SnapToRoadController : CinebotController, IEntityData<FrostySdk.Ebx.SnapToRoadControllerData>
	{
		public new FrostySdk.Ebx.SnapToRoadControllerData Data => data as FrostySdk.Ebx.SnapToRoadControllerData;
		public override string DisplayName => "SnapToRoadController";

		public SnapToRoadController(FrostySdk.Ebx.SnapToRoadControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

