using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OptionsListElementData))]
	public class OptionsListElement : ListElement, IEntityData<FrostySdk.Ebx.OptionsListElementData>
	{
		public new FrostySdk.Ebx.OptionsListElementData Data => data as FrostySdk.Ebx.OptionsListElementData;
		public override string DisplayName => "OptionsListElement";

		public OptionsListElement(FrostySdk.Ebx.OptionsListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

