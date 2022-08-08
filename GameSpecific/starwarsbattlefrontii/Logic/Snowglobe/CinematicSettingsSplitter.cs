using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicSettingsSplitterData))]
	public class CinematicSettingsSplitter : LogicEntity, IEntityData<FrostySdk.Ebx.CinematicSettingsSplitterData>
	{
		public new FrostySdk.Ebx.CinematicSettingsSplitterData Data => data as FrostySdk.Ebx.CinematicSettingsSplitterData;
		public override string DisplayName => "CinematicSettingsSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CinematicSettingsSplitter(FrostySdk.Ebx.CinematicSettingsSplitterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

