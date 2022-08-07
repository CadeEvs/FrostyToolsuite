using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareEnumEntityData))]
	public class CompareEnumEntity : ImpliedEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.CompareEnumEntityData>
	{
		public new FrostySdk.Ebx.CompareEnumEntityData Data => data as FrostySdk.Ebx.CompareEnumEntityData;
		public override string DisplayName => "CompareEnum";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareEnumEntity(FrostySdk.Ebx.CompareEnumEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

