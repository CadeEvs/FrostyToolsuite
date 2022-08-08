using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExitToMenuEntityData))]
	public class ExitToMenuEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ExitToMenuEntityData>
	{
		public new FrostySdk.Ebx.ExitToMenuEntityData Data => data as FrostySdk.Ebx.ExitToMenuEntityData;
		public override string DisplayName => "ExitToMenu";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ExitToMenuEntity(FrostySdk.Ebx.ExitToMenuEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

