using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SnowglobeEntityData))]
	public class SnowglobeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SnowglobeEntityData>
	{
		public new FrostySdk.Ebx.SnowglobeEntityData Data => data as FrostySdk.Ebx.SnowglobeEntityData;
		public override string DisplayName => "Snowglobe";

		public SnowglobeEntity(FrostySdk.Ebx.SnowglobeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

