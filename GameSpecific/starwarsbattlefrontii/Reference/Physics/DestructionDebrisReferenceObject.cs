using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestructionDebrisReferenceObjectData))]
	public class DestructionDebrisReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.DestructionDebrisReferenceObjectData>
	{
		public new FrostySdk.Ebx.DestructionDebrisReferenceObjectData Data => data as FrostySdk.Ebx.DestructionDebrisReferenceObjectData;

		public DestructionDebrisReferenceObject(FrostySdk.Ebx.DestructionDebrisReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

