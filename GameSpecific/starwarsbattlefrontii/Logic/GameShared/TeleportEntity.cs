using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeleportEntityData))]
	public class TeleportEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeleportEntityData>
	{
		public new FrostySdk.Ebx.TeleportEntityData Data => data as FrostySdk.Ebx.TeleportEntityData;
		public override string DisplayName => "Teleport";

		public TeleportEntity(FrostySdk.Ebx.TeleportEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

