using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotNamedTrackableEntityData))]
	public class CinebotNamedTrackableEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CinebotNamedTrackableEntityData>
	{
		public new FrostySdk.Ebx.CinebotNamedTrackableEntityData Data => data as FrostySdk.Ebx.CinebotNamedTrackableEntityData;
		public override string DisplayName => "CinebotNamedTrackable";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Trackable", Direction.In)
			};
		}

		public CinebotNamedTrackableEntity(FrostySdk.Ebx.CinebotNamedTrackableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

