using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicRelativePosDumperControllerData))]
	public class CinematicRelativePosDumperController : CinebotController, IEntityData<FrostySdk.Ebx.CinematicRelativePosDumperControllerData>
	{
		public new FrostySdk.Ebx.CinematicRelativePosDumperControllerData Data => data as FrostySdk.Ebx.CinematicRelativePosDumperControllerData;
		public override string DisplayName => "CinematicRelativePosDumperController";

		public CinematicRelativePosDumperController(FrostySdk.Ebx.CinematicRelativePosDumperControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

