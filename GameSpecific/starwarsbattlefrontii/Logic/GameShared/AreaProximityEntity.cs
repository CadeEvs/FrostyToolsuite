using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaProximityEntityData))]
	public class AreaProximityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AreaProximityEntityData>
	{
		public new FrostySdk.Ebx.AreaProximityEntityData Data => data as FrostySdk.Ebx.AreaProximityEntityData;
		public override string DisplayName => "AreaProximity";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AreaProximityEntity(FrostySdk.Ebx.AreaProximityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

