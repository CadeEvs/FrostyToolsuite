using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingTargetAdhesionControllerData))]
	public class TargetingTargetAdhesionController : LogicController, IEntityData<FrostySdk.Ebx.TargetingTargetAdhesionControllerData>
	{
		public new FrostySdk.Ebx.TargetingTargetAdhesionControllerData Data => data as FrostySdk.Ebx.TargetingTargetAdhesionControllerData;
		public override string DisplayName => "TargetingTargetAdhesionController";

		public TargetingTargetAdhesionController(FrostySdk.Ebx.TargetingTargetAdhesionControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

