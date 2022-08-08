using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TacticalGroupQueryEntityData))]
	public class TacticalGroupQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TacticalGroupQueryEntityData>
	{
		public new FrostySdk.Ebx.TacticalGroupQueryEntityData Data => data as FrostySdk.Ebx.TacticalGroupQueryEntityData;
		public override string DisplayName => "TacticalGroupQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TacticalGroupQueryEntity(FrostySdk.Ebx.TacticalGroupQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

