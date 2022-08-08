using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicRenderTextureControllerData))]
	public class CinematicRenderTextureController : CinebotController, IEntityData<FrostySdk.Ebx.CinematicRenderTextureControllerData>
	{
		public new FrostySdk.Ebx.CinematicRenderTextureControllerData Data => data as FrostySdk.Ebx.CinematicRenderTextureControllerData;
		public override string DisplayName => "CinematicRenderTextureController";

		public CinematicRenderTextureController(FrostySdk.Ebx.CinematicRenderTextureControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

