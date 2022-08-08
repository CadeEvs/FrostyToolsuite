using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReactionControllerData))]
	public class ReactionController : CinebotController, IEntityData<FrostySdk.Ebx.ReactionControllerData>
	{
		public new FrostySdk.Ebx.ReactionControllerData Data => data as FrostySdk.Ebx.ReactionControllerData;
		public override string DisplayName => "ReactionController";

		public ReactionController(FrostySdk.Ebx.ReactionControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

