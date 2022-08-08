using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AdhocSpawnerLinkEntityHelperData))]
	public class AdhocSpawnerLinkEntityHelper : LogicEntity, IEntityData<FrostySdk.Ebx.AdhocSpawnerLinkEntityHelperData>
	{
		public new FrostySdk.Ebx.AdhocSpawnerLinkEntityHelperData Data => data as FrostySdk.Ebx.AdhocSpawnerLinkEntityHelperData;
		public override string DisplayName => "AdhocSpawnerLinkEntityHelper";

		public AdhocSpawnerLinkEntityHelper(FrostySdk.Ebx.AdhocSpawnerLinkEntityHelperData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

