using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEventsListElementBaseData))]
	public class GameEventsListElementBase : WSUIElementEntity, IEntityData<FrostySdk.Ebx.GameEventsListElementBaseData>
	{
		public new FrostySdk.Ebx.GameEventsListElementBaseData Data => data as FrostySdk.Ebx.GameEventsListElementBaseData;
		public override string DisplayName => "GameEventsListElementBase";

		public GameEventsListElementBase(FrostySdk.Ebx.GameEventsListElementBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

