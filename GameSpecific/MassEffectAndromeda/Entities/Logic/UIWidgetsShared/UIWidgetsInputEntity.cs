using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsInputEntityData))]
	public class UIWidgetsInputEntity : UIInputEntity, IEntityData<FrostySdk.Ebx.UIWidgetsInputEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsInputEntityData Data => data as FrostySdk.Ebx.UIWidgetsInputEntityData;
		public override string DisplayName => "UIWidgetsInput";

		public UIWidgetsInputEntity(FrostySdk.Ebx.UIWidgetsInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

