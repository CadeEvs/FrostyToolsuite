using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetProfileOptionEntityData))]
	public class GetProfileOptionEntity : ProfileOptionEntity, IEntityData<FrostySdk.Ebx.GetProfileOptionEntityData>
	{
		public new FrostySdk.Ebx.GetProfileOptionEntityData Data => data as FrostySdk.Ebx.GetProfileOptionEntityData;
		public override string DisplayName => "GetProfileOption";

		public GetProfileOptionEntity(FrostySdk.Ebx.GetProfileOptionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

