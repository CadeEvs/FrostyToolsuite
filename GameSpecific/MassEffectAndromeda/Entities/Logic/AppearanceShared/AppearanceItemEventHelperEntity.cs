using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceItemEventHelperEntityData))]
	public class AppearanceItemEventHelperEntity : BaseAppearanceEventHelperEntity, IEntityData<FrostySdk.Ebx.AppearanceItemEventHelperEntityData>
	{
		public new FrostySdk.Ebx.AppearanceItemEventHelperEntityData Data => data as FrostySdk.Ebx.AppearanceItemEventHelperEntityData;
		public override string DisplayName => "AppearanceItemEventHelper";

		public AppearanceItemEventHelperEntity(FrostySdk.Ebx.AppearanceItemEventHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

