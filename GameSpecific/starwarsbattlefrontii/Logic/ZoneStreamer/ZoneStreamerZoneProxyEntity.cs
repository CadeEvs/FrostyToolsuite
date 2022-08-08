using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerZoneProxyEntityData))]
	public class ZoneStreamerZoneProxyEntity : ZoneStreamerLogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerZoneProxyEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerZoneProxyEntityData Data => data as FrostySdk.Ebx.ZoneStreamerZoneProxyEntityData;
		public override string DisplayName => "ZoneStreamerZoneProxy";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ZoneStreamerZoneProxyEntity(FrostySdk.Ebx.ZoneStreamerZoneProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

