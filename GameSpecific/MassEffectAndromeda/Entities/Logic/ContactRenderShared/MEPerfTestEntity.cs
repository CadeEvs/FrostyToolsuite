using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPerfTestEntityData))]
	public class MEPerfTestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEPerfTestEntityData>
	{
		public new FrostySdk.Ebx.MEPerfTestEntityData Data => data as FrostySdk.Ebx.MEPerfTestEntityData;
		public override string DisplayName => "MEPerfTest";

		public MEPerfTestEntity(FrostySdk.Ebx.MEPerfTestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

