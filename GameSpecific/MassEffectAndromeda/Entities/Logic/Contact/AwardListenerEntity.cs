using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwardListenerEntityData))]
	public class AwardListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AwardListenerEntityData>
	{
		public new FrostySdk.Ebx.AwardListenerEntityData Data => data as FrostySdk.Ebx.AwardListenerEntityData;
		public override string DisplayName => "AwardListener";

		public AwardListenerEntity(FrostySdk.Ebx.AwardListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

