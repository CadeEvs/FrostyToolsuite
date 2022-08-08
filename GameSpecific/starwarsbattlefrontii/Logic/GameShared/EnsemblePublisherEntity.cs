using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnsemblePublisherEntityData))]
	public class EnsemblePublisherEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EnsemblePublisherEntityData>
	{
		public new FrostySdk.Ebx.EnsemblePublisherEntityData Data => data as FrostySdk.Ebx.EnsemblePublisherEntityData;
		public override string DisplayName => "EnsemblePublisher";

		public EnsemblePublisherEntity(FrostySdk.Ebx.EnsemblePublisherEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

