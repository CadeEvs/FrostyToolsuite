using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsTextFieldEntityData))]
	public class UIWidgetsTextFieldEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsTextFieldEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsTextFieldEntityData Data => data as FrostySdk.Ebx.UIWidgetsTextFieldEntityData;
		public override string DisplayName => "UIWidgetsTextField";

		public UIWidgetsTextFieldEntity(FrostySdk.Ebx.UIWidgetsTextFieldEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

