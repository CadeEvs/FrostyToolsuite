using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientAIManagerEntityData))]
	public class AmbientAIManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AmbientAIManagerEntityData>
	{
		public new FrostySdk.Ebx.AmbientAIManagerEntityData Data => data as FrostySdk.Ebx.AmbientAIManagerEntityData;
		public override string DisplayName => "AmbientAIManager";

		public AmbientAIManagerEntity(FrostySdk.Ebx.AmbientAIManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

