using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveLocationMarkerEntityData))]
	public class ObjectiveLocationMarkerEntity : LocationMarkerEntity, IEntityData<FrostySdk.Ebx.ObjectiveLocationMarkerEntityData>
	{
		public new FrostySdk.Ebx.ObjectiveLocationMarkerEntityData Data => data as FrostySdk.Ebx.ObjectiveLocationMarkerEntityData;

		public ObjectiveLocationMarkerEntity(FrostySdk.Ebx.ObjectiveLocationMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

