using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AINarrativeStateSetterEntityData))]
	public class AINarrativeStateSetterEntity : NarrativeStateSetterEntity, IEntityData<FrostySdk.Ebx.AINarrativeStateSetterEntityData>
	{
		public new FrostySdk.Ebx.AINarrativeStateSetterEntityData Data => data as FrostySdk.Ebx.AINarrativeStateSetterEntityData;
		public override string DisplayName => "AINarrativeStateSetter";

		public AINarrativeStateSetterEntity(FrostySdk.Ebx.AINarrativeStateSetterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

