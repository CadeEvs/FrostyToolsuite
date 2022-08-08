using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ViabilitySystemManagerEntityData))]
	public class ViabilitySystemManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ViabilitySystemManagerEntityData>
	{
		public new FrostySdk.Ebx.ViabilitySystemManagerEntityData Data => data as FrostySdk.Ebx.ViabilitySystemManagerEntityData;
		public override string DisplayName => "ViabilitySystemManager";

		public ViabilitySystemManagerEntity(FrostySdk.Ebx.ViabilitySystemManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

