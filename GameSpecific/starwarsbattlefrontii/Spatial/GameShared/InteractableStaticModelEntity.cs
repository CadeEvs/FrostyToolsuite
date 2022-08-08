using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InteractableStaticModelEntityData))]
	public class InteractableStaticModelEntity : StaticModelEntity, IEntityData<FrostySdk.Ebx.InteractableStaticModelEntityData>
	{
		public new FrostySdk.Ebx.InteractableStaticModelEntityData Data => data as FrostySdk.Ebx.InteractableStaticModelEntityData;

		public InteractableStaticModelEntity(FrostySdk.Ebx.InteractableStaticModelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

