using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GamerpicEntityData))]
	public class GamerpicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GamerpicEntityData>
	{
		public new FrostySdk.Ebx.GamerpicEntityData Data => data as FrostySdk.Ebx.GamerpicEntityData;
		public override string DisplayName => "Gamerpic";

		public GamerpicEntity(FrostySdk.Ebx.GamerpicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

