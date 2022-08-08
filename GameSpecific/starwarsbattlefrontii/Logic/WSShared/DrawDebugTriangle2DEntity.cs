using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugTriangle2DEntityData))]
	public class DrawDebugTriangle2DEntity : DrawDebugBase2DEntity, IEntityData<FrostySdk.Ebx.DrawDebugTriangle2DEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugTriangle2DEntityData Data => data as FrostySdk.Ebx.DrawDebugTriangle2DEntityData;
		public override string DisplayName => "DrawDebugTriangle2D";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugTriangle2DEntity(FrostySdk.Ebx.DrawDebugTriangle2DEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

