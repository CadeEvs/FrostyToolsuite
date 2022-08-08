using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SingletonEntityData))]
	public class SingletonEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SingletonEntityData>
	{
		public new FrostySdk.Ebx.SingletonEntityData Data => data as FrostySdk.Ebx.SingletonEntityData;
		public override string DisplayName => "Singleton";

		public SingletonEntity(FrostySdk.Ebx.SingletonEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

