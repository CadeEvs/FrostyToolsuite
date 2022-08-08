using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoopSplitterEntityData))]
	public class CoopSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoopSplitterEntityData>
	{
		public new FrostySdk.Ebx.CoopSplitterEntityData Data => data as FrostySdk.Ebx.CoopSplitterEntityData;
		public override string DisplayName => "CoopSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CoopSplitterEntity(FrostySdk.Ebx.CoopSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

