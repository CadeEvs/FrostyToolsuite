using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsVideoElementEntityData))]
	public class UIWidgetsVideoElementEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsVideoElementEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsVideoElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsVideoElementEntityData;
		public override string DisplayName => "UIWidgetsVideoElement";

		public UIWidgetsVideoElementEntity(FrostySdk.Ebx.UIWidgetsVideoElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

