using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BasicControllerData))]
	public class BasicController : CinebotController, IEntityData<FrostySdk.Ebx.BasicControllerData>
	{
		public new FrostySdk.Ebx.BasicControllerData Data => data as FrostySdk.Ebx.BasicControllerData;
		public override string DisplayName => "BasicController";

		public BasicController(FrostySdk.Ebx.BasicControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

