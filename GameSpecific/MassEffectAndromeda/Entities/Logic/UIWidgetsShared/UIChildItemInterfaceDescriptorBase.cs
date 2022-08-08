using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIChildItemInterfaceDescriptorBaseData))]
	public class UIChildItemInterfaceDescriptorBase : LogicEntity, IEntityData<FrostySdk.Ebx.UIChildItemInterfaceDescriptorBaseData>
	{
		public new FrostySdk.Ebx.UIChildItemInterfaceDescriptorBaseData Data => data as FrostySdk.Ebx.UIChildItemInterfaceDescriptorBaseData;
		public override string DisplayName => "UIChildItemInterfaceDescriptorBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIChildItemInterfaceDescriptorBase(FrostySdk.Ebx.UIChildItemInterfaceDescriptorBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

