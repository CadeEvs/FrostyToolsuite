using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpriteDofParamConverterEntityData))]
	public class SpriteDofParamConverterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpriteDofParamConverterEntityData>
	{
		public new FrostySdk.Ebx.SpriteDofParamConverterEntityData Data => data as FrostySdk.Ebx.SpriteDofParamConverterEntityData;
		public override string DisplayName => "SpriteDofParamConverter";

		public SpriteDofParamConverterEntity(FrostySdk.Ebx.SpriteDofParamConverterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

