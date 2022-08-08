using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistributedEffectReceiveEntityData))]
	public class DistributedEffectReceiveEntity : DistributedEffectEntity, IEntityData<FrostySdk.Ebx.DistributedEffectReceiveEntityData>
	{
		public new FrostySdk.Ebx.DistributedEffectReceiveEntityData Data => data as FrostySdk.Ebx.DistributedEffectReceiveEntityData;
		public override string DisplayName => "DistributedEffectReceive";

		public DistributedEffectReceiveEntity(FrostySdk.Ebx.DistributedEffectReceiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

