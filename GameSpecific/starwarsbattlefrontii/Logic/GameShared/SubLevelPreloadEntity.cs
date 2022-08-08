using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubLevelPreloadEntityData))]
	public class SubLevelPreloadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SubLevelPreloadEntityData>
	{
		public new FrostySdk.Ebx.SubLevelPreloadEntityData Data => data as FrostySdk.Ebx.SubLevelPreloadEntityData;
		public override string DisplayName => "SubLevelPreload";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SubLevelPreloadEntity(FrostySdk.Ebx.SubLevelPreloadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

