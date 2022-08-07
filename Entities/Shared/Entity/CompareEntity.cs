using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareEntityData))]
	public class CompareEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CompareEntityData>
	{
		public new FrostySdk.Ebx.CompareEntityData Data => data as FrostySdk.Ebx.CompareEntityData;
		public override string DisplayName => "Compare";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareEntity(FrostySdk.Ebx.CompareEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

