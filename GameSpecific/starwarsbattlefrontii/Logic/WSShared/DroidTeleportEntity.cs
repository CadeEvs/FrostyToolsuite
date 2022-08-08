using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidTeleportEntityData))]
	public class DroidTeleportEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DroidTeleportEntityData>
	{
		public new FrostySdk.Ebx.DroidTeleportEntityData Data => data as FrostySdk.Ebx.DroidTeleportEntityData;
		public override string DisplayName => "DroidTeleport";

		public DroidTeleportEntity(FrostySdk.Ebx.DroidTeleportEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

