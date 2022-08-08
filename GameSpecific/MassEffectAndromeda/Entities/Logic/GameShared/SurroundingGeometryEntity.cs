using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SurroundingGeometryEntityData))]
	public class SurroundingGeometryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SurroundingGeometryEntityData>
	{
		public new FrostySdk.Ebx.SurroundingGeometryEntityData Data => data as FrostySdk.Ebx.SurroundingGeometryEntityData;
		public override string DisplayName => "SurroundingGeometry";

		public SurroundingGeometryEntity(FrostySdk.Ebx.SurroundingGeometryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

