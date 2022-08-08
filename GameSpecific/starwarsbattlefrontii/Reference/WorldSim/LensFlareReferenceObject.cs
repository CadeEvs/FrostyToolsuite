using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LensFlareReferenceObjectData))]
	public class LensFlareReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.LensFlareReferenceObjectData>
	{
		public new FrostySdk.Ebx.LensFlareReferenceObjectData Data => data as FrostySdk.Ebx.LensFlareReferenceObjectData;

		public LensFlareReferenceObject(FrostySdk.Ebx.LensFlareReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

