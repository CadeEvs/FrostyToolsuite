using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotModeMapEntityData))]
	public class CinebotModeMapEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.CinebotModeMapEntityData>
	{
		public new FrostySdk.Ebx.CinebotModeMapEntityData Data => data as FrostySdk.Ebx.CinebotModeMapEntityData;

		public CinebotModeMapEntity(FrostySdk.Ebx.CinebotModeMapEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

