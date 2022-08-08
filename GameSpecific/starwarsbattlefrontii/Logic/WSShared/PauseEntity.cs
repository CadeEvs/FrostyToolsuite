using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PauseEntityData))]
	public class PauseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PauseEntityData>
	{
		public new FrostySdk.Ebx.PauseEntityData Data => data as FrostySdk.Ebx.PauseEntityData;
		public override string DisplayName => "Pause";

		public PauseEntity(FrostySdk.Ebx.PauseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

