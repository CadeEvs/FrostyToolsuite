using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPMissionHandlingInfoEntityData))]
	public class SPMissionHandlingInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPMissionHandlingInfoEntityData>
	{
		public new FrostySdk.Ebx.SPMissionHandlingInfoEntityData Data => data as FrostySdk.Ebx.SPMissionHandlingInfoEntityData;
		public override string DisplayName => "SPMissionHandlingInfo";

		public SPMissionHandlingInfoEntity(FrostySdk.Ebx.SPMissionHandlingInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

