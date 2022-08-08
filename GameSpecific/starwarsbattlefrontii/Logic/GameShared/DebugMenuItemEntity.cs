using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugMenuItemEntityData))]
	public class DebugMenuItemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DebugMenuItemEntityData>
	{
		public new FrostySdk.Ebx.DebugMenuItemEntityData Data => data as FrostySdk.Ebx.DebugMenuItemEntityData;
		public override string DisplayName => "DebugMenuItem";

		public DebugMenuItemEntity(FrostySdk.Ebx.DebugMenuItemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

