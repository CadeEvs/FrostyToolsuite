using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsClearPayloadEntityData))]
	public class UIWidgetsClearPayloadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetsClearPayloadEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsClearPayloadEntityData Data => data as FrostySdk.Ebx.UIWidgetsClearPayloadEntityData;
		public override string DisplayName => "UIWidgetsClearPayload";

		public UIWidgetsClearPayloadEntity(FrostySdk.Ebx.UIWidgetsClearPayloadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

