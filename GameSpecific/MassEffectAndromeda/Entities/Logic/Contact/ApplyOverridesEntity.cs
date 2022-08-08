using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ApplyOverridesEntityData))]
	public class ApplyOverridesEntity : SaveLoadAppearanceEntityBase, IEntityData<FrostySdk.Ebx.ApplyOverridesEntityData>
	{
		public new FrostySdk.Ebx.ApplyOverridesEntityData Data => data as FrostySdk.Ebx.ApplyOverridesEntityData;
		public override string DisplayName => "ApplyOverrides";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Appearance", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Overrides", Direction.In)
			};
		}

		public ApplyOverridesEntity(FrostySdk.Ebx.ApplyOverridesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

