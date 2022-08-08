using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceLinePlayerAbilityEntityData))]
	public class VoiceLinePlayerAbilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceLinePlayerAbilityEntityData>
	{
		public new FrostySdk.Ebx.VoiceLinePlayerAbilityEntityData Data => data as FrostySdk.Ebx.VoiceLinePlayerAbilityEntityData;
		public override string DisplayName => "VoiceLinePlayerAbility";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VoiceLinePlayerAbilityEntity(FrostySdk.Ebx.VoiceLinePlayerAbilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

