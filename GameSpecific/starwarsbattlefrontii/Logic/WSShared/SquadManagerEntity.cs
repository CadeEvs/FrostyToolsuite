using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadManagerEntityData))]
	public class SquadManagerEntity : SpawnGroupManagerEntity, IEntityData<FrostySdk.Ebx.SquadManagerEntityData>
	{
		public new FrostySdk.Ebx.SquadManagerEntityData Data => data as FrostySdk.Ebx.SquadManagerEntityData;
		public override string DisplayName => "SquadManager";

		public SquadManagerEntity(FrostySdk.Ebx.SquadManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

