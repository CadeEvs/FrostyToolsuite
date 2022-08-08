using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPCompleteMissionEntityData))]
	public class SPCompleteMissionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPCompleteMissionEntityData>
	{
		public new FrostySdk.Ebx.SPCompleteMissionEntityData Data => data as FrostySdk.Ebx.SPCompleteMissionEntityData;
		public override string DisplayName => "SPCompleteMission";

		public SPCompleteMissionEntity(FrostySdk.Ebx.SPCompleteMissionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

