using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIRecoveryLocatorEntityData))]
	public class SquadronAIRecoveryLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SquadronAIRecoveryLocatorEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIRecoveryLocatorEntityData Data => data as FrostySdk.Ebx.SquadronAIRecoveryLocatorEntityData;

		public SquadronAIRecoveryLocatorEntity(FrostySdk.Ebx.SquadronAIRecoveryLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

