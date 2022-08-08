using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetProfileOptionEntityData))]
	public class SetProfileOptionEntity : ProfileOptionEntity, IEntityData<FrostySdk.Ebx.SetProfileOptionEntityData>
	{
		public new FrostySdk.Ebx.SetProfileOptionEntityData Data => data as FrostySdk.Ebx.SetProfileOptionEntityData;
		public override string DisplayName => "SetProfileOption";

		public SetProfileOptionEntity(FrostySdk.Ebx.SetProfileOptionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

