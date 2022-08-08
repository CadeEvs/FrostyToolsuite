using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShaderParameterEntityData))]
	public class ShaderParameterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShaderParameterEntityData>
	{
		public new FrostySdk.Ebx.ShaderParameterEntityData Data => data as FrostySdk.Ebx.ShaderParameterEntityData;
		public override string DisplayName => "ShaderParameter";

		public ShaderParameterEntity(FrostySdk.Ebx.ShaderParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

