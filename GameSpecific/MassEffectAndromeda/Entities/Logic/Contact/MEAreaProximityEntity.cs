using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAreaProximityEntityData))]
	public class MEAreaProximityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEAreaProximityEntityData>
	{
		public new FrostySdk.Ebx.MEAreaProximityEntityData Data => data as FrostySdk.Ebx.MEAreaProximityEntityData;
		public override string DisplayName => "MEAreaProximity";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MEAreaProximityEntity(FrostySdk.Ebx.MEAreaProximityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

