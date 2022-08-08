using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPlayVideoEntityData))]
	public class MEPlayVideoEntity : PlayVideoEntity, IEntityData<FrostySdk.Ebx.MEPlayVideoEntityData>
	{
		public new FrostySdk.Ebx.MEPlayVideoEntityData Data => data as FrostySdk.Ebx.MEPlayVideoEntityData;
		public override string DisplayName => "MEPlayVideo";

		public MEPlayVideoEntity(FrostySdk.Ebx.MEPlayVideoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

