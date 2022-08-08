using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPMissionHandlingStatusEntityData))]
	public class SPMissionHandlingStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPMissionHandlingStatusEntityData>
	{
		public new FrostySdk.Ebx.SPMissionHandlingStatusEntityData Data => data as FrostySdk.Ebx.SPMissionHandlingStatusEntityData;
		public override string DisplayName => "SPMissionHandlingStatus";

		public SPMissionHandlingStatusEntity(FrostySdk.Ebx.SPMissionHandlingStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

