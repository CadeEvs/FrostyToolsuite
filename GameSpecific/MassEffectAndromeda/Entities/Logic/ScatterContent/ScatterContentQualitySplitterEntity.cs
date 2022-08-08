using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentQualitySplitterEntityData))]
	public class ScatterContentQualitySplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentQualitySplitterEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentQualitySplitterEntityData Data => data as FrostySdk.Ebx.ScatterContentQualitySplitterEntityData;
		public override string DisplayName => "ScatterContentQualitySplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ScatterContentQualitySplitterEntity(FrostySdk.Ebx.ScatterContentQualitySplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

