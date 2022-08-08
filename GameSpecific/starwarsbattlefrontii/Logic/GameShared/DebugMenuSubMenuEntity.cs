using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugMenuSubMenuEntityData))]
	public class DebugMenuSubMenuEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DebugMenuSubMenuEntityData>
	{
		public new FrostySdk.Ebx.DebugMenuSubMenuEntityData Data => data as FrostySdk.Ebx.DebugMenuSubMenuEntityData;
		public override string DisplayName => "DebugMenuSubMenu";

		public DebugMenuSubMenuEntity(FrostySdk.Ebx.DebugMenuSubMenuEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

