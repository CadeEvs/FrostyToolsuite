using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InactivityTimeoutTriggerEntityData))]
	public class InactivityTimeoutTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InactivityTimeoutTriggerEntityData>
	{
		public new FrostySdk.Ebx.InactivityTimeoutTriggerEntityData Data => data as FrostySdk.Ebx.InactivityTimeoutTriggerEntityData;
		public override string DisplayName => "InactivityTimeoutTrigger";

		public InactivityTimeoutTriggerEntity(FrostySdk.Ebx.InactivityTimeoutTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

