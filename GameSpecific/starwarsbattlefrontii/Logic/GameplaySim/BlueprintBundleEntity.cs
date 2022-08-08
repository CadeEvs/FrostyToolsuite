using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintBundleEntityData))]
	public class BlueprintBundleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlueprintBundleEntityData>
	{
		public new FrostySdk.Ebx.BlueprintBundleEntityData Data => data as FrostySdk.Ebx.BlueprintBundleEntityData;
		public override string DisplayName => "BlueprintBundle";

		public BlueprintBundleEntity(FrostySdk.Ebx.BlueprintBundleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

