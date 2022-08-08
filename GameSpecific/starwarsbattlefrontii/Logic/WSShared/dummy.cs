using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.dummyData))]
	public class dummy : LogicEntity, IEntityData<FrostySdk.Ebx.dummyData>
	{
		public new FrostySdk.Ebx.dummyData Data => data as FrostySdk.Ebx.dummyData;
		public override string DisplayName => "dummy";

		public dummy(FrostySdk.Ebx.dummyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

