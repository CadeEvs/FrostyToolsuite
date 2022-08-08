using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathLinkEntityData))]
	public class PathLinkEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PathLinkEntityData>
	{
		public new FrostySdk.Ebx.PathLinkEntityData Data => data as FrostySdk.Ebx.PathLinkEntityData;

		public PathLinkEntity(FrostySdk.Ebx.PathLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

