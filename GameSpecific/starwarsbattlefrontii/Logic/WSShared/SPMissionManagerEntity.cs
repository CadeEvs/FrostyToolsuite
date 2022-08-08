using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPMissionManagerEntityData))]
	public class SPMissionManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPMissionManagerEntityData>
	{
		public new FrostySdk.Ebx.SPMissionManagerEntityData Data => data as FrostySdk.Ebx.SPMissionManagerEntityData;
		public override string DisplayName => "SPMissionManager";

		public SPMissionManagerEntity(FrostySdk.Ebx.SPMissionManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

