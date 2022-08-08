using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutManagerEntityData))]
	public class SPLoadoutManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutManagerEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutManagerEntityData Data => data as FrostySdk.Ebx.SPLoadoutManagerEntityData;
		public override string DisplayName => "SPLoadoutManager";

		public SPLoadoutManagerEntity(FrostySdk.Ebx.SPLoadoutManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

