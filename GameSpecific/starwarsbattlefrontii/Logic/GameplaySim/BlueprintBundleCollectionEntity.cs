using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintBundleCollectionEntityData))]
	public class BlueprintBundleCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlueprintBundleCollectionEntityData>
	{
		public new FrostySdk.Ebx.BlueprintBundleCollectionEntityData Data => data as FrostySdk.Ebx.BlueprintBundleCollectionEntityData;
		public override string DisplayName => "BlueprintBundleCollection";

		public BlueprintBundleCollectionEntity(FrostySdk.Ebx.BlueprintBundleCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

