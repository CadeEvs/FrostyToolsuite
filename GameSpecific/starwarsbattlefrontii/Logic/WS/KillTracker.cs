using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KillTrackerData))]
	public class KillTracker : LogicEntity, IEntityData<FrostySdk.Ebx.KillTrackerData>
	{
		public new FrostySdk.Ebx.KillTrackerData Data => data as FrostySdk.Ebx.KillTrackerData;
		public override string DisplayName => "KillTracker";

		public KillTracker(FrostySdk.Ebx.KillTrackerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

