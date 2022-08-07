using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumDebugEntityData))]
	public class EnumDebugEntity : ImpliedEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.EnumDebugEntityData>
	{
		public new FrostySdk.Ebx.EnumDebugEntityData Data => data as FrostySdk.Ebx.EnumDebugEntityData;
		public override string DisplayName => "EnumDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumDebugEntity(FrostySdk.Ebx.EnumDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

