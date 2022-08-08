using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MultipleActorScenarioEntityData))]
	public class MultipleActorScenarioEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.MultipleActorScenarioEntityData>
	{
		public new FrostySdk.Ebx.MultipleActorScenarioEntityData Data => data as FrostySdk.Ebx.MultipleActorScenarioEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MultipleActorScenarioEntity(FrostySdk.Ebx.MultipleActorScenarioEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

