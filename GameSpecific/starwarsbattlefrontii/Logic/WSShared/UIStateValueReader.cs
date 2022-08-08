using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStateValueReaderData))]
	public class UIStateValueReader : LogicEntity, IEntityData<FrostySdk.Ebx.UIStateValueReaderData>
	{
		public new FrostySdk.Ebx.UIStateValueReaderData Data => data as FrostySdk.Ebx.UIStateValueReaderData;
		public override string DisplayName => "UIStateValueReader";

		public UIStateValueReader(FrostySdk.Ebx.UIStateValueReaderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

