using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsDemoGateData))]
	public class IsDemoGate : LogicEntity, IEntityData<FrostySdk.Ebx.IsDemoGateData>
	{
		public new FrostySdk.Ebx.IsDemoGateData Data => data as FrostySdk.Ebx.IsDemoGateData;
		public override string DisplayName => "IsDemoGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IsDemoGate(FrostySdk.Ebx.IsDemoGateData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

