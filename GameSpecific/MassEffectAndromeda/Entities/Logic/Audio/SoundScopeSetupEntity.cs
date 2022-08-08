using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundScopeSetupEntityData))]
	public class SoundScopeSetupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundScopeSetupEntityData>
	{
		public new FrostySdk.Ebx.SoundScopeSetupEntityData Data => data as FrostySdk.Ebx.SoundScopeSetupEntityData;
		public override string DisplayName => "SoundScopeSetup";

		public SoundScopeSetupEntity(FrostySdk.Ebx.SoundScopeSetupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

