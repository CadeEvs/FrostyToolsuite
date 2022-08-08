using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputActionSplitEntityData))]
	public class InputActionSplitEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputActionSplitEntityData>
	{
		public new FrostySdk.Ebx.InputActionSplitEntityData Data => data as FrostySdk.Ebx.InputActionSplitEntityData;
		public override string DisplayName => "InputActionSplit";

		public InputActionSplitEntity(FrostySdk.Ebx.InputActionSplitEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

