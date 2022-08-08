using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MantleShapeEntityData))]
	public class MantleShapeEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.MantleShapeEntityData>
	{
		public new FrostySdk.Ebx.MantleShapeEntityData Data => data as FrostySdk.Ebx.MantleShapeEntityData;

		public MantleShapeEntity(FrostySdk.Ebx.MantleShapeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

