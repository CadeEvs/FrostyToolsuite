using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FirstPartyGamerCardEntityData))]
	public class FirstPartyGamerCardEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FirstPartyGamerCardEntityData>
	{
		public new FrostySdk.Ebx.FirstPartyGamerCardEntityData Data => data as FrostySdk.Ebx.FirstPartyGamerCardEntityData;
		public override string DisplayName => "FirstPartyGamerCard";

		public FirstPartyGamerCardEntity(FrostySdk.Ebx.FirstPartyGamerCardEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

