using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEDamageAreaTriggerEntityData))]
	public class MEDamageAreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.MEDamageAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.MEDamageAreaTriggerEntityData Data => data as FrostySdk.Ebx.MEDamageAreaTriggerEntityData;

		public MEDamageAreaTriggerEntity(FrostySdk.Ebx.MEDamageAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

