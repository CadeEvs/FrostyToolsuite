using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SaveLoadAppearanceEntityBaseData))]
	public class SaveLoadAppearanceEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.SaveLoadAppearanceEntityBaseData>
	{
		public new FrostySdk.Ebx.SaveLoadAppearanceEntityBaseData Data => data as FrostySdk.Ebx.SaveLoadAppearanceEntityBaseData;
		public override string DisplayName => "SaveLoadAppearanceEntityBase";
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Appearance", Direction.In)
			};
        }

        public SaveLoadAppearanceEntityBase(FrostySdk.Ebx.SaveLoadAppearanceEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

