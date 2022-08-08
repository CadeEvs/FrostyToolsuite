using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierEntityData))]
	public class SoldierEntity : CharacterEntity, IEntityData<FrostySdk.Ebx.SoldierEntityData>
	{
		public new FrostySdk.Ebx.SoldierEntityData Data => data as FrostySdk.Ebx.SoldierEntityData;

		public SoldierEntity(FrostySdk.Ebx.SoldierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

