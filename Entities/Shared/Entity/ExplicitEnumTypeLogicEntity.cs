using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExplicitEnumTypeLogicEntityData))]
	public class ExplicitEnumTypeLogicEntity : EnumLogicEntityBase, IEntityData<FrostySdk.Ebx.ExplicitEnumTypeLogicEntityData>
	{
		public new FrostySdk.Ebx.ExplicitEnumTypeLogicEntityData Data => data as FrostySdk.Ebx.ExplicitEnumTypeLogicEntityData;
		public override string DisplayName => "ExplicitEnumTypeLogic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ExplicitEnumTypeLogicEntity(FrostySdk.Ebx.ExplicitEnumTypeLogicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

