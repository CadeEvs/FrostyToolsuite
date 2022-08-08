using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhantomEntityData))]
	public class PhantomEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PhantomEntityData>
	{
		public new FrostySdk.Ebx.PhantomEntityData Data => data as FrostySdk.Ebx.PhantomEntityData;
		public override string DisplayName => "Phantom";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhantomEntity(FrostySdk.Ebx.PhantomEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

