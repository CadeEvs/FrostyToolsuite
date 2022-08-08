using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotLocatorEntityData))]
	public class CinebotLocatorEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.CinebotLocatorEntityData>
	{
		public new FrostySdk.Ebx.CinebotLocatorEntityData Data => data as FrostySdk.Ebx.CinebotLocatorEntityData;

		public CinebotLocatorEntity(FrostySdk.Ebx.CinebotLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

