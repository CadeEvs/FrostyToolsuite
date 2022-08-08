using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CloakingModifierEntityData))]
	public class CloakingModifierEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.CloakingModifierEntityData>
	{
		public new FrostySdk.Ebx.CloakingModifierEntityData Data => data as FrostySdk.Ebx.CloakingModifierEntityData;
		public override string DisplayName => "CloakingModifier";

		public CloakingModifierEntity(FrostySdk.Ebx.CloakingModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

