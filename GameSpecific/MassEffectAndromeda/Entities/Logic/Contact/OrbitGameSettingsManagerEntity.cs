using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrbitGameSettingsManagerEntityData))]
	public class OrbitGameSettingsManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OrbitGameSettingsManagerEntityData>
	{
		public new FrostySdk.Ebx.OrbitGameSettingsManagerEntityData Data => data as FrostySdk.Ebx.OrbitGameSettingsManagerEntityData;
		public override string DisplayName => "OrbitGameSettingsManager";

		public OrbitGameSettingsManagerEntity(FrostySdk.Ebx.OrbitGameSettingsManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

