using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWorldMapIconInterfaceDescData))]
	public class UIWorldMapIconInterfaceDesc : UILocationMarkerInterfaceDesc, IEntityData<FrostySdk.Ebx.UIWorldMapIconInterfaceDescData>
	{
		public new FrostySdk.Ebx.UIWorldMapIconInterfaceDescData Data => data as FrostySdk.Ebx.UIWorldMapIconInterfaceDescData;
		public override string DisplayName => "UIWorldMapIconInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIWorldMapIconInterfaceDesc(FrostySdk.Ebx.UIWorldMapIconInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

