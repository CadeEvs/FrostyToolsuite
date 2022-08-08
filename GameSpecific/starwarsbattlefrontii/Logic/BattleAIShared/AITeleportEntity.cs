using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITeleportEntityData))]
	public class AITeleportEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AITeleportEntityData>
	{
		public new FrostySdk.Ebx.AITeleportEntityData Data => data as FrostySdk.Ebx.AITeleportEntityData;
		public override string DisplayName => "AITeleport";

		public AITeleportEntity(FrostySdk.Ebx.AITeleportEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

