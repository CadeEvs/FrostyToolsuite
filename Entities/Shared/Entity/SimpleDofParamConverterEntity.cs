using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleDofParamConverterEntityData))]
	public class SimpleDofParamConverterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SimpleDofParamConverterEntityData>
	{
		public new FrostySdk.Ebx.SimpleDofParamConverterEntityData Data => data as FrostySdk.Ebx.SimpleDofParamConverterEntityData;
		public override string DisplayName => "SimpleDofParamConverter";

		public SimpleDofParamConverterEntity(FrostySdk.Ebx.SimpleDofParamConverterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

