using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroup_DynamicTagsEntityData))]
	public class SpawnBundleGroup_DynamicTagsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroup_DynamicTagsEntityData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroup_DynamicTagsEntityData Data => data as FrostySdk.Ebx.SpawnBundleGroup_DynamicTagsEntityData;
		public override string DisplayName => "SpawnBundleGroup_DynamicTags";

		public SpawnBundleGroup_DynamicTagsEntity(FrostySdk.Ebx.SpawnBundleGroup_DynamicTagsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

