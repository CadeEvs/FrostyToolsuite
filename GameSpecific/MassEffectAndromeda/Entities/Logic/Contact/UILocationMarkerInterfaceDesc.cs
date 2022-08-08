using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILocationMarkerInterfaceDescData))]
	public class UILocationMarkerInterfaceDesc : UIChildItemInterfaceDescriptorBase, IEntityData<FrostySdk.Ebx.UILocationMarkerInterfaceDescData>
	{
		public new FrostySdk.Ebx.UILocationMarkerInterfaceDescData Data => data as FrostySdk.Ebx.UILocationMarkerInterfaceDescData;
		public override string DisplayName => "UILocationMarkerInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UILocationMarkerInterfaceDesc(FrostySdk.Ebx.UILocationMarkerInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

