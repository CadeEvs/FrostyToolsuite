using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStateManagerEntityData))]
	public class UIStateManagerEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.UIStateManagerEntityData>
	{
		public new FrostySdk.Ebx.UIStateManagerEntityData Data => data as FrostySdk.Ebx.UIStateManagerEntityData;
		public override string DisplayName => "UIStateManager";

		public UIStateManagerEntity(FrostySdk.Ebx.UIStateManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

