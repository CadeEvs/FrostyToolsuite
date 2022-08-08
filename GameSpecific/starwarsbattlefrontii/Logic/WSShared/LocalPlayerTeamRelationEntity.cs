using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerTeamRelationEntityData))]
	public class LocalPlayerTeamRelationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerTeamRelationEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerTeamRelationEntityData Data => data as FrostySdk.Ebx.LocalPlayerTeamRelationEntityData;
		public override string DisplayName => "LocalPlayerTeamRelation";

		public LocalPlayerTeamRelationEntity(FrostySdk.Ebx.LocalPlayerTeamRelationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

