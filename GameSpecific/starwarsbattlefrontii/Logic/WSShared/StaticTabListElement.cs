using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticTabListElementData))]
	public class StaticTabListElement : TabListElement, IEntityData<FrostySdk.Ebx.StaticTabListElementData>
	{
		public new FrostySdk.Ebx.StaticTabListElementData Data => data as FrostySdk.Ebx.StaticTabListElementData;
		public override string DisplayName => "StaticTabListElement";

		public StaticTabListElement(FrostySdk.Ebx.StaticTabListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

