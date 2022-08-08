using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PopulationControllerEntityData))]
	public class PopulationControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PopulationControllerEntityData>
	{
		public new FrostySdk.Ebx.PopulationControllerEntityData Data => data as FrostySdk.Ebx.PopulationControllerEntityData;
		public override string DisplayName => "PopulationController";

		public PopulationControllerEntity(FrostySdk.Ebx.PopulationControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

