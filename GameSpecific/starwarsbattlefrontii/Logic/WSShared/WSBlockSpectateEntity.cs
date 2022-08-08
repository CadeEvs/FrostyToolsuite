using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSBlockSpectateEntityData))]
	public class WSBlockSpectateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSBlockSpectateEntityData>
	{
		public new FrostySdk.Ebx.WSBlockSpectateEntityData Data => data as FrostySdk.Ebx.WSBlockSpectateEntityData;
		public override string DisplayName => "WSBlockSpectate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSBlockSpectateEntity(FrostySdk.Ebx.WSBlockSpectateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

