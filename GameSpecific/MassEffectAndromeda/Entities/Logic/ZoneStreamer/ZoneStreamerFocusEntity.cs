using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerFocusEntityData))]
	public class ZoneStreamerFocusEntity : ZoneStreamerLogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerFocusEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerFocusEntityData Data => data as FrostySdk.Ebx.ZoneStreamerFocusEntityData;
		public override string DisplayName => "ZoneStreamerFocus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ZoneStreamerFocusEntity(FrostySdk.Ebx.ZoneStreamerFocusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

