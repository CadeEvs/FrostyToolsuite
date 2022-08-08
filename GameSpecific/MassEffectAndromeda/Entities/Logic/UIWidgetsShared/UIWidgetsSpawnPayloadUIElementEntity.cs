using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsSpawnPayloadUIElementEntityData))]
	public class UIWidgetsSpawnPayloadUIElementEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsSpawnPayloadUIElementEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsSpawnPayloadUIElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsSpawnPayloadUIElementEntityData;
		public override string DisplayName => "UIWidgetsSpawnPayloadUIElement";

		public UIWidgetsSpawnPayloadUIElementEntity(FrostySdk.Ebx.UIWidgetsSpawnPayloadUIElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

