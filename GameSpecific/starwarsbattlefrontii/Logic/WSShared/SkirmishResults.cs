using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkirmishResultsData))]
	public class SkirmishResults : LogicEntity, IEntityData<FrostySdk.Ebx.SkirmishResultsData>
	{
		public new FrostySdk.Ebx.SkirmishResultsData Data => data as FrostySdk.Ebx.SkirmishResultsData;
		public override string DisplayName => "SkirmishResults";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SkirmishResults(FrostySdk.Ebx.SkirmishResultsData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

