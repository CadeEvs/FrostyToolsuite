using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerTransitionEntityData))]
	public class ZoneStreamerTransitionEntity : ZoneStreamerLogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerTransitionEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerTransitionEntityData Data => data as FrostySdk.Ebx.ZoneStreamerTransitionEntityData;
		public override string DisplayName => "ZoneStreamerTransition";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ZoneStreamerTransitionEntity(FrostySdk.Ebx.ZoneStreamerTransitionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

