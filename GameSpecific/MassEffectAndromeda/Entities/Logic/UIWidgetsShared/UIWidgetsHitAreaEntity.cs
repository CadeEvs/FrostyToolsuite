using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsHitAreaEntityData))]
	public class UIWidgetsHitAreaEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsHitAreaEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsHitAreaEntityData Data => data as FrostySdk.Ebx.UIWidgetsHitAreaEntityData;
		public override string DisplayName => "UIWidgetsHitArea";

		public UIWidgetsHitAreaEntity(FrostySdk.Ebx.UIWidgetsHitAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

