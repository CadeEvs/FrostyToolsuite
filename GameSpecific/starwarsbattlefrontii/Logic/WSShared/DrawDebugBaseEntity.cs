using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugBaseEntityData))]
	public class DrawDebugBaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DrawDebugBaseEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugBaseEntityData Data => data as FrostySdk.Ebx.DrawDebugBaseEntityData;
		public override string DisplayName => "DrawDebugBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugBaseEntity(FrostySdk.Ebx.DrawDebugBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

