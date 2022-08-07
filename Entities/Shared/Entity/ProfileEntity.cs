using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProfileEntityData))]
	public class ProfileEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProfileEntityData>
	{
		public new FrostySdk.Ebx.ProfileEntityData Data => data as FrostySdk.Ebx.ProfileEntityData;
		public override string DisplayName => "Profile";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProfileEntity(FrostySdk.Ebx.ProfileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

