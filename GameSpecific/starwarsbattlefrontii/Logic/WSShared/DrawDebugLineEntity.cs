using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DrawDebugLineEntityData))]
	public class DrawDebugLineEntity : DrawDebugBaseEntity, IEntityData<FrostySdk.Ebx.DrawDebugLineEntityData>
	{
		public new FrostySdk.Ebx.DrawDebugLineEntityData Data => data as FrostySdk.Ebx.DrawDebugLineEntityData;
		public override string DisplayName => "DrawDebugLine";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DrawDebugLineEntity(FrostySdk.Ebx.DrawDebugLineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

