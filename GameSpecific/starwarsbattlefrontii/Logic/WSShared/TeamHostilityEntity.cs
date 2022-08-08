using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamHostilityEntityData))]
	public class TeamHostilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeamHostilityEntityData>
	{
		public new FrostySdk.Ebx.TeamHostilityEntityData Data => data as FrostySdk.Ebx.TeamHostilityEntityData;
		public override string DisplayName => "TeamHostility";

		public TeamHostilityEntity(FrostySdk.Ebx.TeamHostilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

