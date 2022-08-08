using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSpawnBundleCollectionProviderData))]
	public class WSSpawnBundleCollectionProvider : LogicEntity, IEntityData<FrostySdk.Ebx.WSSpawnBundleCollectionProviderData>
	{
		public new FrostySdk.Ebx.WSSpawnBundleCollectionProviderData Data => data as FrostySdk.Ebx.WSSpawnBundleCollectionProviderData;
		public override string DisplayName => "WSSpawnBundleCollectionProvider";

		public WSSpawnBundleCollectionProvider(FrostySdk.Ebx.WSSpawnBundleCollectionProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

