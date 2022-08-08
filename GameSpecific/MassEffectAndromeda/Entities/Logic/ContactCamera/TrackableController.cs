using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackableControllerData))]
	public class TrackableController : VisualController, IEntityData<FrostySdk.Ebx.TrackableControllerData>
	{
		public new FrostySdk.Ebx.TrackableControllerData Data => data as FrostySdk.Ebx.TrackableControllerData;
		public override string DisplayName => "TrackableController";

		public TrackableController(FrostySdk.Ebx.TrackableControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

