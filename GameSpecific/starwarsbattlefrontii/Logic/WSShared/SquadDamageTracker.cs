using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadDamageTrackerData))]
	public class SquadDamageTracker : LogicEntity, IEntityData<FrostySdk.Ebx.SquadDamageTrackerData>
	{
		public new FrostySdk.Ebx.SquadDamageTrackerData Data => data as FrostySdk.Ebx.SquadDamageTrackerData;
		public override string DisplayName => "SquadDamageTracker";

		public SquadDamageTracker(FrostySdk.Ebx.SquadDamageTrackerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

