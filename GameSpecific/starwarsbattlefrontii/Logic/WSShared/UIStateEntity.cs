using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStateEntityData))]
	public class UIStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIStateEntityData>
	{
		public new FrostySdk.Ebx.UIStateEntityData Data => data as FrostySdk.Ebx.UIStateEntityData;
		public override string DisplayName => "UIState";

		public UIStateEntity(FrostySdk.Ebx.UIStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

