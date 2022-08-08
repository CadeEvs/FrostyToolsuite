using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIBreadcrumbElementData))]
	public class UIBreadcrumbElement : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIBreadcrumbElementData>
	{
		public new FrostySdk.Ebx.UIBreadcrumbElementData Data => data as FrostySdk.Ebx.UIBreadcrumbElementData;
		public override string DisplayName => "UIBreadcrumbElement";

		public UIBreadcrumbElement(FrostySdk.Ebx.UIBreadcrumbElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

