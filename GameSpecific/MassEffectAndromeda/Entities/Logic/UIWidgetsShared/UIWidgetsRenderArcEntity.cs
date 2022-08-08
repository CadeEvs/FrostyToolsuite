using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsRenderArcEntityData))]
	public class UIWidgetsRenderArcEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsRenderArcEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsRenderArcEntityData Data => data as FrostySdk.Ebx.UIWidgetsRenderArcEntityData;
		public override string DisplayName => "UIWidgetsRenderArc";

		public UIWidgetsRenderArcEntity(FrostySdk.Ebx.UIWidgetsRenderArcEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

