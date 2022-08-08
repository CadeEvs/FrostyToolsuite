using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadCommandListenerEntityData))]
	public class SquadCommandListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadCommandListenerEntityData>
	{
		public new FrostySdk.Ebx.SquadCommandListenerEntityData Data => data as FrostySdk.Ebx.SquadCommandListenerEntityData;
		public override string DisplayName => "SquadCommandListener";

		public SquadCommandListenerEntity(FrostySdk.Ebx.SquadCommandListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

