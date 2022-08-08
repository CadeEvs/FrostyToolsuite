using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIIdleBehaviorEntityData))]
	public class AIIdleBehaviorEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIIdleBehaviorEntityData>
	{
		public new FrostySdk.Ebx.AIIdleBehaviorEntityData Data => data as FrostySdk.Ebx.AIIdleBehaviorEntityData;
		public override string DisplayName => "AIIdleBehavior";

		public AIIdleBehaviorEntity(FrostySdk.Ebx.AIIdleBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

