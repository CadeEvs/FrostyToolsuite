using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverShapeEntityData))]
	public class CoverShapeEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.CoverShapeEntityData>
	{
		public new FrostySdk.Ebx.CoverShapeEntityData Data => data as FrostySdk.Ebx.CoverShapeEntityData;

		public CoverShapeEntity(FrostySdk.Ebx.CoverShapeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

