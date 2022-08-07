using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RunningAverageEntityData))]
	public class RunningAverageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RunningAverageEntityData>
	{
		public new FrostySdk.Ebx.RunningAverageEntityData Data => data as FrostySdk.Ebx.RunningAverageEntityData;
		public override string DisplayName => "RunningAverage";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RunningAverageEntity(FrostySdk.Ebx.RunningAverageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

