using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutControlEntityData))]
	public class SPLoadoutControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutControlEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutControlEntityData Data => data as FrostySdk.Ebx.SPLoadoutControlEntityData;
		public override string DisplayName => "SPLoadoutControl";

		public SPLoadoutControlEntity(FrostySdk.Ebx.SPLoadoutControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

