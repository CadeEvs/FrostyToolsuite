using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetGalaxyObjectCategoryInfoData))]
	public class GetGalaxyObjectCategoryInfo : LogicEntity, IEntityData<FrostySdk.Ebx.GetGalaxyObjectCategoryInfoData>
	{
		public new FrostySdk.Ebx.GetGalaxyObjectCategoryInfoData Data => data as FrostySdk.Ebx.GetGalaxyObjectCategoryInfoData;
		public override string DisplayName => "GetGalaxyObjectCategoryInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GetGalaxyObjectCategoryInfo(FrostySdk.Ebx.GetGalaxyObjectCategoryInfoData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

