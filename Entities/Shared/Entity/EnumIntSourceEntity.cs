using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumIntSourceEntityData))]
	public class EnumIntSourceEntity : ExplicitEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.EnumIntSourceEntityData>
	{
		public new FrostySdk.Ebx.EnumIntSourceEntityData Data => data as FrostySdk.Ebx.EnumIntSourceEntityData;
		public override string DisplayName => "EnumIntSource";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumIntSourceEntity(FrostySdk.Ebx.EnumIntSourceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

