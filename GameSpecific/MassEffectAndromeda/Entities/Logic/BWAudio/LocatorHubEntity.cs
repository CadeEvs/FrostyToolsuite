using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocatorHubEntityData))]
	public class LocatorHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocatorHubEntityData>
	{
		public new FrostySdk.Ebx.LocatorHubEntityData Data => data as FrostySdk.Ebx.LocatorHubEntityData;
		public override string DisplayName => "LocatorHub";

		public LocatorHubEntity(FrostySdk.Ebx.LocatorHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

