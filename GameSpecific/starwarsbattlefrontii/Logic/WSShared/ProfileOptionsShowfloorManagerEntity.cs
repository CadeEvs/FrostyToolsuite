using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProfileOptionsShowfloorManagerEntityData))]
	public class ProfileOptionsShowfloorManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProfileOptionsShowfloorManagerEntityData>
	{
		public new FrostySdk.Ebx.ProfileOptionsShowfloorManagerEntityData Data => data as FrostySdk.Ebx.ProfileOptionsShowfloorManagerEntityData;
		public override string DisplayName => "ProfileOptionsShowfloorManager";

		public ProfileOptionsShowfloorManagerEntity(FrostySdk.Ebx.ProfileOptionsShowfloorManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

