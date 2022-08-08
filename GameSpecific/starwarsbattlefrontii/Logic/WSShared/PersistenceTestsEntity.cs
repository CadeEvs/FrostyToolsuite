using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistenceTestsEntityData))]
	public class PersistenceTestsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PersistenceTestsEntityData>
	{
		public new FrostySdk.Ebx.PersistenceTestsEntityData Data => data as FrostySdk.Ebx.PersistenceTestsEntityData;
		public override string DisplayName => "PersistenceTests";

		public PersistenceTestsEntity(FrostySdk.Ebx.PersistenceTestsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

