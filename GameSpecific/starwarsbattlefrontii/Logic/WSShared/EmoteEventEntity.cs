using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmoteEventEntityData))]
	public class EmoteEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EmoteEventEntityData>
	{
		public new FrostySdk.Ebx.EmoteEventEntityData Data => data as FrostySdk.Ebx.EmoteEventEntityData;
		public override string DisplayName => "EmoteEvent";

		public EmoteEventEntity(FrostySdk.Ebx.EmoteEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

