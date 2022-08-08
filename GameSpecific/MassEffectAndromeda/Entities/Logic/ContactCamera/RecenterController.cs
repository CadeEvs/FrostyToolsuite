using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecenterControllerData))]
	public class RecenterController : LogicController, IEntityData<FrostySdk.Ebx.RecenterControllerData>
	{
		public new FrostySdk.Ebx.RecenterControllerData Data => data as FrostySdk.Ebx.RecenterControllerData;
		public override string DisplayName => "RecenterController";

		public RecenterController(FrostySdk.Ebx.RecenterControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

