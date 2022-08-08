using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsSetPayloadEntityData))]
	public class UIWidgetsSetPayloadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetsSetPayloadEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsSetPayloadEntityData Data => data as FrostySdk.Ebx.UIWidgetsSetPayloadEntityData;
		public override string DisplayName => "UIWidgetsSetPayload";

		public UIWidgetsSetPayloadEntity(FrostySdk.Ebx.UIWidgetsSetPayloadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

