using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugBoxEntityData))]
	public class DrawDebugBoxEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugBoxEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugBoxEntityData Data => data as FrostySdk.Ebx.DrawDebugBoxEntityData;
		public override string DisplayName => "DrawDebugBox";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugBoxEntity(FrostySdk.Ebx.DrawDebugBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

