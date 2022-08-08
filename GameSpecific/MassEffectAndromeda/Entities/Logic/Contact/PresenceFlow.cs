using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PresenceFlowData))]
	public class PresenceFlow : LogicEntity, IEntityData<FrostySdk.Ebx.PresenceFlowData>
	{
		public new FrostySdk.Ebx.PresenceFlowData Data => data as FrostySdk.Ebx.PresenceFlowData;
		public override string DisplayName => "PresenceFlow";

		public PresenceFlow(FrostySdk.Ebx.PresenceFlowData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

