using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICoverZonesOverrideEntityData))]
	public class AICoverZonesOverrideEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AICoverZonesOverrideEntityData>
	{
		public new FrostySdk.Ebx.AICoverZonesOverrideEntityData Data => data as FrostySdk.Ebx.AICoverZonesOverrideEntityData;
		public override string DisplayName => "AICoverZonesOverride";

		public AICoverZonesOverrideEntity(FrostySdk.Ebx.AICoverZonesOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

