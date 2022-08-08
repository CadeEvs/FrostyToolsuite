using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamFilterEntityData))]
	public class TeamFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeamFilterEntityData>
	{
		public new FrostySdk.Ebx.TeamFilterEntityData Data => data as FrostySdk.Ebx.TeamFilterEntityData;
		public override string DisplayName => "TeamFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TeamFilterEntity(FrostySdk.Ebx.TeamFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

