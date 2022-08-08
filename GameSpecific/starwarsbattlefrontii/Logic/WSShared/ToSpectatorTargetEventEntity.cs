using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ToSpectatorTargetEventEntityData))]
	public class ToSpectatorTargetEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ToSpectatorTargetEventEntityData>
	{
		public new FrostySdk.Ebx.ToSpectatorTargetEventEntityData Data => data as FrostySdk.Ebx.ToSpectatorTargetEventEntityData;
		public override string DisplayName => "ToSpectatorTargetEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ToSpectatorTargetEventEntity(FrostySdk.Ebx.ToSpectatorTargetEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

