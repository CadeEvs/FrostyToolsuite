using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIDynamicSpawnerWidgetEntityData))]
	public class UIDynamicSpawnerWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIDynamicSpawnerWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIDynamicSpawnerWidgetEntityData Data => data as FrostySdk.Ebx.UIDynamicSpawnerWidgetEntityData;
		public override string DisplayName => "UIDynamicSpawnerWidget";

		public UIDynamicSpawnerWidgetEntity(FrostySdk.Ebx.UIDynamicSpawnerWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

