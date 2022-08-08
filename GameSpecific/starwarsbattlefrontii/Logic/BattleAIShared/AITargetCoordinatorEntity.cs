using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITargetCoordinatorEntityData))]
	public class AITargetCoordinatorEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AITargetCoordinatorEntityData>
	{
		public new FrostySdk.Ebx.AITargetCoordinatorEntityData Data => data as FrostySdk.Ebx.AITargetCoordinatorEntityData;
		public override string DisplayName => "AITargetCoordinator";

		public AITargetCoordinatorEntity(FrostySdk.Ebx.AITargetCoordinatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

