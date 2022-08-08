using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CollisionControllerData))]
	public class CollisionController : LogicController, IEntityData<FrostySdk.Ebx.CollisionControllerData>
	{
		public new FrostySdk.Ebx.CollisionControllerData Data => data as FrostySdk.Ebx.CollisionControllerData;
		public override string DisplayName => "CollisionController";

		public CollisionController(FrostySdk.Ebx.CollisionControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

