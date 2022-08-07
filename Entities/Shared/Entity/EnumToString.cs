using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumToStringData))]
	public class EnumToString : ImpliedEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.EnumToStringData>
	{
		public new FrostySdk.Ebx.EnumToStringData Data => data as FrostySdk.Ebx.EnumToStringData;
		public override string DisplayName => "EnumToString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumToString(FrostySdk.Ebx.EnumToStringData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

