using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientSkillpointInfoEntityData))]
	public class ClientSkillpointInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientSkillpointInfoEntityData>
	{
		public new FrostySdk.Ebx.ClientSkillpointInfoEntityData Data => data as FrostySdk.Ebx.ClientSkillpointInfoEntityData;
		public override string DisplayName => "ClientSkillpointInfo";

		public ClientSkillpointInfoEntity(FrostySdk.Ebx.ClientSkillpointInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

