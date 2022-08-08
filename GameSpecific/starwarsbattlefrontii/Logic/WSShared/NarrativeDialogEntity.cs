using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativeDialogEntityData))]
	public class NarrativeDialogEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NarrativeDialogEntityData>
	{
		public new FrostySdk.Ebx.NarrativeDialogEntityData Data => data as FrostySdk.Ebx.NarrativeDialogEntityData;
		public override string DisplayName => "NarrativeDialog";

		public NarrativeDialogEntity(FrostySdk.Ebx.NarrativeDialogEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

