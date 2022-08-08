using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintBundleAnimationStatusEntityData))]
	public class BlueprintBundleAnimationStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlueprintBundleAnimationStatusEntityData>
	{
		public new FrostySdk.Ebx.BlueprintBundleAnimationStatusEntityData Data => data as FrostySdk.Ebx.BlueprintBundleAnimationStatusEntityData;
		public override string DisplayName => "BlueprintBundleAnimationStatus";

		public BlueprintBundleAnimationStatusEntity(FrostySdk.Ebx.BlueprintBundleAnimationStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

