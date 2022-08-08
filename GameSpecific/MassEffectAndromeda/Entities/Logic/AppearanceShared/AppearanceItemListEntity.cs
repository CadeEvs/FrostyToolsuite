using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceItemListEntityData))]
	public class AppearanceItemListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AppearanceItemListEntityData>
	{
		public new FrostySdk.Ebx.AppearanceItemListEntityData Data => data as FrostySdk.Ebx.AppearanceItemListEntityData;
		public override string DisplayName => "AppearanceItemList";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AppearanceItemListEntity(FrostySdk.Ebx.AppearanceItemListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

