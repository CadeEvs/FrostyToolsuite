using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShapePointGeneratorEntityData))]
	public class ShapePointGeneratorEntity : WorldPointGeneratorEntityBase, IEntityData<FrostySdk.Ebx.ShapePointGeneratorEntityData>
	{
		public new FrostySdk.Ebx.ShapePointGeneratorEntityData Data => data as FrostySdk.Ebx.ShapePointGeneratorEntityData;
		public override string DisplayName => "ShapePointGenerator";

		public ShapePointGeneratorEntity(FrostySdk.Ebx.ShapePointGeneratorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

