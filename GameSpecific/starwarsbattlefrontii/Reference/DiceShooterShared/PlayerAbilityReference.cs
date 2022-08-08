using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityReferenceData))]
	public class PlayerAbilityReference : LogicReferenceObject, IEntityData<FrostySdk.Ebx.PlayerAbilityReferenceData>
	{
		public new FrostySdk.Ebx.PlayerAbilityReferenceData Data => data as FrostySdk.Ebx.PlayerAbilityReferenceData;

		public PlayerAbilityReference(FrostySdk.Ebx.PlayerAbilityReferenceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

