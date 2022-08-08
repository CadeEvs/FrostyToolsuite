using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldPointGeneratorEntityBaseData))]
	public class WorldPointGeneratorEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.WorldPointGeneratorEntityBaseData>
	{
		public new FrostySdk.Ebx.WorldPointGeneratorEntityBaseData Data => data as FrostySdk.Ebx.WorldPointGeneratorEntityBaseData;
		public override string DisplayName => "WorldPointGeneratorEntityBase";

		public WorldPointGeneratorEntityBase(FrostySdk.Ebx.WorldPointGeneratorEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

