using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinearAreaStreamerEntityData))]
	public class LinearAreaStreamerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LinearAreaStreamerEntityData>
	{
		public new FrostySdk.Ebx.LinearAreaStreamerEntityData Data => data as FrostySdk.Ebx.LinearAreaStreamerEntityData;
		public override string DisplayName => "LinearAreaStreamer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LinearAreaStreamerEntity(FrostySdk.Ebx.LinearAreaStreamerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

