using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DeathAreaTriggerEntityData))]
	public class DeathAreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.DeathAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.DeathAreaTriggerEntityData Data => data as FrostySdk.Ebx.DeathAreaTriggerEntityData;

		public DeathAreaTriggerEntity(FrostySdk.Ebx.DeathAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

