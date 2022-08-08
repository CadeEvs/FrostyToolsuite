using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPartnerEntityData))]
	public class AutoPartnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoPartnerEntityData>
	{
		public new FrostySdk.Ebx.AutoPartnerEntityData Data => data as FrostySdk.Ebx.AutoPartnerEntityData;
		public override string DisplayName => "AutoPartner";

		public AutoPartnerEntity(FrostySdk.Ebx.AutoPartnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

