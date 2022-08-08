using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISoundAreaEntityData))]
	public class AISoundAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AISoundAreaEntityData>
	{
		public new FrostySdk.Ebx.AISoundAreaEntityData Data => data as FrostySdk.Ebx.AISoundAreaEntityData;
		public override string DisplayName => "AISoundArea";

		public AISoundAreaEntity(FrostySdk.Ebx.AISoundAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

