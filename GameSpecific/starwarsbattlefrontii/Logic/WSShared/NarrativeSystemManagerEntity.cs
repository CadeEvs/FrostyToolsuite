using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativeSystemManagerEntityData))]
	public class NarrativeSystemManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NarrativeSystemManagerEntityData>
	{
		public new FrostySdk.Ebx.NarrativeSystemManagerEntityData Data => data as FrostySdk.Ebx.NarrativeSystemManagerEntityData;
		public override string DisplayName => "NarrativeSystemManager";

		public NarrativeSystemManagerEntity(FrostySdk.Ebx.NarrativeSystemManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

