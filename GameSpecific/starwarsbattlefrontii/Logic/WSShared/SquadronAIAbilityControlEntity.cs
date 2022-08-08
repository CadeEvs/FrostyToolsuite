using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIAbilityControlEntityData))]
	public class SquadronAIAbilityControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIAbilityControlEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIAbilityControlEntityData Data => data as FrostySdk.Ebx.SquadronAIAbilityControlEntityData;
		public override string DisplayName => "SquadronAIAbilityControl";

		public SquadronAIAbilityControlEntity(FrostySdk.Ebx.SquadronAIAbilityControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

