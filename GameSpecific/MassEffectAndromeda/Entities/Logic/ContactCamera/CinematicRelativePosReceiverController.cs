using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicRelativePosReceiverControllerData))]
	public class CinematicRelativePosReceiverController : CinebotController, IEntityData<FrostySdk.Ebx.CinematicRelativePosReceiverControllerData>
	{
		public new FrostySdk.Ebx.CinematicRelativePosReceiverControllerData Data => data as FrostySdk.Ebx.CinematicRelativePosReceiverControllerData;
		public override string DisplayName => "CinematicRelativePosReceiverController";

		public CinematicRelativePosReceiverController(FrostySdk.Ebx.CinematicRelativePosReceiverControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

