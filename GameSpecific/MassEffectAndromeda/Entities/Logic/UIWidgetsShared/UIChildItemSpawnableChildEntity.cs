using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIChildItemSpawnableChildEntityData))]
	public class UIChildItemSpawnableChildEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIChildItemSpawnableChildEntityData>
	{
		public new FrostySdk.Ebx.UIChildItemSpawnableChildEntityData Data => data as FrostySdk.Ebx.UIChildItemSpawnableChildEntityData;
		public override string DisplayName => "UIChildItemSpawnableChild";

		public UIChildItemSpawnableChildEntity(FrostySdk.Ebx.UIChildItemSpawnableChildEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

