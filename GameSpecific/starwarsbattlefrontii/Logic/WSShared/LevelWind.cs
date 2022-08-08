using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LevelWindData))]
	public class LevelWind : LogicEntity, IEntityData<FrostySdk.Ebx.LevelWindData>
	{
		public new FrostySdk.Ebx.LevelWindData Data => data as FrostySdk.Ebx.LevelWindData;
		public override string DisplayName => "LevelWind";

		public LevelWind(FrostySdk.Ebx.LevelWindData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

