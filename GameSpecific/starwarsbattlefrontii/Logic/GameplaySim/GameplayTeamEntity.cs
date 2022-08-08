using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameplayTeamEntityData))]
	public class GameplayTeamEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameplayTeamEntityData>
	{
		public new FrostySdk.Ebx.GameplayTeamEntityData Data => data as FrostySdk.Ebx.GameplayTeamEntityData;
		public override string DisplayName => "GameplayTeam";

		public GameplayTeamEntity(FrostySdk.Ebx.GameplayTeamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

