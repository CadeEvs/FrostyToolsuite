using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLastSavedMissionEntityData))]
	public class SPLastSavedMissionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLastSavedMissionEntityData>
	{
		public new FrostySdk.Ebx.SPLastSavedMissionEntityData Data => data as FrostySdk.Ebx.SPLastSavedMissionEntityData;
		public override string DisplayName => "SPLastSavedMission";

		public SPLastSavedMissionEntity(FrostySdk.Ebx.SPLastSavedMissionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

