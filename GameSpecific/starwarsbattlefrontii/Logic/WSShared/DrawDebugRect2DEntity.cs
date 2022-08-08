using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugRect2DEntityData))]
	public class DrawDebugRect2DEntity : DrawDebugBase2DEntity, IEntityData<FrostySdk.Ebx.DrawDebugRect2DEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugRect2DEntityData Data => data as FrostySdk.Ebx.DrawDebugRect2DEntityData;
		public override string DisplayName => "DrawDebugRect2D";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugRect2DEntity(FrostySdk.Ebx.DrawDebugRect2DEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

