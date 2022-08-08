using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlatformSplitterEntityData))]
	public class PlatformSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlatformSplitterEntityData>
	{
		public new FrostySdk.Ebx.PlatformSplitterEntityData Data => data as FrostySdk.Ebx.PlatformSplitterEntityData;
		public override string DisplayName => "PlatformSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlatformSplitterEntity(FrostySdk.Ebx.PlatformSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

