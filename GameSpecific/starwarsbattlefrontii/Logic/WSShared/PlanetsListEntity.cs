using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetsListEntityData))]
	public class PlanetsListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetsListEntityData>
	{
		public new FrostySdk.Ebx.PlanetsListEntityData Data => data as FrostySdk.Ebx.PlanetsListEntityData;
		public override string DisplayName => "PlanetsList";

		public PlanetsListEntity(FrostySdk.Ebx.PlanetsListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

