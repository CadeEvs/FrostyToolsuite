using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumConversionEntityData))]
	public class EnumConversionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EnumConversionEntityData>
	{
		public new FrostySdk.Ebx.EnumConversionEntityData Data => data as FrostySdk.Ebx.EnumConversionEntityData;
		public override string DisplayName => "EnumConversion";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumConversionEntity(FrostySdk.Ebx.EnumConversionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

