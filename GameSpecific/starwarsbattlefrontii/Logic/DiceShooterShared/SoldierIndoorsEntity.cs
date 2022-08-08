using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierIndoorsEntityData))]
	public class SoldierIndoorsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoldierIndoorsEntityData>
	{
		public new FrostySdk.Ebx.SoldierIndoorsEntityData Data => data as FrostySdk.Ebx.SoldierIndoorsEntityData;
		public override string DisplayName => "SoldierIndoors";

		public SoldierIndoorsEntity(FrostySdk.Ebx.SoldierIndoorsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

