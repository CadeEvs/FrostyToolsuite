using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StreamingGateEntityData))]
	public class StreamingGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StreamingGateEntityData>
	{
		public new FrostySdk.Ebx.StreamingGateEntityData Data => data as FrostySdk.Ebx.StreamingGateEntityData;
		public override string DisplayName => "StreamingGate";

		public StreamingGateEntity(FrostySdk.Ebx.StreamingGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

