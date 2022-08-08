using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FreeLookControllerData))]
	public class FreeLookController : CinebotController, IEntityData<FrostySdk.Ebx.FreeLookControllerData>
	{
		public new FrostySdk.Ebx.FreeLookControllerData Data => data as FrostySdk.Ebx.FreeLookControllerData;
		public override string DisplayName => "FreeLookController";

		public FreeLookController(FrostySdk.Ebx.FreeLookControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

