using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SaveLoadCustomizationSetEntityData))]
	public class SaveLoadCustomizationSetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SaveLoadCustomizationSetEntityData>
	{
		public new FrostySdk.Ebx.SaveLoadCustomizationSetEntityData Data => data as FrostySdk.Ebx.SaveLoadCustomizationSetEntityData;
		public override string DisplayName => "SaveLoadCustomizationSet";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SaveLoadCustomizationSetEntity(FrostySdk.Ebx.SaveLoadCustomizationSetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

