using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugPieEntityData))]
	public class DrawDebugPieEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugPieEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugPieEntityData Data => data as FrostySdk.Ebx.DrawDebugPieEntityData;
		public override string DisplayName => "DrawDebugPie";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugPieEntity(FrostySdk.Ebx.DrawDebugPieEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

