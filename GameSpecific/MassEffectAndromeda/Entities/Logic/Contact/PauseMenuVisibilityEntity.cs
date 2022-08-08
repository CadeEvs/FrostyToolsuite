using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PauseMenuVisibilityEntityData))]
	public class PauseMenuVisibilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PauseMenuVisibilityEntityData>
	{
		public new FrostySdk.Ebx.PauseMenuVisibilityEntityData Data => data as FrostySdk.Ebx.PauseMenuVisibilityEntityData;
		public override string DisplayName => "PauseMenuVisibility";

		public PauseMenuVisibilityEntity(FrostySdk.Ebx.PauseMenuVisibilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

