using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistributedEffectEntityData))]
	public class DistributedEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DistributedEffectEntityData>
	{
		public new FrostySdk.Ebx.DistributedEffectEntityData Data => data as FrostySdk.Ebx.DistributedEffectEntityData;
		public override string DisplayName => "DistributedEffect";

		public DistributedEffectEntity(FrostySdk.Ebx.DistributedEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

