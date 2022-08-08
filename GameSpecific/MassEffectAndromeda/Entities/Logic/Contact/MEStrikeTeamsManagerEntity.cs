using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStrikeTeamsManagerEntityData))]
	public class MEStrikeTeamsManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEStrikeTeamsManagerEntityData>
	{
		public new FrostySdk.Ebx.MEStrikeTeamsManagerEntityData Data => data as FrostySdk.Ebx.MEStrikeTeamsManagerEntityData;
		public override string DisplayName => "MEStrikeTeamsManager";

		public MEStrikeTeamsManagerEntity(FrostySdk.Ebx.MEStrikeTeamsManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

