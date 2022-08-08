using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStatusUpdateEntityData))]
	public class UIStatusUpdateEntity : UIChildItemSpawnerWidgetEntity, IEntityData<FrostySdk.Ebx.UIStatusUpdateEntityData>
	{
		public new FrostySdk.Ebx.UIStatusUpdateEntityData Data => data as FrostySdk.Ebx.UIStatusUpdateEntityData;
		public override string DisplayName => "UIStatusUpdate";

		public UIStatusUpdateEntity(FrostySdk.Ebx.UIStatusUpdateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

