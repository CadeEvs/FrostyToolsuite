using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReleaseVersionEntityData))]
	public class ReleaseVersionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ReleaseVersionEntityData>
	{
		public new FrostySdk.Ebx.ReleaseVersionEntityData Data => data as FrostySdk.Ebx.ReleaseVersionEntityData;
		public override string DisplayName => "ReleaseVersion";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReleaseVersionEntity(FrostySdk.Ebx.ReleaseVersionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

