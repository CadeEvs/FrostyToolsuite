using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightBarEntityData))]
	public class LightBarEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LightBarEntityData>
	{
		public new FrostySdk.Ebx.LightBarEntityData Data => data as FrostySdk.Ebx.LightBarEntityData;
		public override string DisplayName => "LightBar";

		public LightBarEntity(FrostySdk.Ebx.LightBarEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

