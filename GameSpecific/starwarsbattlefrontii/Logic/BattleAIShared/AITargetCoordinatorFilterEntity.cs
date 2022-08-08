using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITargetCoordinatorFilterEntityData))]
	public class AITargetCoordinatorFilterEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AITargetCoordinatorFilterEntityData>
	{
		public new FrostySdk.Ebx.AITargetCoordinatorFilterEntityData Data => data as FrostySdk.Ebx.AITargetCoordinatorFilterEntityData;
		public override string DisplayName => "AITargetCoordinatorFilter";

		public AITargetCoordinatorFilterEntity(FrostySdk.Ebx.AITargetCoordinatorFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

