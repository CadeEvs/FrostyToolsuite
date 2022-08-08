using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FreeControllerData))]
	public class FreeController : VisualController, IEntityData<FrostySdk.Ebx.FreeControllerData>
	{
		public new FrostySdk.Ebx.FreeControllerData Data => data as FrostySdk.Ebx.FreeControllerData;
		public override string DisplayName => "FreeController";

		public FreeController(FrostySdk.Ebx.FreeControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

