using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsLookedAtTargetOverrideEntityData))]
	public class IsLookedAtTargetOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsLookedAtTargetOverrideEntityData>
	{
		public new FrostySdk.Ebx.IsLookedAtTargetOverrideEntityData Data => data as FrostySdk.Ebx.IsLookedAtTargetOverrideEntityData;
		public override string DisplayName => "IsLookedAtTargetOverride";

		public IsLookedAtTargetOverrideEntity(FrostySdk.Ebx.IsLookedAtTargetOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

