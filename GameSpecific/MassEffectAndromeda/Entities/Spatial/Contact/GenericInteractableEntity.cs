using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GenericInteractableEntityData))]
	public class GenericInteractableEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.GenericInteractableEntityData>
	{
		public new FrostySdk.Ebx.GenericInteractableEntityData Data => data as FrostySdk.Ebx.GenericInteractableEntityData;

		public GenericInteractableEntity(FrostySdk.Ebx.GenericInteractableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

