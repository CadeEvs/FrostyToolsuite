using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIDamageIndicatorInterfaceDescData))]
	public class UIDamageIndicatorInterfaceDesc : UIChildItemInterfaceDescriptorBase, IEntityData<FrostySdk.Ebx.UIDamageIndicatorInterfaceDescData>
	{
		public new FrostySdk.Ebx.UIDamageIndicatorInterfaceDescData Data => data as FrostySdk.Ebx.UIDamageIndicatorInterfaceDescData;
		public override string DisplayName => "UIDamageIndicatorInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIDamageIndicatorInterfaceDesc(FrostySdk.Ebx.UIDamageIndicatorInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

