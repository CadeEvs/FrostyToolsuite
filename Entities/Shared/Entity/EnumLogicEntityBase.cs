using FrostySdk;
using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumLogicEntityBaseData))]
	public class EnumLogicEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.EnumLogicEntityBaseData>
	{
		public new FrostySdk.Ebx.EnumLogicEntityBaseData Data => data as FrostySdk.Ebx.EnumLogicEntityBaseData;
		public override string DisplayName => "EnumLogicEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		protected Type enumType;

		public EnumLogicEntityBase(FrostySdk.Ebx.EnumLogicEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
			enumType = TypeLibrary.GetType(Data.TypeNameHash);
		}
	}
}

