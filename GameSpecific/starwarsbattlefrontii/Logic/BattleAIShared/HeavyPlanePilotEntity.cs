using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeavyPlanePilotEntityData))]
	public class HeavyPlanePilotEntity : PilotEntity, IEntityData<FrostySdk.Ebx.HeavyPlanePilotEntityData>
	{
		public new FrostySdk.Ebx.HeavyPlanePilotEntityData Data => data as FrostySdk.Ebx.HeavyPlanePilotEntityData;
		public override string DisplayName => "HeavyPlanePilot";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HeavyPlanePilotEntity(FrostySdk.Ebx.HeavyPlanePilotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

