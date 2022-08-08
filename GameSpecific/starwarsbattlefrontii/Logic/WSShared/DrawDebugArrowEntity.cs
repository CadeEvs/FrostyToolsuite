using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugArrowEntityData))]
	public class DrawDebugArrowEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugArrowEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugArrowEntityData Data => data as FrostySdk.Ebx.DrawDebugArrowEntityData;
		public override string DisplayName => "DrawDebugArrow";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugArrowEntity(FrostySdk.Ebx.DrawDebugArrowEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

