using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIContainerEntityData))]
	public class UIContainerEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.UIContainerEntityData>
	{
		public new FrostySdk.Ebx.UIContainerEntityData Data => data as FrostySdk.Ebx.UIContainerEntityData;
		public override string DisplayName => "UIContainer";

		public UIContainerEntity(FrostySdk.Ebx.UIContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

