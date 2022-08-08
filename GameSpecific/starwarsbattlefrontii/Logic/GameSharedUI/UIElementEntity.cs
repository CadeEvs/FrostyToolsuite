using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementEntityData))]
	public class UIElementEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIElementEntityData>
	{
		public new FrostySdk.Ebx.UIElementEntityData Data => data as FrostySdk.Ebx.UIElementEntityData;
		public override string DisplayName => "UIElement";

		public UIElementEntity(FrostySdk.Ebx.UIElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

