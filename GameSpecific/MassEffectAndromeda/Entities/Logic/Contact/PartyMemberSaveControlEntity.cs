using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberSaveControlEntityData))]
	public class PartyMemberSaveControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PartyMemberSaveControlEntityData>
	{
		public new FrostySdk.Ebx.PartyMemberSaveControlEntityData Data => data as FrostySdk.Ebx.PartyMemberSaveControlEntityData;
		public override string DisplayName => "PartyMemberSaveControl";

		public PartyMemberSaveControlEntity(FrostySdk.Ebx.PartyMemberSaveControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

