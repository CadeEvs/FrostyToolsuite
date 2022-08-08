using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpacerElementData))]
	public class SpacerElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.SpacerElementData>
	{
		public new FrostySdk.Ebx.SpacerElementData Data => data as FrostySdk.Ebx.SpacerElementData;
		public override string DisplayName => "SpacerElement";

		public SpacerElement(FrostySdk.Ebx.SpacerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

