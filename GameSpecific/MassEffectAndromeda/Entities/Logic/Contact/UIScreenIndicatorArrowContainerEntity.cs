using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenIndicatorArrowContainerEntityData))]
	public class UIScreenIndicatorArrowContainerEntity : UIContainerEntity, IEntityData<FrostySdk.Ebx.UIScreenIndicatorArrowContainerEntityData>
	{
		public new FrostySdk.Ebx.UIScreenIndicatorArrowContainerEntityData Data => data as FrostySdk.Ebx.UIScreenIndicatorArrowContainerEntityData;
		public override string DisplayName => "UIScreenIndicatorArrowContainer";

		public UIScreenIndicatorArrowContainerEntity(FrostySdk.Ebx.UIScreenIndicatorArrowContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

