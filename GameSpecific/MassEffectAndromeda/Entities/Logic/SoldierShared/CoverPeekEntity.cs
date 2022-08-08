using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverPeekEntityData))]
	public class CoverPeekEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoverPeekEntityData>
	{
		public new FrostySdk.Ebx.CoverPeekEntityData Data => data as FrostySdk.Ebx.CoverPeekEntityData;
		public override string DisplayName => "CoverPeek";

		public CoverPeekEntity(FrostySdk.Ebx.CoverPeekEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

