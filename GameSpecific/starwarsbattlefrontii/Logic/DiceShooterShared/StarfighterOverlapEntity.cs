using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterOverlapEntityData))]
	public class StarfighterOverlapEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterOverlapEntityData>
	{
		public new FrostySdk.Ebx.StarfighterOverlapEntityData Data => data as FrostySdk.Ebx.StarfighterOverlapEntityData;
		public override string DisplayName => "StarfighterOverlap";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StarfighterOverlapEntity(FrostySdk.Ebx.StarfighterOverlapEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

