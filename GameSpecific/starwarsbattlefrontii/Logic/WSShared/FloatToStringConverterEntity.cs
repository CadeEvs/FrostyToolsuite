using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatToStringConverterEntityData))]
	public class FloatToStringConverterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatToStringConverterEntityData>
	{
		public new FrostySdk.Ebx.FloatToStringConverterEntityData Data => data as FrostySdk.Ebx.FloatToStringConverterEntityData;
		public override string DisplayName => "FloatToStringConverter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatToStringConverterEntity(FrostySdk.Ebx.FloatToStringConverterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

