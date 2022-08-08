using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StorytellerSocketData))]
	public class StorytellerSocket : LocatorEntity, IEntityData<FrostySdk.Ebx.StorytellerSocketData>
	{
		public new FrostySdk.Ebx.StorytellerSocketData Data => data as FrostySdk.Ebx.StorytellerSocketData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StorytellerSocket(FrostySdk.Ebx.StorytellerSocketData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

