
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeatheringComponentData))]
	public class WeatheringComponent : GameComponent, IEntityData<FrostySdk.Ebx.WeatheringComponentData>
	{
		public new FrostySdk.Ebx.WeatheringComponentData Data => data as FrostySdk.Ebx.WeatheringComponentData;
		public override string DisplayName => "WeatheringComponent";

		public WeatheringComponent(FrostySdk.Ebx.WeatheringComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

