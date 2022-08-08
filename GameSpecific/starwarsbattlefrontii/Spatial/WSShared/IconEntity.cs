using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IconEntityData))]
	public class IconEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.IconEntityData>
	{
		public new FrostySdk.Ebx.IconEntityData Data => data as FrostySdk.Ebx.IconEntityData;

		public IconEntity(FrostySdk.Ebx.IconEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

