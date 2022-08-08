using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerControlEntityData))]
	public class ZoneStreamerControlEntity : ZoneStreamerLogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerControlEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerControlEntityData Data => data as FrostySdk.Ebx.ZoneStreamerControlEntityData;
		public override string DisplayName => "ZoneStreamerControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ZoneStreamerControlEntity(FrostySdk.Ebx.ZoneStreamerControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

