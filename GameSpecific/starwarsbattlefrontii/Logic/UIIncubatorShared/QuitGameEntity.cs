using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QuitGameEntityData))]
	public class QuitGameEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QuitGameEntityData>
	{
		public new FrostySdk.Ebx.QuitGameEntityData Data => data as FrostySdk.Ebx.QuitGameEntityData;
		public override string DisplayName => "QuitGame";

		public QuitGameEntity(FrostySdk.Ebx.QuitGameEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

