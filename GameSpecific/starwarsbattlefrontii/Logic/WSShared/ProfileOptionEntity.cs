using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProfileOptionEntityData))]
	public class ProfileOptionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProfileOptionEntityData>
	{
		public new FrostySdk.Ebx.ProfileOptionEntityData Data => data as FrostySdk.Ebx.ProfileOptionEntityData;
		public override string DisplayName => "ProfileOption";

		public ProfileOptionEntity(FrostySdk.Ebx.ProfileOptionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

