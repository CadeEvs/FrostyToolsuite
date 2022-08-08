using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SaveLoadAppearanceDataEntityData))]
	public class SaveLoadAppearanceDataEntity : SaveLoadAppearanceEntityBase, IEntityData<FrostySdk.Ebx.SaveLoadAppearanceDataEntityData>
	{
		public new FrostySdk.Ebx.SaveLoadAppearanceDataEntityData Data => data as FrostySdk.Ebx.SaveLoadAppearanceDataEntityData;
		public override string DisplayName => "SaveLoadAppearanceData";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("LoadData", Direction.In),
				new ConnectionDesc("SaveData", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("CustomizedCharacterId", Direction.In)
			};
		}

		public SaveLoadAppearanceDataEntity(FrostySdk.Ebx.SaveLoadAppearanceDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

