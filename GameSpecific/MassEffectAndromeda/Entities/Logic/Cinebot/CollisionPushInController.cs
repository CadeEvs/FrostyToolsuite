using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CollisionPushInControllerData))]
	public class CollisionPushInController : CinebotController, IEntityData<FrostySdk.Ebx.CollisionPushInControllerData>
	{
		public new FrostySdk.Ebx.CollisionPushInControllerData Data => data as FrostySdk.Ebx.CollisionPushInControllerData;
		public override string DisplayName => "CollisionPushInController";

		public CollisionPushInController(FrostySdk.Ebx.CollisionPushInControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

