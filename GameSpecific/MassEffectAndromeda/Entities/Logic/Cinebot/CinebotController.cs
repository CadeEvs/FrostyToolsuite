using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotControllerData))]
	public class CinebotController : CinebotTrackable, IEntityData<FrostySdk.Ebx.CinebotControllerData>
	{
		public new FrostySdk.Ebx.CinebotControllerData Data => data as FrostySdk.Ebx.CinebotControllerData;
		public override string DisplayName => "CinebotController";

		public CinebotController(FrostySdk.Ebx.CinebotControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

