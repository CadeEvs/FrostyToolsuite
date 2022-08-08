using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KillLogElementData))]
	public class KillLogElement : GameEventsListElementBase, IEntityData<FrostySdk.Ebx.KillLogElementData>
	{
		public new FrostySdk.Ebx.KillLogElementData Data => data as FrostySdk.Ebx.KillLogElementData;
		public override string DisplayName => "KillLogElement";

		public KillLogElement(FrostySdk.Ebx.KillLogElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

