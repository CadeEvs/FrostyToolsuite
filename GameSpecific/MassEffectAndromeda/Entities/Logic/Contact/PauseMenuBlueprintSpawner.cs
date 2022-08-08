using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PauseMenuBlueprintSpawnerData))]
	public class PauseMenuBlueprintSpawner : LogicEntity, IEntityData<FrostySdk.Ebx.PauseMenuBlueprintSpawnerData>
	{
		public new FrostySdk.Ebx.PauseMenuBlueprintSpawnerData Data => data as FrostySdk.Ebx.PauseMenuBlueprintSpawnerData;
		public override string DisplayName => "PauseMenuBlueprintSpawner";

		public PauseMenuBlueprintSpawner(FrostySdk.Ebx.PauseMenuBlueprintSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

