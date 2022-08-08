using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSpectatorEntityData))]
	public class WSSpectatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSSpectatorEntityData>
	{
		public new FrostySdk.Ebx.WSSpectatorEntityData Data => data as FrostySdk.Ebx.WSSpectatorEntityData;
		public override string DisplayName => "WSSpectator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSSpectatorEntity(FrostySdk.Ebx.WSSpectatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

