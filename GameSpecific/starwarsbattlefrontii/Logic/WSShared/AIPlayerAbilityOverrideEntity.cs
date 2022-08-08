using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPlayerAbilityOverrideEntityData))]
	public class AIPlayerAbilityOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPlayerAbilityOverrideEntityData>
	{
		public new FrostySdk.Ebx.AIPlayerAbilityOverrideEntityData Data => data as FrostySdk.Ebx.AIPlayerAbilityOverrideEntityData;
		public override string DisplayName => "AIPlayerAbilityOverride";

		public AIPlayerAbilityOverrideEntity(FrostySdk.Ebx.AIPlayerAbilityOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

