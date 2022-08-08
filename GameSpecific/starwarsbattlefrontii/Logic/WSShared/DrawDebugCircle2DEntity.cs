using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugCircle2DEntityData))]
	public class DrawDebugCircle2DEntity : DrawDebugBase2DEntity, IEntityData<FrostySdk.Ebx.DrawDebugCircle2DEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugCircle2DEntityData Data => data as FrostySdk.Ebx.DrawDebugCircle2DEntityData;
		public override string DisplayName => "DrawDebugCircle2D";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugCircle2DEntity(FrostySdk.Ebx.DrawDebugCircle2DEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

