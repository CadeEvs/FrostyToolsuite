using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerLogicEntityData))]
	public class ZoneStreamerLogicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerLogicEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerLogicEntityData Data => data as FrostySdk.Ebx.ZoneStreamerLogicEntityData;
		public override string DisplayName => "ZoneStreamerLogic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ZoneStreamerLogicEntity(FrostySdk.Ebx.ZoneStreamerLogicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

