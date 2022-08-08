using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProfileOptionsManagerEntityData))]
	public class ProfileOptionsManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProfileOptionsManagerEntityData>
	{
		public new FrostySdk.Ebx.ProfileOptionsManagerEntityData Data => data as FrostySdk.Ebx.ProfileOptionsManagerEntityData;
		public override string DisplayName => "ProfileOptionsManager";

		public ProfileOptionsManagerEntity(FrostySdk.Ebx.ProfileOptionsManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

