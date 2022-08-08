using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MusicInstanceManagerEntityData))]
	public class MusicInstanceManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MusicInstanceManagerEntityData>
	{
		public new FrostySdk.Ebx.MusicInstanceManagerEntityData Data => data as FrostySdk.Ebx.MusicInstanceManagerEntityData;
		public override string DisplayName => "MusicInstanceManager";

		public MusicInstanceManagerEntity(FrostySdk.Ebx.MusicInstanceManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

