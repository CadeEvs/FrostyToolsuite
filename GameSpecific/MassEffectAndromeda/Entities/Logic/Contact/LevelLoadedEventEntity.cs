using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LevelLoadedEventEntityData))]
	public class LevelLoadedEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LevelLoadedEventEntityData>
	{
		public new FrostySdk.Ebx.LevelLoadedEventEntityData Data => data as FrostySdk.Ebx.LevelLoadedEventEntityData;
		public override string DisplayName => "LevelLoadedEvent";

		public LevelLoadedEventEntity(FrostySdk.Ebx.LevelLoadedEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

