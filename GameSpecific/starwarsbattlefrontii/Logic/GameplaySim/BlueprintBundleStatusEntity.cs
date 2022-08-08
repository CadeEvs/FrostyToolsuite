using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintBundleStatusEntityData))]
	public class BlueprintBundleStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlueprintBundleStatusEntityData>
	{
		public new FrostySdk.Ebx.BlueprintBundleStatusEntityData Data => data as FrostySdk.Ebx.BlueprintBundleStatusEntityData;
		public override string DisplayName => "BlueprintBundleStatus";

		public BlueprintBundleStatusEntity(FrostySdk.Ebx.BlueprintBundleStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

