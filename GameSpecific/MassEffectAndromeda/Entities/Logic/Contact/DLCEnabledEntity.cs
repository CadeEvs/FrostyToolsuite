using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DLCEnabledEntityData))]
	public class DLCEnabledEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DLCEnabledEntityData>
	{
		public new FrostySdk.Ebx.DLCEnabledEntityData Data => data as FrostySdk.Ebx.DLCEnabledEntityData;
		public override string DisplayName => "DLCEnabled";

		public DLCEnabledEntity(FrostySdk.Ebx.DLCEnabledEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

