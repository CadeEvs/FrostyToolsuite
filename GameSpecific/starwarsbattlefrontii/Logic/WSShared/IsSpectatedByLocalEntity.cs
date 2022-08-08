using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsSpectatedByLocalEntityData))]
	public class IsSpectatedByLocalEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsSpectatedByLocalEntityData>
	{
		public new FrostySdk.Ebx.IsSpectatedByLocalEntityData Data => data as FrostySdk.Ebx.IsSpectatedByLocalEntityData;
		public override string DisplayName => "IsSpectatedByLocal";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IsSpectatedByLocalEntity(FrostySdk.Ebx.IsSpectatedByLocalEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

