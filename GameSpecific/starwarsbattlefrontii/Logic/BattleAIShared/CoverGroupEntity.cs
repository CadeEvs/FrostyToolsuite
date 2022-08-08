using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverGroupEntityData))]
	public class CoverGroupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoverGroupEntityData>
	{
		public new FrostySdk.Ebx.CoverGroupEntityData Data => data as FrostySdk.Ebx.CoverGroupEntityData;
		public override string DisplayName => "CoverGroup";

		public CoverGroupEntity(FrostySdk.Ebx.CoverGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

