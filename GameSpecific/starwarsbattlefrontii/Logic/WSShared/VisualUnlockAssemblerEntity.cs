using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualUnlockAssemblerEntityData))]
	public class VisualUnlockAssemblerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VisualUnlockAssemblerEntityData>
	{
		public new FrostySdk.Ebx.VisualUnlockAssemblerEntityData Data => data as FrostySdk.Ebx.VisualUnlockAssemblerEntityData;
		public override string DisplayName => "VisualUnlockAssembler";

		public VisualUnlockAssemblerEntity(FrostySdk.Ebx.VisualUnlockAssemblerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

