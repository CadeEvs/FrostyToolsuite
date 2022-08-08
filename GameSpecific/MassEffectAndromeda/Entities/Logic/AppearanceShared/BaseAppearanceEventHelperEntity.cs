using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BaseAppearanceEventHelperEntityData))]
	public class BaseAppearanceEventHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BaseAppearanceEventHelperEntityData>
	{
		public new FrostySdk.Ebx.BaseAppearanceEventHelperEntityData Data => data as FrostySdk.Ebx.BaseAppearanceEventHelperEntityData;
		public override string DisplayName => "BaseAppearanceEventHelper";

		public BaseAppearanceEventHelperEntity(FrostySdk.Ebx.BaseAppearanceEventHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

