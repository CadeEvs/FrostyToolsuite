using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightBarMixEntityData))]
	public class LightBarMixEntity : LightBarEntity, IEntityData<FrostySdk.Ebx.LightBarMixEntityData>
	{
		public new FrostySdk.Ebx.LightBarMixEntityData Data => data as FrostySdk.Ebx.LightBarMixEntityData;
		public override string DisplayName => "LightBarMix";

		public LightBarMixEntity(FrostySdk.Ebx.LightBarMixEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

