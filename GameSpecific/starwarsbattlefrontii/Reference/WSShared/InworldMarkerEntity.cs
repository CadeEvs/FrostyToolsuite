using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InworldMarkerEntityData))]
	public class InworldMarkerEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.InworldMarkerEntityData>
	{
		public new FrostySdk.Ebx.InworldMarkerEntityData Data => data as FrostySdk.Ebx.InworldMarkerEntityData;

		public InworldMarkerEntity(FrostySdk.Ebx.InworldMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

