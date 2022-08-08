using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExecutingCSMStateData))]
	public class ExecutingCSMState : BWExecutingTimeline, IEntityData<FrostySdk.Ebx.ExecutingCSMStateData>
	{
		public new FrostySdk.Ebx.ExecutingCSMStateData Data => data as FrostySdk.Ebx.ExecutingCSMStateData;
		public override string DisplayName => "ExecutingCSMState";

		public ExecutingCSMState(FrostySdk.Ebx.ExecutingCSMStateData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

