using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlatformEntityData))]
	public class PlatformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlatformEntityData>
	{
		public new FrostySdk.Ebx.PlatformEntityData Data => data as FrostySdk.Ebx.PlatformEntityData;
		public override string DisplayName => "Platform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlatformEntity(FrostySdk.Ebx.PlatformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

