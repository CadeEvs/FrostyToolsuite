using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TraceLayerManagerEntityData))]
	public class TraceLayerManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TraceLayerManagerEntityData>
	{
		public new FrostySdk.Ebx.TraceLayerManagerEntityData Data => data as FrostySdk.Ebx.TraceLayerManagerEntityData;
		public override string DisplayName => "TraceLayerManager";

		public TraceLayerManagerEntity(FrostySdk.Ebx.TraceLayerManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

