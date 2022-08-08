using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICreditsRollElementEntityData))]
	public class UICreditsRollElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.UICreditsRollElementEntityData>
	{
		public new FrostySdk.Ebx.UICreditsRollElementEntityData Data => data as FrostySdk.Ebx.UICreditsRollElementEntityData;
		public override string DisplayName => "UICreditsRollElement";

		public UICreditsRollElementEntity(FrostySdk.Ebx.UICreditsRollElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

