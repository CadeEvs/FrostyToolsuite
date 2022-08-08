using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientProfilerData))]
	public class ClientProfiler : LogicEntity, IEntityData<FrostySdk.Ebx.ClientProfilerData>
	{
		public new FrostySdk.Ebx.ClientProfilerData Data => data as FrostySdk.Ebx.ClientProfilerData;
		public override string DisplayName => "ClientProfiler";

		public ClientProfiler(FrostySdk.Ebx.ClientProfilerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

