using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AliveTriggerData))]
	public class AliveTrigger : LogicEntity, IEntityData<FrostySdk.Ebx.AliveTriggerData>
	{
		public new FrostySdk.Ebx.AliveTriggerData Data => data as FrostySdk.Ebx.AliveTriggerData;
		public override string DisplayName => "AliveTrigger";

		public AliveTrigger(FrostySdk.Ebx.AliveTriggerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

