using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeroNarrativeStateSetterEntityData))]
	public class HeroNarrativeStateSetterEntity : NarrativeStateSetterEntity, IEntityData<FrostySdk.Ebx.HeroNarrativeStateSetterEntityData>
	{
		public new FrostySdk.Ebx.HeroNarrativeStateSetterEntityData Data => data as FrostySdk.Ebx.HeroNarrativeStateSetterEntityData;
		public override string DisplayName => "HeroNarrativeStateSetter";

		public HeroNarrativeStateSetterEntity(FrostySdk.Ebx.HeroNarrativeStateSetterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

