using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SuicideEntityData))]
	public class SuicideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SuicideEntityData>
	{
		public new FrostySdk.Ebx.SuicideEntityData Data => data as FrostySdk.Ebx.SuicideEntityData;
		public override string DisplayName => "Suicide";

		public SuicideEntity(FrostySdk.Ebx.SuicideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

