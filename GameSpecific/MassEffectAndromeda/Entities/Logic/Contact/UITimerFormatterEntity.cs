using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UITimerFormatterEntityData))]
	public class UITimerFormatterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UITimerFormatterEntityData>
	{
		public new FrostySdk.Ebx.UITimerFormatterEntityData Data => data as FrostySdk.Ebx.UITimerFormatterEntityData;
		public override string DisplayName => "UITimerFormatter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UITimerFormatterEntity(FrostySdk.Ebx.UITimerFormatterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

