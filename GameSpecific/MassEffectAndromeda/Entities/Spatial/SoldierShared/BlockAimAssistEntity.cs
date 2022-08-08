using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlockAimAssistEntityData))]
	public class BlockAimAssistEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.BlockAimAssistEntityData>
	{
		public new FrostySdk.Ebx.BlockAimAssistEntityData Data => data as FrostySdk.Ebx.BlockAimAssistEntityData;

		public BlockAimAssistEntity(FrostySdk.Ebx.BlockAimAssistEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

