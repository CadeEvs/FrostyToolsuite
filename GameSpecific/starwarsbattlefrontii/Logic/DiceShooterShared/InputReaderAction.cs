using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputReaderActionData))]
	public class InputReaderAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.InputReaderActionData>
	{
		public new FrostySdk.Ebx.InputReaderActionData Data => data as FrostySdk.Ebx.InputReaderActionData;
		public override string DisplayName => "InputReaderAction";

		public InputReaderAction(FrostySdk.Ebx.InputReaderActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

