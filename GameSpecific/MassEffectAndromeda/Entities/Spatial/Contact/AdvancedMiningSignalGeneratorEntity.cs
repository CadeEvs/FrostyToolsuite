using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AdvancedMiningSignalGeneratorEntityData))]
	public class AdvancedMiningSignalGeneratorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AdvancedMiningSignalGeneratorEntityData>
	{
		public new FrostySdk.Ebx.AdvancedMiningSignalGeneratorEntityData Data => data as FrostySdk.Ebx.AdvancedMiningSignalGeneratorEntityData;

		public AdvancedMiningSignalGeneratorEntity(FrostySdk.Ebx.AdvancedMiningSignalGeneratorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

