using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIElementWorldProjectionEntityData))]
	public class WSUIElementWorldProjectionEntity : WSUIElementEntity, IEntityData<FrostySdk.Ebx.WSUIElementWorldProjectionEntityData>
	{
		public new FrostySdk.Ebx.WSUIElementWorldProjectionEntityData Data => data as FrostySdk.Ebx.WSUIElementWorldProjectionEntityData;
		public override string DisplayName => "WSUIElementWorldProjection";

		public WSUIElementWorldProjectionEntity(FrostySdk.Ebx.WSUIElementWorldProjectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

