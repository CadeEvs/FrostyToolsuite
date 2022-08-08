using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICombatBehaviorEntityData))]
	public class AICombatBehaviorEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AICombatBehaviorEntityData>
	{
		public new FrostySdk.Ebx.AICombatBehaviorEntityData Data => data as FrostySdk.Ebx.AICombatBehaviorEntityData;
		public override string DisplayName => "AICombatBehavior";

		public AICombatBehaviorEntity(FrostySdk.Ebx.AICombatBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

