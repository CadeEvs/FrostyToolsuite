using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSnowglobeEntityData))]
	public class WSSnowglobeEntity : SnowglobeEntity, IEntityData<FrostySdk.Ebx.WSSnowglobeEntityData>
	{
		public new FrostySdk.Ebx.WSSnowglobeEntityData Data => data as FrostySdk.Ebx.WSSnowglobeEntityData;
		public override string DisplayName => "WSSnowglobe";

		public WSSnowglobeEntity(FrostySdk.Ebx.WSSnowglobeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

