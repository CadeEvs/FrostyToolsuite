using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PreviewBuildMarkerEntityData))]
	public class PreviewBuildMarkerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PreviewBuildMarkerEntityData>
	{
		public new FrostySdk.Ebx.PreviewBuildMarkerEntityData Data => data as FrostySdk.Ebx.PreviewBuildMarkerEntityData;
		public override string DisplayName => "PreviewBuildMarker";

		public PreviewBuildMarkerEntity(FrostySdk.Ebx.PreviewBuildMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

