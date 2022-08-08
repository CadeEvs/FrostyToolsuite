using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugBase2DEntityData))]
	public class DrawDebugBase2DEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugBase2DEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugBase2DEntityData Data => data as FrostySdk.Ebx.DrawDebugBase2DEntityData;
		public override string DisplayName => "DrawDebugBase2D";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugBase2DEntity(FrostySdk.Ebx.DrawDebugBase2DEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

