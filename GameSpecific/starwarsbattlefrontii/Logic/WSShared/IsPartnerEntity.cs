using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsPartnerEntityData))]
	public class IsPartnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsPartnerEntityData>
	{
		public new FrostySdk.Ebx.IsPartnerEntityData Data => data as FrostySdk.Ebx.IsPartnerEntityData;
		public override string DisplayName => "IsPartner";

		public IsPartnerEntity(FrostySdk.Ebx.IsPartnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

