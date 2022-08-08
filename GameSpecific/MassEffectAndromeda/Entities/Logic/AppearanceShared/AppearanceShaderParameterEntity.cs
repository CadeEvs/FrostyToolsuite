using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceShaderParameterEntityData))]
	public class AppearanceShaderParameterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AppearanceShaderParameterEntityData>
	{
		public new FrostySdk.Ebx.AppearanceShaderParameterEntityData Data => data as FrostySdk.Ebx.AppearanceShaderParameterEntityData;
		public override string DisplayName => "AppearanceShaderParameter";

		public AppearanceShaderParameterEntity(FrostySdk.Ebx.AppearanceShaderParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

