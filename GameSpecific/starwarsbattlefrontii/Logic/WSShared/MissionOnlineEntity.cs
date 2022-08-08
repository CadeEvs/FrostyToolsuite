using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissionOnlineEntityData))]
	public class MissionOnlineEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MissionOnlineEntityData>
	{
		public new FrostySdk.Ebx.MissionOnlineEntityData Data => data as FrostySdk.Ebx.MissionOnlineEntityData;
		public override string DisplayName => "MissionOnline";

		public MissionOnlineEntity(FrostySdk.Ebx.MissionOnlineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

