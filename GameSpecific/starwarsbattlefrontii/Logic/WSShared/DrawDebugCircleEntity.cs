using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugCircleEntityData))]
	public class DrawDebugCircleEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugCircleEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugCircleEntityData Data => data as FrostySdk.Ebx.DrawDebugCircleEntityData;
		public override string DisplayName => "DrawDebugCircle";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugCircleEntity(FrostySdk.Ebx.DrawDebugCircleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

