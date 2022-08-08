using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubLevelCollectionEntityData))]
	public class SubLevelCollectionEntity : DetachableSubWorldCollectionBase, IEntityData<FrostySdk.Ebx.SubLevelCollectionEntityData>
	{
		public new FrostySdk.Ebx.SubLevelCollectionEntityData Data => data as FrostySdk.Ebx.SubLevelCollectionEntityData;
		public override string DisplayName => "SubLevelCollection";

		public SubLevelCollectionEntity(FrostySdk.Ebx.SubLevelCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

