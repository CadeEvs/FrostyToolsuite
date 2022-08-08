using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIBaseSelectorChildInterfaceDescData))]
	public class UIBaseSelectorChildInterfaceDesc : UIChildItemInterfaceDescriptorBase, IEntityData<FrostySdk.Ebx.UIBaseSelectorChildInterfaceDescData>
	{
		public new FrostySdk.Ebx.UIBaseSelectorChildInterfaceDescData Data => data as FrostySdk.Ebx.UIBaseSelectorChildInterfaceDescData;
		public override string DisplayName => "UIBaseSelectorChildInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIBaseSelectorChildInterfaceDesc(FrostySdk.Ebx.UIBaseSelectorChildInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

