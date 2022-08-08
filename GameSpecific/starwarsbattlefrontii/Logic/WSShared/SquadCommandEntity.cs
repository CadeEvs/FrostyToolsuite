using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadCommandEntityData))]
	public class SquadCommandEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadCommandEntityData>
	{
		public new FrostySdk.Ebx.SquadCommandEntityData Data => data as FrostySdk.Ebx.SquadCommandEntityData;
		public override string DisplayName => "SquadCommand";

		public SquadCommandEntity(FrostySdk.Ebx.SquadCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

