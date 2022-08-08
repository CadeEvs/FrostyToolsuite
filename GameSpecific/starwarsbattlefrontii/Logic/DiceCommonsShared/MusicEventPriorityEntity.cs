using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MusicEventPriorityEntityData))]
	public class MusicEventPriorityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MusicEventPriorityEntityData>
	{
		public new FrostySdk.Ebx.MusicEventPriorityEntityData Data => data as FrostySdk.Ebx.MusicEventPriorityEntityData;
		public override string DisplayName => "MusicEventPriority";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MusicEventPriorityEntity(FrostySdk.Ebx.MusicEventPriorityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

