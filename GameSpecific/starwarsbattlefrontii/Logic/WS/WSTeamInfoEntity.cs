using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSTeamInfoEntityData))]
	public class WSTeamInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSTeamInfoEntityData>
	{
		public new FrostySdk.Ebx.WSTeamInfoEntityData Data => data as FrostySdk.Ebx.WSTeamInfoEntityData;
		public override string DisplayName => "WSTeamInfo";

		public WSTeamInfoEntity(FrostySdk.Ebx.WSTeamInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

