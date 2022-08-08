using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinkedPlayerAbilityReferenceEntityData))]
	public class LinkedPlayerAbilityReferenceEntity : PlayerAbilityReference, IEntityData<FrostySdk.Ebx.LinkedPlayerAbilityReferenceEntityData>
	{
		public new FrostySdk.Ebx.LinkedPlayerAbilityReferenceEntityData Data => data as FrostySdk.Ebx.LinkedPlayerAbilityReferenceEntityData;

		public LinkedPlayerAbilityReferenceEntity(FrostySdk.Ebx.LinkedPlayerAbilityReferenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

