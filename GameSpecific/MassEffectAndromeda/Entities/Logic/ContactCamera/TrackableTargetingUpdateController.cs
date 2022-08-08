using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackableTargetingUpdateControllerData))]
	public class TrackableTargetingUpdateController : LogicController, IEntityData<FrostySdk.Ebx.TrackableTargetingUpdateControllerData>
	{
		public new FrostySdk.Ebx.TrackableTargetingUpdateControllerData Data => data as FrostySdk.Ebx.TrackableTargetingUpdateControllerData;
		public override string DisplayName => "TrackableTargetingUpdateController";

		public TrackableTargetingUpdateController(FrostySdk.Ebx.TrackableTargetingUpdateControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

