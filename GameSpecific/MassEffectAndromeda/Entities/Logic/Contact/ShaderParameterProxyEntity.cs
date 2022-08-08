using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShaderParameterProxyEntityData))]
	public class ShaderParameterProxyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShaderParameterProxyEntityData>
	{
		public new FrostySdk.Ebx.ShaderParameterProxyEntityData Data => data as FrostySdk.Ebx.ShaderParameterProxyEntityData;
		public override string DisplayName => "ShaderParameterProxy";

		public ShaderParameterProxyEntity(FrostySdk.Ebx.ShaderParameterProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

