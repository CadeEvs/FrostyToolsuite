using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotStateEntityData))]
	public class CinebotStateEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.CinebotStateEntityData>
	{
		public new FrostySdk.Ebx.CinebotStateEntityData Data => data as FrostySdk.Ebx.CinebotStateEntityData;

		public CinebotStateEntity(FrostySdk.Ebx.CinebotStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

