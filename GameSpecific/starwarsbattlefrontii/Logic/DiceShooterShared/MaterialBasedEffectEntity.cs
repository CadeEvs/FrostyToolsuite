using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MaterialBasedEffectEntityData))]
	public class MaterialBasedEffectEntity : DiceVFXEntityBase, IEntityData<FrostySdk.Ebx.MaterialBasedEffectEntityData>
	{
		public new FrostySdk.Ebx.MaterialBasedEffectEntityData Data => data as FrostySdk.Ebx.MaterialBasedEffectEntityData;
		public override string DisplayName => "MaterialBasedEffect";

		public MaterialBasedEffectEntity(FrostySdk.Ebx.MaterialBasedEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

