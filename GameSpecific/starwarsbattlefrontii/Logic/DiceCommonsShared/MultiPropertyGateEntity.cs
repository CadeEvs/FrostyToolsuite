using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MultiPropertyGateEntityData))]
	public class MultiPropertyGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MultiPropertyGateEntityData>
	{
		public new FrostySdk.Ebx.MultiPropertyGateEntityData Data => data as FrostySdk.Ebx.MultiPropertyGateEntityData;
		public override string DisplayName => "MultiPropertyGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MultiPropertyGateEntity(FrostySdk.Ebx.MultiPropertyGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

