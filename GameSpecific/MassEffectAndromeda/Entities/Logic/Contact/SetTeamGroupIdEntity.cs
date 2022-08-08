using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetTeamGroupIdEntityData))]
	public class SetTeamGroupIdEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetTeamGroupIdEntityData>
	{
		public new FrostySdk.Ebx.SetTeamGroupIdEntityData Data => data as FrostySdk.Ebx.SetTeamGroupIdEntityData;
		public override string DisplayName => "SetTeamGroupId";

		public SetTeamGroupIdEntity(FrostySdk.Ebx.SetTeamGroupIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

