using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPGenericProfilerData))]
	public class SPGenericProfiler : LogicEntity, IEntityData<FrostySdk.Ebx.SPGenericProfilerData>
	{
		public new FrostySdk.Ebx.SPGenericProfilerData Data => data as FrostySdk.Ebx.SPGenericProfilerData;
		public override string DisplayName => "SPGenericProfiler";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SPGenericProfiler(FrostySdk.Ebx.SPGenericProfilerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

