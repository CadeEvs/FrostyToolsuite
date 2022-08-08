using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PrintAppearanceDebugTextEntityData))]
	public class PrintAppearanceDebugTextEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PrintAppearanceDebugTextEntityData>
	{
		public new FrostySdk.Ebx.PrintAppearanceDebugTextEntityData Data => data as FrostySdk.Ebx.PrintAppearanceDebugTextEntityData;
		public override string DisplayName => "PrintAppearanceDebugText";

		public PrintAppearanceDebugTextEntity(FrostySdk.Ebx.PrintAppearanceDebugTextEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

