using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlockSpottingEntityData))]
	public class BlockSpottingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlockSpottingEntityData>
	{
		public new FrostySdk.Ebx.BlockSpottingEntityData Data => data as FrostySdk.Ebx.BlockSpottingEntityData;
		public override string DisplayName => "BlockSpotting";

		public BlockSpottingEntity(FrostySdk.Ebx.BlockSpottingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

