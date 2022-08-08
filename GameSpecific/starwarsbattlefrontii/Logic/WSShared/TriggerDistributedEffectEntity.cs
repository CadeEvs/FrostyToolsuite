using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerDistributedEffectEntityData))]
	public class TriggerDistributedEffectEntity : DistributedEffectEntity, IEntityData<FrostySdk.Ebx.TriggerDistributedEffectEntityData>
	{
		public new FrostySdk.Ebx.TriggerDistributedEffectEntityData Data => data as FrostySdk.Ebx.TriggerDistributedEffectEntityData;
		public override string DisplayName => "TriggerDistributedEffect";

		public TriggerDistributedEffectEntity(FrostySdk.Ebx.TriggerDistributedEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

