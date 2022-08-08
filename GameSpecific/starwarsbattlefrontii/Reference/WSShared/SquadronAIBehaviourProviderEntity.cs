using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIBehaviourProviderEntityData))]
	public class SquadronAIBehaviourProviderEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.SquadronAIBehaviourProviderEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIBehaviourProviderEntityData Data => data as FrostySdk.Ebx.SquadronAIBehaviourProviderEntityData;

		public SquadronAIBehaviourProviderEntity(FrostySdk.Ebx.SquadronAIBehaviourProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

