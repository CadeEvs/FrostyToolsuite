using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMissionResultChildInterfaceDescData))]
	public class UIMissionResultChildInterfaceDesc : UISelectorChildInterfaceDesc, IEntityData<FrostySdk.Ebx.UIMissionResultChildInterfaceDescData>
	{
		public new FrostySdk.Ebx.UIMissionResultChildInterfaceDescData Data => data as FrostySdk.Ebx.UIMissionResultChildInterfaceDescData;
		public override string DisplayName => "UIMissionResultChildInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIMissionResultChildInterfaceDesc(FrostySdk.Ebx.UIMissionResultChildInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

