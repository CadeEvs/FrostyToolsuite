using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Dx12SettingsEntityData))]
	public class Dx12SettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Dx12SettingsEntityData>
	{
		public new FrostySdk.Ebx.Dx12SettingsEntityData Data => data as FrostySdk.Ebx.Dx12SettingsEntityData;
		public override string DisplayName => "Dx12Settings";

		public Dx12SettingsEntity(FrostySdk.Ebx.Dx12SettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

