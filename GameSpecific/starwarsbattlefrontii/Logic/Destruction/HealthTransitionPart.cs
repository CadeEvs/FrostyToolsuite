using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HealthTransitionPartData))]
	public class HealthTransitionPart : LogicEntity, IEntityData<FrostySdk.Ebx.HealthTransitionPartData>
	{
		public new FrostySdk.Ebx.HealthTransitionPartData Data => data as FrostySdk.Ebx.HealthTransitionPartData;
		public override string DisplayName => "HealthTransitionPart";

		public HealthTransitionPart(FrostySdk.Ebx.HealthTransitionPartData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

