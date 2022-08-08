using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIButtonLegendChildInterfaceDescData))]
	public class UIButtonLegendChildInterfaceDesc : UIChildItemInterfaceDescriptorBase, IEntityData<FrostySdk.Ebx.UIButtonLegendChildInterfaceDescData>
	{
		public new FrostySdk.Ebx.UIButtonLegendChildInterfaceDescData Data => data as FrostySdk.Ebx.UIButtonLegendChildInterfaceDescData;
		public override string DisplayName => "UIButtonLegendChildInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIButtonLegendChildInterfaceDesc(FrostySdk.Ebx.UIButtonLegendChildInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

