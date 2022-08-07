using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumPropertyGateEntityData))]
	public class EnumPropertyGateEntity : ImpliedEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.EnumPropertyGateEntityData>
	{
		public new FrostySdk.Ebx.EnumPropertyGateEntityData Data => data as FrostySdk.Ebx.EnumPropertyGateEntityData;
		public override string DisplayName => "EnumPropertyGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumPropertyGateEntity(FrostySdk.Ebx.EnumPropertyGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

