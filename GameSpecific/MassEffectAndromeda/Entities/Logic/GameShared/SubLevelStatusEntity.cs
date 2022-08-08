using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubLevelStatusEntityData))]
	public class SubLevelStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SubLevelStatusEntityData>
	{
		public new FrostySdk.Ebx.SubLevelStatusEntityData Data => data as FrostySdk.Ebx.SubLevelStatusEntityData;
		public override string DisplayName => "SubLevelStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SubLevelStatusEntity(FrostySdk.Ebx.SubLevelStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

