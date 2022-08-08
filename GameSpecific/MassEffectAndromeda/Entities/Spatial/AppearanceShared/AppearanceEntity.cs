using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceEntityData))]
	public class AppearanceEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.AppearanceEntityData>
	{
		public new FrostySdk.Ebx.AppearanceEntityData Data => data as FrostySdk.Ebx.AppearanceEntityData;

		public AppearanceEntity(FrostySdk.Ebx.AppearanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

