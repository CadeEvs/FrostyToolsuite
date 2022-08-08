using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugLine2DEntityData))]
	public class DrawDebugLine2DEntity : DrawDebugBase2DEntity, IEntityData<FrostySdk.Ebx.DrawDebugLine2DEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugLine2DEntityData Data => data as FrostySdk.Ebx.DrawDebugLine2DEntityData;
		public override string DisplayName => "DrawDebugLine2D";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugLine2DEntity(FrostySdk.Ebx.DrawDebugLine2DEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

