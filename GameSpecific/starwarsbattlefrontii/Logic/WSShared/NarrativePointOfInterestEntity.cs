using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativePointOfInterestEntityData))]
	public class NarrativePointOfInterestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NarrativePointOfInterestEntityData>
	{
		public new FrostySdk.Ebx.NarrativePointOfInterestEntityData Data => data as FrostySdk.Ebx.NarrativePointOfInterestEntityData;
		public override string DisplayName => "NarrativePointOfInterest";

		public NarrativePointOfInterestEntity(FrostySdk.Ebx.NarrativePointOfInterestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

