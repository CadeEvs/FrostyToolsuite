using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareEntityBaseData))]
	public class CompareEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.CompareEntityBaseData>
	{
		public new FrostySdk.Ebx.CompareEntityBaseData Data => data as FrostySdk.Ebx.CompareEntityBaseData;
		public override string DisplayName => "CompareEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareEntityBase(FrostySdk.Ebx.CompareEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

