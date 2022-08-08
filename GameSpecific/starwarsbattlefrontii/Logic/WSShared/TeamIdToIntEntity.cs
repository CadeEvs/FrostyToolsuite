using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamIdToIntEntityData))]
	public class TeamIdToIntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeamIdToIntEntityData>
	{
		public new FrostySdk.Ebx.TeamIdToIntEntityData Data => data as FrostySdk.Ebx.TeamIdToIntEntityData;
		public override string DisplayName => "TeamIdToInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TeamIdToIntEntity(FrostySdk.Ebx.TeamIdToIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

