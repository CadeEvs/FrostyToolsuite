using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugTextEntityData))]
	public class DebugTextEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.DebugTextEntityData>
	{
		public new FrostySdk.Ebx.DebugTextEntityData Data => data as FrostySdk.Ebx.DebugTextEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DebugTextEntity(FrostySdk.Ebx.DebugTextEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

