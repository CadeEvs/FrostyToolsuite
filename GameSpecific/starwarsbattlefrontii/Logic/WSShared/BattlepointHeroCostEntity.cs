using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BattlepointHeroCostEntityData))]
	public class BattlepointHeroCostEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BattlepointHeroCostEntityData>
	{
		public new FrostySdk.Ebx.BattlepointHeroCostEntityData Data => data as FrostySdk.Ebx.BattlepointHeroCostEntityData;
		public override string DisplayName => "BattlepointHeroCost";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BattlepointHeroCostEntity(FrostySdk.Ebx.BattlepointHeroCostEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

