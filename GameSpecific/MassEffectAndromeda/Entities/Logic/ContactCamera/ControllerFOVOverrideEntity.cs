using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerFOVOverrideEntityData))]
	public class ControllerFOVOverrideEntity : ControllerSpecificActionEntity, IEntityData<FrostySdk.Ebx.ControllerFOVOverrideEntityData>
	{
		public new FrostySdk.Ebx.ControllerFOVOverrideEntityData Data => data as FrostySdk.Ebx.ControllerFOVOverrideEntityData;
		public override string DisplayName => "ControllerFOVOverride";

		public ControllerFOVOverrideEntity(FrostySdk.Ebx.ControllerFOVOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

