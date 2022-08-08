using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativeStateSetterEntityData))]
	public class NarrativeStateSetterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NarrativeStateSetterEntityData>
	{
		public new FrostySdk.Ebx.NarrativeStateSetterEntityData Data => data as FrostySdk.Ebx.NarrativeStateSetterEntityData;
		public override string DisplayName => "NarrativeStateSetter";

		public NarrativeStateSetterEntity(FrostySdk.Ebx.NarrativeStateSetterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

