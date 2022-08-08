using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadQueryEntityData))]
	public class SquadQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadQueryEntityData>
	{
		public new FrostySdk.Ebx.SquadQueryEntityData Data => data as FrostySdk.Ebx.SquadQueryEntityData;
		public override string DisplayName => "SquadQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SquadQueryEntity(FrostySdk.Ebx.SquadQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

