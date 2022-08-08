using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AdhocSpawnerLinkEntityData))]
	public class AdhocSpawnerLinkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AdhocSpawnerLinkEntityData>
	{
		public new FrostySdk.Ebx.AdhocSpawnerLinkEntityData Data => data as FrostySdk.Ebx.AdhocSpawnerLinkEntityData;
		public override string DisplayName => "AdhocSpawnerLink";

		public AdhocSpawnerLinkEntity(FrostySdk.Ebx.AdhocSpawnerLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

