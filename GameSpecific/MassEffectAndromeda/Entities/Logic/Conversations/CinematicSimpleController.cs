using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicSimpleControllerData))]
	public class CinematicSimpleController : CinebotController, IEntityData<FrostySdk.Ebx.CinematicSimpleControllerData>
	{
		public new FrostySdk.Ebx.CinematicSimpleControllerData Data => data as FrostySdk.Ebx.CinematicSimpleControllerData;
		public override string DisplayName => "CinematicSimpleController";

		public CinematicSimpleController(FrostySdk.Ebx.CinematicSimpleControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

