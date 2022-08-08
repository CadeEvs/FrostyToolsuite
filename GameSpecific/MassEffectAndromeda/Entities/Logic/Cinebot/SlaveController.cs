using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SlaveControllerData))]
	public class SlaveController : CinebotController, IEntityData<FrostySdk.Ebx.SlaveControllerData>
	{
		public new FrostySdk.Ebx.SlaveControllerData Data => data as FrostySdk.Ebx.SlaveControllerData;
		public override string DisplayName => "SlaveController";

		public SlaveController(FrostySdk.Ebx.SlaveControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

