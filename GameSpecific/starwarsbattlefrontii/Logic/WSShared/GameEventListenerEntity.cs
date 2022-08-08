using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEventListenerEntityData))]
	public class GameEventListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameEventListenerEntityData>
	{
		public new FrostySdk.Ebx.GameEventListenerEntityData Data => data as FrostySdk.Ebx.GameEventListenerEntityData;
		public override string DisplayName => "GameEventListener";

		public GameEventListenerEntity(FrostySdk.Ebx.GameEventListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

