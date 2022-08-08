using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIInputEntityData))]
	public class UIInputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIInputEntityData>
	{
		public new FrostySdk.Ebx.UIInputEntityData Data => data as FrostySdk.Ebx.UIInputEntityData;
		public override string DisplayName => "UIInput";

		public UIInputEntity(FrostySdk.Ebx.UIInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

