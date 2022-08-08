using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualControllerData))]
	public class VisualController : ExtendedController, IEntityData<FrostySdk.Ebx.VisualControllerData>
	{
		public new FrostySdk.Ebx.VisualControllerData Data => data as FrostySdk.Ebx.VisualControllerData;
		public override string DisplayName => "VisualController";

		public VisualController(FrostySdk.Ebx.VisualControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

