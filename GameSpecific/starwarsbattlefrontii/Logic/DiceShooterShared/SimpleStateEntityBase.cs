using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleStateEntityBaseData))]
	public class SimpleStateEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.SimpleStateEntityBaseData>
	{
		public new FrostySdk.Ebx.SimpleStateEntityBaseData Data => data as FrostySdk.Ebx.SimpleStateEntityBaseData;
		public override string DisplayName => "SimpleStateEntityBase";

		public SimpleStateEntityBase(FrostySdk.Ebx.SimpleStateEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

