using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIListItemWidgetEntityData))]
	public class FBUIListItemWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.FBUIListItemWidgetEntityData>
	{
		public new FrostySdk.Ebx.FBUIListItemWidgetEntityData Data => data as FrostySdk.Ebx.FBUIListItemWidgetEntityData;
		public override string DisplayName => "FBUIListItemWidget";

		public FBUIListItemWidgetEntity(FrostySdk.Ebx.FBUIListItemWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

