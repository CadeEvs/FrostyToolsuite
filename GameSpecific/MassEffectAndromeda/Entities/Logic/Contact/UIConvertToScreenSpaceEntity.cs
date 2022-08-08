using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIConvertToScreenSpaceEntityData))]
	public class UIConvertToScreenSpaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIConvertToScreenSpaceEntityData>
	{
		public new FrostySdk.Ebx.UIConvertToScreenSpaceEntityData Data => data as FrostySdk.Ebx.UIConvertToScreenSpaceEntityData;
		public override string DisplayName => "UIConvertToScreenSpace";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIConvertToScreenSpaceEntity(FrostySdk.Ebx.UIConvertToScreenSpaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

