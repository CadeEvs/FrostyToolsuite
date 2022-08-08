using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStrikeTeamsDataProviderData))]
	public class MEStrikeTeamsDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.MEStrikeTeamsDataProviderData>
	{
		public new FrostySdk.Ebx.MEStrikeTeamsDataProviderData Data => data as FrostySdk.Ebx.MEStrikeTeamsDataProviderData;
		public override string DisplayName => "MEStrikeTeamsDataProvider";

		public MEStrikeTeamsDataProvider(FrostySdk.Ebx.MEStrikeTeamsDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

