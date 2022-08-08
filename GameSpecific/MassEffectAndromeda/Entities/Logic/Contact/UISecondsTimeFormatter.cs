using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISecondsTimeFormatterData))]
	public class UISecondsTimeFormatter : LogicEntity, IEntityData<FrostySdk.Ebx.UISecondsTimeFormatterData>
	{
		public new FrostySdk.Ebx.UISecondsTimeFormatterData Data => data as FrostySdk.Ebx.UISecondsTimeFormatterData;
		public override string DisplayName => "UISecondsTimeFormatter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UISecondsTimeFormatter(FrostySdk.Ebx.UISecondsTimeFormatterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

