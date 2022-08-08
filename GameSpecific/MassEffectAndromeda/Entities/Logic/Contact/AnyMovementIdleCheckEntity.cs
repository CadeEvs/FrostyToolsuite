using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnyMovementIdleCheckEntityData))]
	public class AnyMovementIdleCheckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnyMovementIdleCheckEntityData>
	{
		public new FrostySdk.Ebx.AnyMovementIdleCheckEntityData Data => data as FrostySdk.Ebx.AnyMovementIdleCheckEntityData;
		public override string DisplayName => "AnyMovementIdleCheck";

		public AnyMovementIdleCheckEntity(FrostySdk.Ebx.AnyMovementIdleCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

