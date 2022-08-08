using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreditsListElementData))]
	public class CreditsListElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.CreditsListElementData>
	{
		public new FrostySdk.Ebx.CreditsListElementData Data => data as FrostySdk.Ebx.CreditsListElementData;
		public override string DisplayName => "CreditsListElement";

		public CreditsListElement(FrostySdk.Ebx.CreditsListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

