using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticMorphBlendEntityData))]
	public class StaticMorphBlendEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StaticMorphBlendEntityData>
	{
		public new FrostySdk.Ebx.StaticMorphBlendEntityData Data => data as FrostySdk.Ebx.StaticMorphBlendEntityData;
		public override string DisplayName => "StaticMorphBlend";

		public StaticMorphBlendEntity(FrostySdk.Ebx.StaticMorphBlendEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

