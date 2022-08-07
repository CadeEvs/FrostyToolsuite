using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ImpliedEnumTypeLogicEntityData))]
	public class ImpliedEnumTypeLogicEntity : EnumLogicEntityBase, IEntityData<FrostySdk.Ebx.ImpliedEnumTypeLogicEntityData>
	{
		public new FrostySdk.Ebx.ImpliedEnumTypeLogicEntityData Data => data as FrostySdk.Ebx.ImpliedEnumTypeLogicEntityData;
		public override string DisplayName => "ImpliedEnumTypeLogic";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ImpliedEnumTypeLogicEntity(FrostySdk.Ebx.ImpliedEnumTypeLogicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

