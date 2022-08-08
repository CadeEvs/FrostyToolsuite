using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AssertEntityData))]
	public class AssertEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AssertEntityData>
	{
		public new FrostySdk.Ebx.AssertEntityData Data => data as FrostySdk.Ebx.AssertEntityData;
		public override string DisplayName => "Assert";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AssertEntity(FrostySdk.Ebx.AssertEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

