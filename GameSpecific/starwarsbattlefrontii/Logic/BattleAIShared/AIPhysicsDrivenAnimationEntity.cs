using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPhysicsDrivenAnimationEntityData))]
	public class AIPhysicsDrivenAnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPhysicsDrivenAnimationEntityData>
	{
		public new FrostySdk.Ebx.AIPhysicsDrivenAnimationEntityData Data => data as FrostySdk.Ebx.AIPhysicsDrivenAnimationEntityData;
		public override string DisplayName => "AIPhysicsDrivenAnimation";

		public AIPhysicsDrivenAnimationEntity(FrostySdk.Ebx.AIPhysicsDrivenAnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

