using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsTexturePositioningLogicData))]
	public class UIWidgetsTexturePositioningLogic : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetsTexturePositioningLogicData>
	{
		public new FrostySdk.Ebx.UIWidgetsTexturePositioningLogicData Data => data as FrostySdk.Ebx.UIWidgetsTexturePositioningLogicData;
		public override string DisplayName => "UIWidgetsTexturePositioningLogic";

		public UIWidgetsTexturePositioningLogic(FrostySdk.Ebx.UIWidgetsTexturePositioningLogicData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

