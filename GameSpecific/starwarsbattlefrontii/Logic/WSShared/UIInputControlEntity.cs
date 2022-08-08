using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIInputControlEntityData))]
	public class UIInputControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIInputControlEntityData>
	{
		public new FrostySdk.Ebx.UIInputControlEntityData Data => data as FrostySdk.Ebx.UIInputControlEntityData;
		public override string DisplayName => "UIInputControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIInputControlEntity(FrostySdk.Ebx.UIInputControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

