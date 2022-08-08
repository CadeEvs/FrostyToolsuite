using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlendControllerData))]
	public class BlendController : CinebotController, IEntityData<FrostySdk.Ebx.BlendControllerData>
	{
		public new FrostySdk.Ebx.BlendControllerData Data => data as FrostySdk.Ebx.BlendControllerData;
		public override string DisplayName => "BlendController";

		public BlendController(FrostySdk.Ebx.BlendControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

