using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamEntityData))]
	public class TeamEntity : GameplayTeamEntity, IEntityData<FrostySdk.Ebx.TeamEntityData>
	{
		public new FrostySdk.Ebx.TeamEntityData Data => data as FrostySdk.Ebx.TeamEntityData;
		public override string DisplayName => "Team";

		public TeamEntity(FrostySdk.Ebx.TeamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

