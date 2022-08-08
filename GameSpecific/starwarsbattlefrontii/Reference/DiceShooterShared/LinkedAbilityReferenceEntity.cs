using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinkedAbilityReferenceEntityData))]
	public class LinkedAbilityReferenceEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.LinkedAbilityReferenceEntityData>
	{
		public new FrostySdk.Ebx.LinkedAbilityReferenceEntityData Data => data as FrostySdk.Ebx.LinkedAbilityReferenceEntityData;

		public LinkedAbilityReferenceEntity(FrostySdk.Ebx.LinkedAbilityReferenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

