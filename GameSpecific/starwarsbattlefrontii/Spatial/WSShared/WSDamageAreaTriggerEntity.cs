using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSDamageAreaTriggerEntityData))]
	public class WSDamageAreaTriggerEntity : DamageAreaTriggerEntity, IEntityData<FrostySdk.Ebx.WSDamageAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.WSDamageAreaTriggerEntityData Data => data as FrostySdk.Ebx.WSDamageAreaTriggerEntityData;

		public WSDamageAreaTriggerEntity(FrostySdk.Ebx.WSDamageAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

