using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectTeamIdEntityData))]
	public class SelectTeamIdEntity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectTeamIdEntityData>
	{
		public new FrostySdk.Ebx.SelectTeamIdEntityData Data => data as FrostySdk.Ebx.SelectTeamIdEntityData;
		public override string DisplayName => "SelectTeamId";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SelectTeamIdEntity(FrostySdk.Ebx.SelectTeamIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

