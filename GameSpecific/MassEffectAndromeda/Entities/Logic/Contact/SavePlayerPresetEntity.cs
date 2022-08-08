using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SavePlayerPresetEntityData))]
	public class SavePlayerPresetEntity : SaveLoadAppearanceEntityBase, IEntityData<FrostySdk.Ebx.SavePlayerPresetEntityData>
	{
		public new FrostySdk.Ebx.SavePlayerPresetEntityData Data => data as FrostySdk.Ebx.SavePlayerPresetEntityData;
		public override string DisplayName => "SavePlayerPreset";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("SavePlayerPreset", Direction.In)
			};
		}

		public SavePlayerPresetEntity(FrostySdk.Ebx.SavePlayerPresetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

