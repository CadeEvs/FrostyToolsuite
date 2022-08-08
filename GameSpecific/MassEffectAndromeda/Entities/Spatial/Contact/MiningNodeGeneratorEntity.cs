using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MiningNodeGeneratorEntityData))]
	public class MiningNodeGeneratorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.MiningNodeGeneratorEntityData>
	{
		public new FrostySdk.Ebx.MiningNodeGeneratorEntityData Data => data as FrostySdk.Ebx.MiningNodeGeneratorEntityData;

		public MiningNodeGeneratorEntity(FrostySdk.Ebx.MiningNodeGeneratorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

