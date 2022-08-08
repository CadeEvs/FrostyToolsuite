using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAITargetProviderEntityData))]
	public class SquadronAITargetProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAITargetProviderEntityData>
	{
		public new FrostySdk.Ebx.SquadronAITargetProviderEntityData Data => data as FrostySdk.Ebx.SquadronAITargetProviderEntityData;
		public override string DisplayName => "SquadronAITargetProvider";

		public SquadronAITargetProviderEntity(FrostySdk.Ebx.SquadronAITargetProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

