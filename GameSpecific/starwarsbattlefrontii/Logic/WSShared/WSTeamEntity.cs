using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSTeamEntityData))]
	public class WSTeamEntity : TeamEntity, IEntityData<FrostySdk.Ebx.WSTeamEntityData>
	{
		public new FrostySdk.Ebx.WSTeamEntityData Data => data as FrostySdk.Ebx.WSTeamEntityData;
		public override string DisplayName => "WSTeam";

		public WSTeamEntity(FrostySdk.Ebx.WSTeamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

