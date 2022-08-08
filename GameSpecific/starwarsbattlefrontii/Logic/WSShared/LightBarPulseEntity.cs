using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightBarPulseEntityData))]
	public class LightBarPulseEntity : LightBarEntity, IEntityData<FrostySdk.Ebx.LightBarPulseEntityData>
	{
		public new FrostySdk.Ebx.LightBarPulseEntityData Data => data as FrostySdk.Ebx.LightBarPulseEntityData;
		public override string DisplayName => "LightBarPulse";

		public LightBarPulseEntity(FrostySdk.Ebx.LightBarPulseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

