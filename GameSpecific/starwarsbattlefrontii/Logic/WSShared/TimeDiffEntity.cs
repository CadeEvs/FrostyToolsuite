using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TimeDiffEntityData))]
	public class TimeDiffEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TimeDiffEntityData>
	{
		public new FrostySdk.Ebx.TimeDiffEntityData Data => data as FrostySdk.Ebx.TimeDiffEntityData;
		public override string DisplayName => "TimeDiff";

		public TimeDiffEntity(FrostySdk.Ebx.TimeDiffEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

