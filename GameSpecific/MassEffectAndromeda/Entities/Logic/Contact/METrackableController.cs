using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.METrackableControllerData))]
	public class METrackableController : TrackableController, IEntityData<FrostySdk.Ebx.METrackableControllerData>
	{
		public new FrostySdk.Ebx.METrackableControllerData Data => data as FrostySdk.Ebx.METrackableControllerData;
		public override string DisplayName => "METrackableController";

		public METrackableController(FrostySdk.Ebx.METrackableControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

