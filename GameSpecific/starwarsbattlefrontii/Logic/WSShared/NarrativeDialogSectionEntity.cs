using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativeDialogSectionEntityData))]
	public class NarrativeDialogSectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NarrativeDialogSectionEntityData>
	{
		public new FrostySdk.Ebx.NarrativeDialogSectionEntityData Data => data as FrostySdk.Ebx.NarrativeDialogSectionEntityData;
		public override string DisplayName => "NarrativeDialogSection";

		public NarrativeDialogSectionEntity(FrostySdk.Ebx.NarrativeDialogSectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

