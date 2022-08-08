using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompositionSettingsEntityData))]
	public class CompositionSettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CompositionSettingsEntityData>
	{
		public new FrostySdk.Ebx.CompositionSettingsEntityData Data => data as FrostySdk.Ebx.CompositionSettingsEntityData;
		public override string DisplayName => "CompositionSettings";

		public CompositionSettingsEntity(FrostySdk.Ebx.CompositionSettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

