using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoplayerIsGameplayEntityData))]
	public class AutoplayerIsGameplayEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoplayerIsGameplayEntityData>
	{
		public new FrostySdk.Ebx.AutoplayerIsGameplayEntityData Data => data as FrostySdk.Ebx.AutoplayerIsGameplayEntityData;
		public override string DisplayName => "AutoplayerIsGameplay";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoplayerIsGameplayEntity(FrostySdk.Ebx.AutoplayerIsGameplayEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

