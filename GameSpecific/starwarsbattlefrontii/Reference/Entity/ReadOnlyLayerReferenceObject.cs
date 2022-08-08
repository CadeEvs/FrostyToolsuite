using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReadOnlyLayerReferenceObjectData))]
	public class ReadOnlyLayerReferenceObject : LayerReferenceObject, IEntityData<FrostySdk.Ebx.ReadOnlyLayerReferenceObjectData>
	{
		public new FrostySdk.Ebx.ReadOnlyLayerReferenceObjectData Data => data as FrostySdk.Ebx.ReadOnlyLayerReferenceObjectData;

		public ReadOnlyLayerReferenceObject(FrostySdk.Ebx.ReadOnlyLayerReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

