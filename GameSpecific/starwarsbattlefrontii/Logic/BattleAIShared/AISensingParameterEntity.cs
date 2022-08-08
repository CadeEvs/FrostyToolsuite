using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISensingParameterEntityData))]
	public class AISensingParameterEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AISensingParameterEntityData>
	{
		public new FrostySdk.Ebx.AISensingParameterEntityData Data => data as FrostySdk.Ebx.AISensingParameterEntityData;
		public override string DisplayName => "AISensingParameter";

		public AISensingParameterEntity(FrostySdk.Ebx.AISensingParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

