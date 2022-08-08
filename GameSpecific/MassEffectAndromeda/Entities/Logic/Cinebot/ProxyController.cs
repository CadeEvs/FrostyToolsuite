using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyControllerData))]
	public class ProxyController : CinebotController, IEntityData<FrostySdk.Ebx.ProxyControllerData>
	{
		public new FrostySdk.Ebx.ProxyControllerData Data => data as FrostySdk.Ebx.ProxyControllerData;
		public override string DisplayName => "ProxyController";

		public ProxyController(FrostySdk.Ebx.ProxyControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

