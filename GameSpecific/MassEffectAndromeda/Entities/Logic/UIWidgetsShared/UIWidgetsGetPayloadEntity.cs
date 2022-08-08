using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsGetPayloadEntityData))]
	public class UIWidgetsGetPayloadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetsGetPayloadEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsGetPayloadEntityData Data => data as FrostySdk.Ebx.UIWidgetsGetPayloadEntityData;
		public override string DisplayName => "UIWidgetsGetPayload";

		public UIWidgetsGetPayloadEntity(FrostySdk.Ebx.UIWidgetsGetPayloadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

