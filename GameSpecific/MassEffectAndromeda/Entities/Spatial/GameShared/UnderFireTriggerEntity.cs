using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnderFireTriggerEntityData))]
	public class UnderFireTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.UnderFireTriggerEntityData>
	{
		public new FrostySdk.Ebx.UnderFireTriggerEntityData Data => data as FrostySdk.Ebx.UnderFireTriggerEntityData;

		public UnderFireTriggerEntity(FrostySdk.Ebx.UnderFireTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

