using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroundLevelerEntityData))]
	public class GroundLevelerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GroundLevelerEntityData>
	{
		public new FrostySdk.Ebx.GroundLevelerEntityData Data => data as FrostySdk.Ebx.GroundLevelerEntityData;
		public override string DisplayName => "GroundLeveler";

		public GroundLevelerEntity(FrostySdk.Ebx.GroundLevelerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

