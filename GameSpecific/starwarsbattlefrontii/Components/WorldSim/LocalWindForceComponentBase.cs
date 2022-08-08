
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceComponentBaseData))]
	public class LocalWindForceComponentBase : GameComponent, IEntityData<FrostySdk.Ebx.LocalWindForceComponentBaseData>
	{
		public new FrostySdk.Ebx.LocalWindForceComponentBaseData Data => data as FrostySdk.Ebx.LocalWindForceComponentBaseData;
		public override string DisplayName => "LocalWindForceComponentBase";

		public LocalWindForceComponentBase(FrostySdk.Ebx.LocalWindForceComponentBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

