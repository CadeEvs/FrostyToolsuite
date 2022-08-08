using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LookAtControllerData))]
	public class LookAtController : LogicController, IEntityData<FrostySdk.Ebx.LookAtControllerData>
	{
		public new FrostySdk.Ebx.LookAtControllerData Data => data as FrostySdk.Ebx.LookAtControllerData;
		public override string DisplayName => "LookAtController";

		public LookAtController(FrostySdk.Ebx.LookAtControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

