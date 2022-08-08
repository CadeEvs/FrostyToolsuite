using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetTeamEntityData))]
	public class SetTeamEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetTeamEntityData>
	{
		public new FrostySdk.Ebx.SetTeamEntityData Data => data as FrostySdk.Ebx.SetTeamEntityData;
		public override string DisplayName => "SetTeam";

		public SetTeamEntity(FrostySdk.Ebx.SetTeamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

