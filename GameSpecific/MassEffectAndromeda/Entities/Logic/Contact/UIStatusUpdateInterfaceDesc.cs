using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStatusUpdateInterfaceDescData))]
	public class UIStatusUpdateInterfaceDesc : UIChildItemInterfaceDescriptorBase, IEntityData<FrostySdk.Ebx.UIStatusUpdateInterfaceDescData>
	{
		public new FrostySdk.Ebx.UIStatusUpdateInterfaceDescData Data => data as FrostySdk.Ebx.UIStatusUpdateInterfaceDescData;
		public override string DisplayName => "UIStatusUpdateInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIStatusUpdateInterfaceDesc(FrostySdk.Ebx.UIStatusUpdateInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

