using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HealthTransitionData))]
	public class HealthTransition : LogicEntity, IEntityData<FrostySdk.Ebx.HealthTransitionData>
	{
		public new FrostySdk.Ebx.HealthTransitionData Data => data as FrostySdk.Ebx.HealthTransitionData;
		public override string DisplayName => "HealthTransition";

		public HealthTransition(FrostySdk.Ebx.HealthTransitionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

