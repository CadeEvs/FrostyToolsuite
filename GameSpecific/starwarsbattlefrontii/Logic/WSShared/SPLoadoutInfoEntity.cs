using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutInfoEntityData))]
	public class SPLoadoutInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutInfoEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutInfoEntityData Data => data as FrostySdk.Ebx.SPLoadoutInfoEntityData;
		public override string DisplayName => "SPLoadoutInfo";

		public SPLoadoutInfoEntity(FrostySdk.Ebx.SPLoadoutInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

