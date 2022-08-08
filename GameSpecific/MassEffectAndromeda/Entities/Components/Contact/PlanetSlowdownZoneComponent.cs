using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetSlowdownZoneComponentData))]
	public class PlanetSlowdownZoneComponent : GameComponent, IEntityData<FrostySdk.Ebx.PlanetSlowdownZoneComponentData>
	{
		public new FrostySdk.Ebx.PlanetSlowdownZoneComponentData Data => data as FrostySdk.Ebx.PlanetSlowdownZoneComponentData;
		public override string DisplayName => "PlanetSlowdownZoneComponent";

		public PlanetSlowdownZoneComponent(FrostySdk.Ebx.PlanetSlowdownZoneComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

