using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPAIPlayerAbilityReferenceData))]
	public class SPAIPlayerAbilityReference : PlayerAbilityReference, IEntityData<FrostySdk.Ebx.SPAIPlayerAbilityReferenceData>
	{
		public new FrostySdk.Ebx.SPAIPlayerAbilityReferenceData Data => data as FrostySdk.Ebx.SPAIPlayerAbilityReferenceData;

		public SPAIPlayerAbilityReference(FrostySdk.Ebx.SPAIPlayerAbilityReferenceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

