using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsListElementEntityData))]
	public class UIWidgetsListElementEntity : UIWidgetsContainerElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsListElementEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsListElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsListElementEntityData;
		public override string DisplayName => "UIWidgetsListElement";

		public UIWidgetsListElementEntity(FrostySdk.Ebx.UIWidgetsListElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

