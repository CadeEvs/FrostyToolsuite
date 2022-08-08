using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerIteratorEntityData))]
	public class PlayerIteratorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerIteratorEntityData>
	{
		public new FrostySdk.Ebx.PlayerIteratorEntityData Data => data as FrostySdk.Ebx.PlayerIteratorEntityData;
		public override string DisplayName => "PlayerIterator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerIteratorEntity(FrostySdk.Ebx.PlayerIteratorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

