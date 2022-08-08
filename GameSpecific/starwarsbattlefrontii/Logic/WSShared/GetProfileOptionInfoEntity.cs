using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetProfileOptionInfoEntityData))]
	public class GetProfileOptionInfoEntity : ProfileOptionEntity, IEntityData<FrostySdk.Ebx.GetProfileOptionInfoEntityData>
	{
		public new FrostySdk.Ebx.GetProfileOptionInfoEntityData Data => data as FrostySdk.Ebx.GetProfileOptionInfoEntityData;
		public override string DisplayName => "GetProfileOptionInfo";

		public GetProfileOptionInfoEntity(FrostySdk.Ebx.GetProfileOptionInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

