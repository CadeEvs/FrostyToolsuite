using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForceTeamFullAwarenessEntityData))]
	public class ForceTeamFullAwarenessEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ForceTeamFullAwarenessEntityData>
	{
		public new FrostySdk.Ebx.ForceTeamFullAwarenessEntityData Data => data as FrostySdk.Ebx.ForceTeamFullAwarenessEntityData;
		public override string DisplayName => "ForceTeamFullAwareness";

		public ForceTeamFullAwarenessEntity(FrostySdk.Ebx.ForceTeamFullAwarenessEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

