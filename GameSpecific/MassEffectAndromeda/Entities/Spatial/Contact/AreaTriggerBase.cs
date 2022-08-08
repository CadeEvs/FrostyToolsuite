using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaTriggerBaseData))]
	public class AreaTriggerBase : TriggerEntity, IEntityData<FrostySdk.Ebx.AreaTriggerBaseData>
	{
		public new FrostySdk.Ebx.AreaTriggerBaseData Data => data as FrostySdk.Ebx.AreaTriggerBaseData;

		public AreaTriggerBase(FrostySdk.Ebx.AreaTriggerBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

