using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEUIButtonLegendChildInterfaceDescData))]
	public class MEUIButtonLegendChildInterfaceDesc : UIButtonLegendChildInterfaceDesc, IEntityData<FrostySdk.Ebx.MEUIButtonLegendChildInterfaceDescData>
	{
		public new FrostySdk.Ebx.MEUIButtonLegendChildInterfaceDescData Data => data as FrostySdk.Ebx.MEUIButtonLegendChildInterfaceDescData;
		public override string DisplayName => "MEUIButtonLegendChildInterfaceDesc";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MEUIButtonLegendChildInterfaceDesc(FrostySdk.Ebx.MEUIButtonLegendChildInterfaceDescData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

