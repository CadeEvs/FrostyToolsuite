using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RelativePosDumperControllerData))]
	public class RelativePosDumperController : LogicController, IEntityData<FrostySdk.Ebx.RelativePosDumperControllerData>
	{
		public new FrostySdk.Ebx.RelativePosDumperControllerData Data => data as FrostySdk.Ebx.RelativePosDumperControllerData;
		public override string DisplayName => "RelativePosDumperController";

		public RelativePosDumperController(FrostySdk.Ebx.RelativePosDumperControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

