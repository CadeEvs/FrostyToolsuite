using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ToPartnerEventEntityData))]
	public class ToPartnerEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ToPartnerEventEntityData>
	{
		public new FrostySdk.Ebx.ToPartnerEventEntityData Data => data as FrostySdk.Ebx.ToPartnerEventEntityData;
		public override string DisplayName => "ToPartnerEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ToPartnerEventEntity(FrostySdk.Ebx.ToPartnerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

