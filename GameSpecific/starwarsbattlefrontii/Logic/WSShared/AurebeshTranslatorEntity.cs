using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AurebeshTranslatorEntityData))]
	public class AurebeshTranslatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AurebeshTranslatorEntityData>
	{
		public new FrostySdk.Ebx.AurebeshTranslatorEntityData Data => data as FrostySdk.Ebx.AurebeshTranslatorEntityData;
		public override string DisplayName => "AurebeshTranslator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AurebeshTranslatorEntity(FrostySdk.Ebx.AurebeshTranslatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

