using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamRelationEntityData))]
	public class TeamRelationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeamRelationEntityData>
	{
		public new FrostySdk.Ebx.TeamRelationEntityData Data => data as FrostySdk.Ebx.TeamRelationEntityData;
		public override string DisplayName => "TeamRelation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TeamRelationEntity(FrostySdk.Ebx.TeamRelationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

