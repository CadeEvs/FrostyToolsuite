using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TraceLayerEntitiesData))]
	public class TraceLayerEntities : LogicEntity, IEntityData<FrostySdk.Ebx.TraceLayerEntitiesData>
	{
		public new FrostySdk.Ebx.TraceLayerEntitiesData Data => data as FrostySdk.Ebx.TraceLayerEntitiesData;
		public override string DisplayName => "TraceLayerEntities";
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Entities", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Layer", Direction.In)
			};
		}

        public TraceLayerEntities(FrostySdk.Ebx.TraceLayerEntitiesData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

