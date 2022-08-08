using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIFlyToBehaviourEntityData))]
	public class SquadronAIFlyToBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIFlyToBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIFlyToBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIFlyToBehaviourEntityData;
		public override string DisplayName => "SquadronAIFlyToBehaviour";

		public SquadronAIFlyToBehaviourEntity(FrostySdk.Ebx.SquadronAIFlyToBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

