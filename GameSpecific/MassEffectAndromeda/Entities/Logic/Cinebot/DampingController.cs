using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DampingControllerData))]
	public class DampingController : CinebotController, IEntityData<FrostySdk.Ebx.DampingControllerData>
	{
		public new FrostySdk.Ebx.DampingControllerData Data => data as FrostySdk.Ebx.DampingControllerData;
		public override string DisplayName => "DampingController";

		public DampingController(FrostySdk.Ebx.DampingControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

