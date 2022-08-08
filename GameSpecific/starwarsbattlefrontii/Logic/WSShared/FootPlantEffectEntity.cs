using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FootPlantEffectEntityData))]
	public class FootPlantEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FootPlantEffectEntityData>
	{
		public new FrostySdk.Ebx.FootPlantEffectEntityData Data => data as FrostySdk.Ebx.FootPlantEffectEntityData;
		public override string DisplayName => "FootPlantEffect";

		public FootPlantEffectEntity(FrostySdk.Ebx.FootPlantEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

