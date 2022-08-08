using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RenderTextureControllerData))]
	public class RenderTextureController : LogicController, IEntityData<FrostySdk.Ebx.RenderTextureControllerData>
	{
		public new FrostySdk.Ebx.RenderTextureControllerData Data => data as FrostySdk.Ebx.RenderTextureControllerData;
		public override string DisplayName => "RenderTextureController";

		public RenderTextureController(FrostySdk.Ebx.RenderTextureControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

