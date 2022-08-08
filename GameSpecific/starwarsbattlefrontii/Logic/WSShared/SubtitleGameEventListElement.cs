using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubtitleGameEventListElementData))]
	public class SubtitleGameEventListElement : GameEventsListElementBase, IEntityData<FrostySdk.Ebx.SubtitleGameEventListElementData>
	{
		public new FrostySdk.Ebx.SubtitleGameEventListElementData Data => data as FrostySdk.Ebx.SubtitleGameEventListElementData;
		public override string DisplayName => "SubtitleGameEventListElement";

		public SubtitleGameEventListElement(FrostySdk.Ebx.SubtitleGameEventListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

