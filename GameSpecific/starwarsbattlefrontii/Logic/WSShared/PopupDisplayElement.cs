using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PopupDisplayElementData))]
	public class PopupDisplayElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.PopupDisplayElementData>
	{
		public new FrostySdk.Ebx.PopupDisplayElementData Data => data as FrostySdk.Ebx.PopupDisplayElementData;
		public override string DisplayName => "PopupDisplayElement";

		public PopupDisplayElement(FrostySdk.Ebx.PopupDisplayElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

