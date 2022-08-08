using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientClassExperiencePercentageData))]
	public class ClientClassExperiencePercentage : LogicEntity, IEntityData<FrostySdk.Ebx.ClientClassExperiencePercentageData>
	{
		public new FrostySdk.Ebx.ClientClassExperiencePercentageData Data => data as FrostySdk.Ebx.ClientClassExperiencePercentageData;
		public override string DisplayName => "ClientClassExperiencePercentage";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientClassExperiencePercentage(FrostySdk.Ebx.ClientClassExperiencePercentageData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

