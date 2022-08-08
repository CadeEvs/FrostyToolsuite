using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEChangeTeamIdEntityData))]
	public class MEChangeTeamIdEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEChangeTeamIdEntityData>
	{
		public new FrostySdk.Ebx.MEChangeTeamIdEntityData Data => data as FrostySdk.Ebx.MEChangeTeamIdEntityData;
		public override string DisplayName => "MEChangeTeamId";

		public MEChangeTeamIdEntity(FrostySdk.Ebx.MEChangeTeamIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

