using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerEntityData))]
	public class AutoPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoPlayerEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerEntityData Data => data as FrostySdk.Ebx.AutoPlayerEntityData;
		public override string DisplayName => "AutoPlayer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerEntity(FrostySdk.Ebx.AutoPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

