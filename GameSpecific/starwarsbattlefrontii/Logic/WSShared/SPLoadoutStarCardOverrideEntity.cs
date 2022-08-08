using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutStarCardOverrideEntityData))]
	public class SPLoadoutStarCardOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutStarCardOverrideEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutStarCardOverrideEntityData Data => data as FrostySdk.Ebx.SPLoadoutStarCardOverrideEntityData;
		public override string DisplayName => "SPLoadoutStarCardOverride";

		public SPLoadoutStarCardOverrideEntity(FrostySdk.Ebx.SPLoadoutStarCardOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

