using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GalaxyDataManagerData))]
	public class GalaxyDataManager : LogicEntity, IEntityData<FrostySdk.Ebx.GalaxyDataManagerData>
	{
		public new FrostySdk.Ebx.GalaxyDataManagerData Data => data as FrostySdk.Ebx.GalaxyDataManagerData;
		public override string DisplayName => "GalaxyDataManager";

		public GalaxyDataManager(FrostySdk.Ebx.GalaxyDataManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

