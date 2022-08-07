using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumToIntData))]
	public class EnumToInt : ImpliedEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.EnumToIntData>
	{
		public new FrostySdk.Ebx.EnumToIntData Data => data as FrostySdk.Ebx.EnumToIntData;
		public override string DisplayName => "EnumToInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumToInt(FrostySdk.Ebx.EnumToIntData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

