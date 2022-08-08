using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPMissionHandlingControlEntityData))]
	public class SPMissionHandlingControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPMissionHandlingControlEntityData>
	{
		public new FrostySdk.Ebx.SPMissionHandlingControlEntityData Data => data as FrostySdk.Ebx.SPMissionHandlingControlEntityData;
		public override string DisplayName => "SPMissionHandlingControl";

		public SPMissionHandlingControlEntity(FrostySdk.Ebx.SPMissionHandlingControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

