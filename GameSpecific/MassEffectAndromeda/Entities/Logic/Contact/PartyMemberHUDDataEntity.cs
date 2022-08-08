using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberHUDDataEntityData))]
	public class PartyMemberHUDDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PartyMemberHUDDataEntityData>
	{
		public new FrostySdk.Ebx.PartyMemberHUDDataEntityData Data => data as FrostySdk.Ebx.PartyMemberHUDDataEntityData;
		public override string DisplayName => "PartyMemberHUDData";

		public PartyMemberHUDDataEntity(FrostySdk.Ebx.PartyMemberHUDDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

