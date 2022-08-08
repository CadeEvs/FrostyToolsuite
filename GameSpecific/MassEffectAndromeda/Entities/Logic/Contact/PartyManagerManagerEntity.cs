using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyManagerManagerEntityData))]
	public class PartyManagerManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PartyManagerManagerEntityData>
	{
		public new FrostySdk.Ebx.PartyManagerManagerEntityData Data => data as FrostySdk.Ebx.PartyManagerManagerEntityData;
		public override string DisplayName => "PartyManagerManager";

		public PartyManagerManagerEntity(FrostySdk.Ebx.PartyManagerManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

