using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticEnlightenEntityData))]
	public class StaticEnlightenEntity : EnlightenEntity, IEntityData<FrostySdk.Ebx.StaticEnlightenEntityData>
	{
		public new FrostySdk.Ebx.StaticEnlightenEntityData Data => data as FrostySdk.Ebx.StaticEnlightenEntityData;

		public StaticEnlightenEntity(FrostySdk.Ebx.StaticEnlightenEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

