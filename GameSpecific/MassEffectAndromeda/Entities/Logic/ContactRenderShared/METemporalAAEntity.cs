using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.METemporalAAEntityData))]
	public class METemporalAAEntity : LogicEntity, IEntityData<FrostySdk.Ebx.METemporalAAEntityData>
	{
		public new FrostySdk.Ebx.METemporalAAEntityData Data => data as FrostySdk.Ebx.METemporalAAEntityData;
		public override string DisplayName => "METemporalAA";

		public METemporalAAEntity(FrostySdk.Ebx.METemporalAAEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

