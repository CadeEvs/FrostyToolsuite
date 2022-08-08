using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEventsFastListElementData))]
	public class GameEventsFastListElement : GameEventsListElementBase, IEntityData<FrostySdk.Ebx.GameEventsFastListElementData>
	{
		public new FrostySdk.Ebx.GameEventsFastListElementData Data => data as FrostySdk.Ebx.GameEventsFastListElementData;
		public override string DisplayName => "GameEventsFastListElement";

		public GameEventsFastListElement(FrostySdk.Ebx.GameEventsFastListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

