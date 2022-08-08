using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlockUIInputData))]
	public class BlockUIInput : LogicEntity, IEntityData<FrostySdk.Ebx.BlockUIInputData>
	{
		public new FrostySdk.Ebx.BlockUIInputData Data => data as FrostySdk.Ebx.BlockUIInputData;
		public override string DisplayName => "BlockUIInput";

		public BlockUIInput(FrostySdk.Ebx.BlockUIInputData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

