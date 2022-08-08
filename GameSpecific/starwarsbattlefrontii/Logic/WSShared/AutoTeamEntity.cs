using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoTeamEntityData))]
	public class AutoTeamEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoTeamEntityData>
	{
		public new FrostySdk.Ebx.AutoTeamEntityData Data => data as FrostySdk.Ebx.AutoTeamEntityData;
		public override string DisplayName => "AutoTeam";

		public AutoTeamEntity(FrostySdk.Ebx.AutoTeamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

