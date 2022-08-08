using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotConditionOverrideEntityData))]
	public class PlotConditionOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlotConditionOverrideEntityData>
	{
		public new FrostySdk.Ebx.PlotConditionOverrideEntityData Data => data as FrostySdk.Ebx.PlotConditionOverrideEntityData;
		public override string DisplayName => "PlotConditionOverride";

		public PlotConditionOverrideEntity(FrostySdk.Ebx.PlotConditionOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

