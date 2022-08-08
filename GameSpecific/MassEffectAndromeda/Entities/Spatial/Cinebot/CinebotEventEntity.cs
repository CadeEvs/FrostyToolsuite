using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotEventEntityData))]
	public class CinebotEventEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.CinebotEventEntityData>
	{
		public new FrostySdk.Ebx.CinebotEventEntityData Data => data as FrostySdk.Ebx.CinebotEventEntityData;

		public CinebotEventEntity(FrostySdk.Ebx.CinebotEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

