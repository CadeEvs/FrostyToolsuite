using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShaderParameterEntityData))]
	public class ShaderParameterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShaderParameterEntityData>
	{
		public new FrostySdk.Ebx.ShaderParameterEntityData Data => data as FrostySdk.Ebx.ShaderParameterEntityData;
		public override string DisplayName => "ShaderParameter";
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                string paramName = "";

                switch (Data.ParameterType)
                {
                    case FrostySdk.Ebx.ShaderParameterDataType.ShaderParameterDataType_Bool: paramName = "BoolParam"; break;
                    case FrostySdk.Ebx.ShaderParameterDataType.ShaderParameterDataType_Permutation: paramName = "PermutationParam"; break;
                    case FrostySdk.Ebx.ShaderParameterDataType.ShaderParameterDataType_Texture: paramName = "TextureParam"; break;
                    case FrostySdk.Ebx.ShaderParameterDataType.ShaderParameterDataType_Vector: paramName = "VecParam"; break;
                }

                outProperties.Add(new ConnectionDesc() { Name = paramName, Direction = Direction.In });
                return outProperties;
            }
        }

        public ShaderParameterEntity(FrostySdk.Ebx.ShaderParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

