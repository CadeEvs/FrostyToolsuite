using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementLayerEntityData))]
	public class UIElementLayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIElementLayerEntityData>
	{
		public new FrostySdk.Ebx.UIElementLayerEntityData Data => data as FrostySdk.Ebx.UIElementLayerEntityData;
		public override string DisplayName => "UIElementLayer";

		public UIElementLayerEntity(FrostySdk.Ebx.UIElementLayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

