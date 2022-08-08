using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugConeEntityData))]
	public class DrawDebugConeEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugConeEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugConeEntityData Data => data as FrostySdk.Ebx.DrawDebugConeEntityData;
		public override string DisplayName => "DrawDebugCone";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugConeEntity(FrostySdk.Ebx.DrawDebugConeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

