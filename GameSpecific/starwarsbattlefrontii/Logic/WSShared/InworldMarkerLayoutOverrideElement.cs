using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InworldMarkerLayoutOverrideElementData))]
	public class InworldMarkerLayoutOverrideElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.InworldMarkerLayoutOverrideElementData>
	{
		public new FrostySdk.Ebx.InworldMarkerLayoutOverrideElementData Data => data as FrostySdk.Ebx.InworldMarkerLayoutOverrideElementData;
		public override string DisplayName => "InworldMarkerLayoutOverrideElement";

		public InworldMarkerLayoutOverrideElement(FrostySdk.Ebx.InworldMarkerLayoutOverrideElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

