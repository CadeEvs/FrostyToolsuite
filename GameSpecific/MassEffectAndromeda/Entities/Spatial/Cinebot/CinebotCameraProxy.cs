using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotCameraProxyData))]
	public class CinebotCameraProxy : GameComponentEntity, IEntityData<FrostySdk.Ebx.CinebotCameraProxyData>
	{
		public new FrostySdk.Ebx.CinebotCameraProxyData Data => data as FrostySdk.Ebx.CinebotCameraProxyData;

		public CinebotCameraProxy(FrostySdk.Ebx.CinebotCameraProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

