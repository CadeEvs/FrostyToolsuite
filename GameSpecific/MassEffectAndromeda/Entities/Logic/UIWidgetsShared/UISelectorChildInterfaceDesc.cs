using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISelectorChildInterfaceDescData))]
	public class UISelectorChildInterfaceDesc : UIBaseSelectorChildInterfaceDesc, IEntityData<FrostySdk.Ebx.UISelectorChildInterfaceDescData>
	{
		public new FrostySdk.Ebx.UISelectorChildInterfaceDescData Data => data as FrostySdk.Ebx.UISelectorChildInterfaceDescData;
		public override string DisplayName => "UISelectorChildInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UISelectorChildInterfaceDesc(FrostySdk.Ebx.UISelectorChildInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

