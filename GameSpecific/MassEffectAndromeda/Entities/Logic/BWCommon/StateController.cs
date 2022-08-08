using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StateControllerData))]
	public class StateController : LogicEntity, IEntityData<FrostySdk.Ebx.StateControllerData>
	{
		public new FrostySdk.Ebx.StateControllerData Data => data as FrostySdk.Ebx.StateControllerData;
		public override string DisplayName => "StateController";

		public StateController(FrostySdk.Ebx.StateControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

