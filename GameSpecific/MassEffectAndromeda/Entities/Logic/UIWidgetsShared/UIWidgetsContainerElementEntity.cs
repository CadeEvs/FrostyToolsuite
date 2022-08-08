using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsContainerElementEntityData))]
	public class UIWidgetsContainerElementEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsContainerElementEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsContainerElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsContainerElementEntityData;
		public override string DisplayName => "UIWidgetsContainerElement";

		public UIWidgetsContainerElementEntity(FrostySdk.Ebx.UIWidgetsContainerElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

