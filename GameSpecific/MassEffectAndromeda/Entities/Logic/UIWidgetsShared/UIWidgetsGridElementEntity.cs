using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsGridElementEntityData))]
	public class UIWidgetsGridElementEntity : UIWidgetsContainerElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsGridElementEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsGridElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsGridElementEntityData;
		public override string DisplayName => "UIWidgetsGridElement";

		public UIWidgetsGridElementEntity(FrostySdk.Ebx.UIWidgetsGridElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

