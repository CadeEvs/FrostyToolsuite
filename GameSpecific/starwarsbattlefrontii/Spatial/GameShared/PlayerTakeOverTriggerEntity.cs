using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerTakeOverTriggerEntityData))]
	public class PlayerTakeOverTriggerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PlayerTakeOverTriggerEntityData>
	{
		public new FrostySdk.Ebx.PlayerTakeOverTriggerEntityData Data => data as FrostySdk.Ebx.PlayerTakeOverTriggerEntityData;

		public PlayerTakeOverTriggerEntity(FrostySdk.Ebx.PlayerTakeOverTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

