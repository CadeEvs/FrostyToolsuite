using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InvestigateSettingsOverrideData))]
	public class InvestigateSettingsOverride : LogicEntity, IEntityData<FrostySdk.Ebx.InvestigateSettingsOverrideData>
	{
		public new FrostySdk.Ebx.InvestigateSettingsOverrideData Data => data as FrostySdk.Ebx.InvestigateSettingsOverrideData;
		public override string DisplayName => "InvestigateSettingsOverride";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InvestigateSettingsOverride(FrostySdk.Ebx.InvestigateSettingsOverrideData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

