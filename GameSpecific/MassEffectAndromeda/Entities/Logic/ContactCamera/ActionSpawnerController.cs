using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActionSpawnerControllerData))]
	public class ActionSpawnerController : LogicController, IEntityData<FrostySdk.Ebx.ActionSpawnerControllerData>
	{
		public new FrostySdk.Ebx.ActionSpawnerControllerData Data => data as FrostySdk.Ebx.ActionSpawnerControllerData;
		public override string DisplayName => "ActionSpawnerController";

		public ActionSpawnerController(FrostySdk.Ebx.ActionSpawnerControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

