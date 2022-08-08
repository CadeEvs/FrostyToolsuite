using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITargetingEntityData))]
	public class AITargetingEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AITargetingEntityData>
	{
		public new FrostySdk.Ebx.AITargetingEntityData Data => data as FrostySdk.Ebx.AITargetingEntityData;
		public override string DisplayName => "AITargeting";

		public AITargetingEntity(FrostySdk.Ebx.AITargetingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

