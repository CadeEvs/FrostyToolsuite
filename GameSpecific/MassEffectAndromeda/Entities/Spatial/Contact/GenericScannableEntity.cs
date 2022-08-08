using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GenericScannableEntityData))]
	public class GenericScannableEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.GenericScannableEntityData>
	{
		public new FrostySdk.Ebx.GenericScannableEntityData Data => data as FrostySdk.Ebx.GenericScannableEntityData;

		public GenericScannableEntity(FrostySdk.Ebx.GenericScannableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

