using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventControllerData))]
	public class EventController : CinebotController, IEntityData<FrostySdk.Ebx.EventControllerData>
	{
		public new FrostySdk.Ebx.EventControllerData Data => data as FrostySdk.Ebx.EventControllerData;
		public override string DisplayName => "EventController";

		public EventController(FrostySdk.Ebx.EventControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

