using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ApplyPlayerPresetEntityData))]
	public class ApplyPlayerPresetEntity : SaveLoadAppearanceEntityBase, IEntityData<FrostySdk.Ebx.ApplyPlayerPresetEntityData>
	{
		public new FrostySdk.Ebx.ApplyPlayerPresetEntityData Data => data as FrostySdk.Ebx.ApplyPlayerPresetEntityData;
		public override string DisplayName => "ApplyPlayerPreset";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ApplyPlayerPreset", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("HeadId", Direction.In)
			};
		}

		public ApplyPlayerPresetEntity(FrostySdk.Ebx.ApplyPlayerPresetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

