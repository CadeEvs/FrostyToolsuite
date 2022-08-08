using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIErrorOutputEntityData))]
	public class UIErrorOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIErrorOutputEntityData>
	{
		public new FrostySdk.Ebx.UIErrorOutputEntityData Data => data as FrostySdk.Ebx.UIErrorOutputEntityData;
		public override string DisplayName => "UIErrorOutput";

		public UIErrorOutputEntity(FrostySdk.Ebx.UIErrorOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

