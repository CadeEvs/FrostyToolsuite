using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QuickscopeControlEntityData))]
	public class QuickscopeControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QuickscopeControlEntityData>
	{
		public new FrostySdk.Ebx.QuickscopeControlEntityData Data => data as FrostySdk.Ebx.QuickscopeControlEntityData;
		public override string DisplayName => "QuickscopeControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QuickscopeControlEntity(FrostySdk.Ebx.QuickscopeControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

